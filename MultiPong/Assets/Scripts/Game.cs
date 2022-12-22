using Assets.Scripts.Data;
using Assets.Scripts.HUD;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkControllers;
using Assets.Scripts.Utils;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        private const string _GAME_SETTINGS_URL = "GameSettingsScriptableObject";

        private const float _VERTICAL_PADDING = 0.1f;
        private const float _HORIZONTAL_PADDING = 0.1f;
        [SerializeField] private NetworkObject _boardPrototype;
        [SerializeField] private NetworkObject _ballPrototype;
        [SerializeField] private HUDManager _hudManager;
        [SerializeField] private BallMoveController _localBall;
        [SerializeField] private BoardMoveController _localBoard;
        [SerializeField] private BoardMoveController _localBonusPrototype;
        private GameSettings _gameSettings;
        private RandomProvider _randomProvider;
        private GameModel _gameModel;
        private IGameFactory _factory;

        private BoardMoveController _playerBoard;
        private MatchFieldView _matchField;
        private BallMoveController _spawnedBall;
        private bool _isMatchStarted;
        private float _halfWidthAccessibleField;
        private float _halfHeightAccessibleField;
        private float _halfHeightAccessibleFieldWithBoard;
        private BonusService _bonusService;
        private NetworkService _networkService;

        private void Start()
        {
            _randomProvider = new RandomProvider((int)DateTimeOffset.Now.ToUnixTimeSeconds());

            _gameSettings = Resources.Load<GameSettingsScriptableObject>(_GAME_SETTINGS_URL).GameSettings;

            _halfWidthAccessibleField = (_gameSettings.FieldWidth - _gameSettings.BallDiameter - _HORIZONTAL_PADDING) / 2;
            _halfHeightAccessibleField = (_gameSettings.FieldHeight - _gameSettings.BallDiameter - _VERTICAL_PADDING) / 2;
            _halfHeightAccessibleFieldWithBoard = (_gameSettings.FieldHeight - _gameSettings.BallDiameter - 
                   _VERTICAL_PADDING) / 2 - _gameSettings.BoardHeight - _gameSettings.BoardShiftPosY;

            _gameModel = new GameModel(_gameSettings);
            _gameModel.GameStateChange += OnGameStateChange;
            _gameModel.ScoreChange += OnScoreChange;

            _factory = GetComponent<GameFactory>();

            _hudManager = GetComponentInChildren<HUDManager>();
            _hudManager.Setup(_gameModel, _gameSettings);

            _matchField = GetComponentInChildren<MatchFieldView>();
            _matchField.Setup(_gameModel);
            _matchField.Hide();

            _bonusService = GetComponent<BonusService>();
            _bonusService.Setup(_gameSettings, _randomProvider);

            _networkService = GetComponent<NetworkService>();

            _gameModel.StartGame();

            DontDestroyOnLoad(this);
        }

        private void OnScoreChange()
        {
            if (_gameModel.HasWinner)
                _gameModel.EndMatch();
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                    break;
                case GameState.MainMenu:
                    break;
                case GameState.Matchmaking:
                    if (_gameModel.IsClient)
                        _networkService.StartMatchAsClient(_gameModel.IpAddress);
                    else
                        _networkService.StartMatchAsHost();
                    break;
                case GameState.Gameplay:
                    _isMatchStarted = true;
                    _matchField.Show();
                    _bonusService.StartGenerate();
                    break;
                case GameState.EndGame:
                    _isMatchStarted = false;
                    _matchField.Hide();
                    _bonusService.StopGenerate();
                    break;
            }
        }

        public bool TryGetObstacleNormalVector(Vector3 position, out Vector3 obstacleNormalVector)
        {
            obstacleNormalVector = Vector3.zero;

            if (position.x >= _halfWidthAccessibleField)
            {
                obstacleNormalVector = Vector3.left;
                return true;
            }
            if (position.x <= -_halfWidthAccessibleField)
            {
                obstacleNormalVector = Vector3.right;
                return true;
            }
            if (_gameModel.IsPractice && position.y >= _halfHeightAccessibleField)
            {
                obstacleNormalVector = Vector3.down;
                return true;
            }
            if (position.y >= _halfHeightAccessibleFieldWithBoard && HasBoardOnWay())
            {
                obstacleNormalVector = Vector3.down;
                return true;
            }
            if (position.y <= -_halfHeightAccessibleFieldWithBoard && HasBoardOnWay())
            {
                obstacleNormalVector = Vector3.up;
                return true;
            }
            return false;
        }

        private bool IsRoundEnd()
        {
            if (!_spawnedBall)
                return false;

            if (_spawnedBall.PosY >= _halfHeightAccessibleField)
            {
                if (_gameModel.IsPractice)
                    return false;
                else
                    return !HasBoardOnWay();
            }
            if (_spawnedBall.PosY <= -_halfHeightAccessibleField)
            {
                return !HasBoardOnWay();
            }
            return false;
        }

        private bool HasBoardOnWay()
        {
            if (_spawnedBall.PosX <= _playerBoard.PosX + 2 && _spawnedBall.PosX >= _playerBoard.PosX - 2)
                return true;
            return false;
        }

        private void SpawnBall()
        {
            //_factory.SpawnBall(_ballPrototype);
            _spawnedBall = Instantiate(_localBall, _matchField.transform);
            _spawnedBall.Setup(this, _gameSettings, _randomProvider);
        }

        private void SpawnBoard()
        {
            //_factory.SpawnBoard(_boardPrototype);
            var boardPosY = -(_halfHeightAccessibleField - (_gameSettings.BoardHeight - _gameSettings.BoardShiftPosY) / 2);
            _playerBoard = Instantiate(_localBoard, new Vector3(0, boardPosY), Quaternion.identity, _matchField.transform);
            _playerBoard.Setup(_gameSettings);
        }

        private void ResetBall()
        {
            _spawnedBall.ResetToInitialValues();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBall();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                SpawnBoard();
            }

            if (_spawnedBall != null && _bonusService.TryCaptureBonus(_spawnedBall.transform.position, out var bonusModel))
            {
                ApplyBonusEffect(bonusModel);
                _bonusService.CaptureBonus(bonusModel);
            }

            if (_isMatchStarted && IsRoundEnd())
            {
                ResetBall();
                _gameModel.AddScoreToOpponent();
                // restart round, respawn ball
            }
        }

        private void ApplyBonusEffect(BonusModel bonusModel)
        {
            switch (bonusModel.BonusType)
            {
                case BonusType.None:
                    break;
                case BonusType.Shrink:
                    _playerBoard.ChangeSize(1/bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
                case BonusType.Stretch:
                    _playerBoard.ChangeSize(bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
                case BonusType.SpeedUp:
                    _spawnedBall.SpeedUp(bonusModel.EffectValue, bonusModel.EffectDuration);
                    break;
            }
        }
    }
}
