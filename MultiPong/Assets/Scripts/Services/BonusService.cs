using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkControllers;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class BonusService : MonoBehaviour
    {
        private const float _HORIZONTAL_PADDING = 0.1f;

        private GameModel _gameModel;
        private GameSettings _settings;
        private IGameFactory _factory;
        private RandomProvider _randomProvider;
        private List<BonusModel> _bonusList;
        private int _alreadyGeneratedCount;
        private List<BonusModel> _bonusesOnField;
        private IEnumerator _bonusGeneratingCoroutine;

        public void Setup(GameModel gameModel, GameSettings settings, IGameFactory factory, RandomProvider randomProvider)
        {
            _gameModel = gameModel;
            _settings = settings;
            _factory = factory;
            _randomProvider = randomProvider;
        }

        private void Start()
        {
            _bonusList = GameSource.Instance.BonusModels;
            _bonusesOnField = new List<BonusModel>();
            _bonusGeneratingCoroutine = BonusGenerating();
        }

        public void StartGenerate()
        {
            StartCoroutine(_bonusGeneratingCoroutine);
        }

        public void StopGenerate() 
        {
            StopCoroutine(_bonusGeneratingCoroutine);
        }

        public void CheckBonusCapture()
        {
            if (TryCaptureBonus(_gameModel.MatchData.SpawnedBall.transform.position, out var bonusModel))
            {
                ApplyBonusEffect(bonusModel);
                CaptureBonus(bonusModel.NetId);
            }
        }

        public void Clear()
        {
            _bonusesOnField.Clear();
        }

        private IEnumerator BonusGenerating()
        {
            while (true)
            {
                yield return new WaitForSeconds(_settings.BonusGeneratingTimeout);
                if (_alreadyGeneratedCount < _settings.BonusOnFieldLimit)
                {
                    _alreadyGeneratedCount++;
                    SpawnBonus();
                }
            }
        }

        private void SpawnBonus()
        {
            var halfAccessibleFieldWidth = (_settings.BoardWidth - _settings.BonusDiameter) / 2 - _HORIZONTAL_PADDING;
            _randomProvider.GetRandom(-halfAccessibleFieldWidth, halfAccessibleFieldWidth, out var bonusPosX);
            _randomProvider.GetRandom(_bonusList, out var bonusModel);

            _factory.SpawnBonus(new Vector3(bonusPosX, 0), bonusModel);
            _bonusesOnField.Add(bonusModel);
        }

        private void CaptureBonus(ulong bonusNetId)
        {
            var bonus = _bonusesOnField
                .Where(b => b.NetId == bonusNetId)
                .FirstOrDefault();
            if (bonus != null)
            {
                bonus.CanBeCaptured = false;
                _bonusesOnField.Remove(bonus);
                _factory.DespawnBonus(bonusNetId);
                _alreadyGeneratedCount--;
            }
        }

        private bool TryCaptureBonus(Vector3 ballPosition, out BonusModel bonusModel)
        {
            bonusModel = default;
            var bonusRadius = _settings.BonusDiameter / 2;
            if (ballPosition.y < -bonusRadius || ballPosition.y > bonusRadius)
                return false;
            foreach (var bonus in _bonusesOnField)
            {
                if (bonus.CanBeCaptured && 
                    ballPosition.x >= bonus.PosX - bonusRadius && 
                    ballPosition.x <= bonus.PosX + bonusRadius)
                {
                    bonusModel = bonus;
                    return true;
                }
            }
            return false;
        }

        private void ApplyBonusEffect(BonusModel bonusModel)
        {
            switch (bonusModel.BonusType)
            {
                case BonusType.None:
                    break;
                case BonusType.Shrink:
                    _gameModel.ApplyChangeSizeEffect(1 / bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
                case BonusType.Stretch:
                    _gameModel.ApplyChangeSizeEffect(bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
                case BonusType.SpeedUp:
                    _gameModel.ApplySpeedUpEffect(bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
            }
        }
    }
}
