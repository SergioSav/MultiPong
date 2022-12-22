using Assets.Scripts.Data;
using Assets.Scripts.Models;
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
        private const string _BONUS_SETTINGS_URL = "BonusScriptableObject";

        [SerializeField] private BonusController _bonusPrototype;

        private GameSettings _settings;
        private RandomProvider _randomProvider;
        private List<BonusModel> _bonusList;
        private int _alreadyGeneratedCount;
        private List<BonusController> _bonusesOnField;

        public void Setup(GameSettings settings, RandomProvider randomProvider)
        {
            _settings = settings;
            _randomProvider = randomProvider;
        }

        private void Start()
        {
            _bonusList = Resources.Load<BonusScriptableObject>(_BONUS_SETTINGS_URL).BonusModels;
            _bonusesOnField = new List<BonusController>();
        }

        public void StartGenerate()
        {
            StartCoroutine(BonusGenerating());
        }

        public void StopGenerate() 
        {
            StopCoroutine(BonusGenerating());
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
            var bonus = Instantiate(_bonusPrototype, new Vector3(bonusPosX, 0), Quaternion.identity);
            _randomProvider.GetRandom(_bonusList, out var bonusModel);
            bonus.Setup(bonusModel);
            _bonusesOnField.Add(bonus);
        }

        public void CaptureBonus(BonusModel model)
        {
            var bonus = _bonusesOnField
                .Where(b => b.Model == model)
                .FirstOrDefault();
            if (bonus != null)
            {
                _bonusesOnField.Remove(bonus);
                bonus.HandleCapture();
                _alreadyGeneratedCount--;
            }
        }

        public bool TryCaptureBonus(Vector3 ballPosition, out BonusModel bonusModel)
        {
            bonusModel = default;
            var bonusRadius = _settings.BonusDiameter / 2;
            if (ballPosition.y < -bonusRadius || ballPosition.y > bonusRadius)
                return false;
            foreach (var bonus in _bonusesOnField)
            {
                if (bonus.CanBeCaptured && ballPosition.x >= bonus.PosX - bonusRadius && ballPosition.x <= bonus.PosX + bonusRadius)
                {
                    bonusModel = bonus.Model;
                    return true;
                }
            }
            return false;
        }

    }
}
