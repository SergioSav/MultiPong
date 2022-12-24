using Assets.Scripts.Data;
using Assets.Scripts.GameplayControllers;
using Assets.Scripts.HUD;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkControllers;
using Assets.Scripts.Services;
using Assets.Scripts.Utils;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        private GameModel _gameModel;
        private GameSettings _gameSettings;
        private RandomProvider _randomProvider;
        private IGameFactory _factory;
        private HUDManager _hudManager;
        private GameplayService _gameplayService;
        private BonusService _bonusService;
        private NetworkService _networkService;
        private MatchFieldController _matchField;
        private bool _isMatchStarted;

        private void Start()
        {
            _randomProvider = new RandomProvider((int)DateTimeOffset.Now.ToUnixTimeSeconds());

            GameSource.Instance.LoadSettings();
            GameSource.Instance.LoadBonuses();
            _gameSettings = GameSource.Instance.Settings;

            _gameModel = new GameModel(_gameSettings);
            _gameModel.GameStateChange += OnGameStateChange;
            _gameModel.ScoreChange += OnScoreChange;

            _matchField = GetComponentInChildren<MatchFieldController>();
            _matchField.Setup(_gameModel);
            _matchField.Hide();

            _bonusService = GetComponent<BonusService>();

            _factory = GetComponent<GameFactory>();

            _hudManager = GetComponentInChildren<HUDManager>();
            _hudManager.Setup(_gameModel, _gameSettings);

            _networkService = GetComponent<NetworkService>();
            _networkService.Setup(_gameSettings, _gameModel);

            _bonusService.Setup(_gameModel, _gameSettings, _factory, _randomProvider);

            _gameplayService = new GameplayService(_gameModel, _gameSettings, _factory);

            _gameModel.StartGame();

            DontDestroyOnLoad(this);
        }

        private void OnScoreChange()
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            _networkService.UpdateScoreServerRpc(_gameModel.PlayerOneScore, _gameModel.PlayerTwoScore);
            if (_gameModel.HasWinner)
                _networkService.MatchEndServerRpc(_gameModel.IsHostPlayerWinner);
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
                case GameState.PlayersReady:
                    break;
                case GameState.Gameplay:
                    _matchField.Show();
                    if (NetworkManager.Singleton.IsServer)
                    {
                        PrepareField();
                        _bonusService.StartGenerate();
                    }
                    _isMatchStarted = true;
                    break;
                case GameState.EndGame:
                    _isMatchStarted = false;
                    if (NetworkManager.Singleton.IsServer)
                    {
                        ClearField();
                        _bonusService.StopGenerate();
                        _bonusService.Clear();
                        _networkService.StopMatch();
                    }
                    _matchField.Hide();
                    break;
            }
        }

        private void PrepareField()
        {
            foreach (var player in _gameModel.Players)
            {
                _factory.SpawnBoard(player.NetId);
            }

            _factory.SpawnBall(
                new BallContext { 
                    Settings = _gameSettings, 
                    GameplayService = _gameplayService, 
                    RandomProvider = _randomProvider 
                });

            var matchData = new MatchData
            {
                SpawnedBall = _factory.SpawnedBall,
                PlayerOneBoard = _factory.GetBoard(_gameModel.Players[0].NetId),
                PlayerTwoBoard = _factory.GetBoard(_gameModel.Players[1].NetId)
            };
            _gameModel.SetMatchData(matchData);
        }

        private void ClearField()
        {
            _factory.DespawnBoards();
            _factory.DespawnBall();
            _factory.DespawnAllOtherObjects();
        }
        
        private void Update()
        {
            if (_isMatchStarted && NetworkManager.Singleton.IsServer)
            {
                _bonusService.CheckBonusCapture();
                _gameplayService.CheckRoundEnd();
            }
        }
    }
}
