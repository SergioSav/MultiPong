using Assets.Scripts.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class GameModel
    {
        public event Action<GameState> GameStateChange;
        public event Action ScoreChange;

        private string _connectionIpAddress;
        private GameState _state;
        private bool _isPractice;
        private int _opponentScore;
        private int _playerScore;
        private int _winScore;

        private Dictionary<int, PlayerModel> _players;
        private PlayerModel _attackingPlayer;

        public GameModel(GameSettings settings)
        {
            _winScore = settings.WinScore;

            _players = new Dictionary<int, PlayerModel>();
        }

        public void RegisterPlayer(PlayerModel player)
        {
            _players[player.Id] = player;
        }

        public void SetLastTouchPlayer(int id)
        {
            _attackingPlayer = _players[id];
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
            _connectionIpAddress = ipAddress;
            _state = GameState.Matchmaking;
            CallChangeEvent();
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

        public void EndMatch()
        {
            _state = GameState.EndGame;
            CallChangeEvent();
        }

        private void Reset()
        {
            _isPractice = false;
            _playerScore = 0;
            _opponentScore = 0;
        }

        public void ReturnToMainMenu() 
        {
            SwitchToMainMenuMode();
        }
        
        public bool IsCurrentPlayerWinner => _playerScore > _opponentScore && _playerScore >= _winScore;
        public bool HasWinner => _playerScore >= _winScore || _opponentScore >= _winScore;
        public int PlayerScore => _playerScore;
        public int OpponentScore => _opponentScore;
        public bool IsPractice => _isPractice;
        public bool IsClient => !string.IsNullOrEmpty(_connectionIpAddress);
        public string IpAddress => _connectionIpAddress;

        public void AddScoreToPlayer()
        {
            _playerScore++;
            ScoreChange?.Invoke();
        }

        public void AddScoreToOpponent()
        {
            _opponentScore++;
            ScoreChange?.Invoke();
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
    }

    public class PlayerModel
    {
        public int Id;
        public string Name;
        public int Score;
    }
}
