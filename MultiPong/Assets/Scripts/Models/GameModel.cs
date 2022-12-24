using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models
{
    public class GameModel
    {
        public event Action<GameState> GameStateChange;
        public event Action ScoreChange;

        private GameState _state;
        private string _connectionIpAddress;
        private bool _isWinner;
        private bool _isPractice;
        private bool _isClient;
        private int _winScore;

        private Dictionary<ulong, PlayerModel> _players;
        private MatchData _matchData;
        private int _playerOneScore;
        private int _playerTwoScore;

        public List<PlayerModel> Players => _players.Values.ToList();
        public MatchData MatchData => _matchData;

        public GameModel(GameSettings settings)
        {
            _winScore = settings.WinScore;

            _players = new Dictionary<ulong, PlayerModel>();
        }

        public void StartGame()
        {
            SwitchToMainMenuMode();
        }

        public void StartMatchmaking()
        {
            _state = GameState.Matchmaking;
            CallChangeEvent();
        }

        public void ConnectTo(string ipAddress)
        {
            _connectionIpAddress = string.IsNullOrEmpty(ipAddress) ? NativeNetTools.LocalHost : ipAddress;
            _isClient = true;
            _state = GameState.Matchmaking;
            CallChangeEvent();
        }

        public void RegisterPlayer(ulong playerId)
        {
            var player = new PlayerModel
            {
                NetId = playerId
            };
            _players[playerId] = player;
        }

        public void AllPlayersReady()
        {
            _state = GameState.PlayersReady;
            CallChangeEvent();
        }

        public void SetMatchData(MatchData matchData)
        {
            _matchData = matchData;
        }

        public void StartMatch()
        {
            _state = GameState.Gameplay;
            CallChangeEvent();
        }

        public void StartPractice()
        {
            _state = GameState.Gameplay;
            _isPractice = true;
            CallChangeEvent();
        }

        public void EndMatch(bool isWinner)
        {
            _isWinner = isWinner;
            _state = GameState.EndGame;
            CallChangeEvent();
        }

        public void ReturnToMainMenu() 
        {
            SwitchToMainMenuMode();
        }

        public bool IsCurrentPlayerWinner => _isWinner;
        public bool IsHostPlayerWinner => _playerOneScore > _playerTwoScore && _playerOneScore >= _winScore;
        public bool HasWinner => _playerOneScore >= _winScore || _playerTwoScore >= _winScore;
        public int PlayerOneScore => _playerOneScore;
        public int PlayerTwoScore => _playerTwoScore;
        public bool IsPractice => _isPractice;
        public bool IsClient => _isClient;
        public string IpAddress => _connectionIpAddress;
        public int RegisteredPlayerCount => _players.Count;

        public void AddScore()
        {
            if (MatchData.SpawnedBall.HasPositiveDirection)
                _playerOneScore++;
            else
                _playerTwoScore++;
            ScoreChange?.Invoke();
        }

        public void UpdateScores(int playerOneScore, int playerTwoScore)
        {
            _playerOneScore = playerOneScore;
            _playerTwoScore = playerTwoScore;
            ScoreChange?.Invoke();
        }

        public void ApplyChangeSizeEffect(float multiplier, float duration)
        {
            if (multiplier >= 1)
            {
                if (MatchData.SpawnedBall.HasPositiveDirection)
                    MatchData.PlayerOneBoard.ChangeSize(multiplier, duration);
                else
                    MatchData.PlayerTwoBoard.ChangeSize(multiplier, duration);
            }
            else
            {
                if (MatchData.SpawnedBall.HasPositiveDirection)
                    MatchData.PlayerTwoBoard.ChangeSize(multiplier, duration);
                else
                    MatchData.PlayerOneBoard.ChangeSize(multiplier, duration);
            }
        }

        public void ApplySpeedUpEffect(float multiplier, float duration)
        {
            MatchData.SpawnedBall.SpeedUp(multiplier, duration);
        }

        private void SwitchToMainMenuMode()
        {
            Reset();
            _state = GameState.MainMenu;
            CallChangeEvent();
        }

        private void CallChangeEvent()
        {
            GameStateChange?.Invoke(_state);
        }

        private void Reset()
        {
            _isPractice = false;
            _isClient = false;
            _playerOneScore = 0;
            _playerTwoScore = 0;
            _connectionIpAddress = NativeNetTools.LocalHost;
            _players.Clear();
            _matchData = null;
        }
    }
}
