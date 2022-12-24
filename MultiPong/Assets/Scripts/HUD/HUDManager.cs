using Assets.Scripts.Data;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private EndGameView _endGameView;
        [SerializeField] private MatchmackingView _matchmakingView;

        [SerializeField] private GameplayHud _gameplayHud;

        public void Setup(GameModel gameModel, GameSettings settings)
        {
            _mainMenuView.Setup(gameModel);
            _matchmakingView.Setup(gameModel, settings);
            _endGameView.Setup(gameModel, settings);

            _gameplayHud.Setup(gameModel);

            gameModel.GameStateChange += OnGameStateChange;
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                    break;
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameState.Matchmaking:
                    ShowMatchmacking();
                    break;
                case GameState.PlayersReady:
                    _matchmakingView.HandlePlayersReady();
                    break;
                case GameState.Gameplay:
                    ShowMatch();
                    break;
                case GameState.EndGame:
                    ShowEndMatch();
                    break;
            }
        }

        public void ShowMatchmacking()
        {
            _mainMenuView.Hide();
            _matchmakingView.Show();
        }

        public void ShowMatch()
        {
            _mainMenuView.Hide();
            _matchmakingView.Hide();
            _gameplayHud.Show();
        }

        public void ShowEndMatch()
        {
            _gameplayHud.Hide();
            _endGameView.Show();
        }

        public void ShowMainMenu()
        {
            _mainMenuView.Show();
            _endGameView.Hide();
            _matchmakingView.Hide();
            _gameplayHud.Hide();
        }

        private void Start()
        {
            ShowMainMenu();
        }
    }
}
