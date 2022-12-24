using Assets.Scripts.Models;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class GameplayHud : MonoBehaviour, IHudElement
    {
        [SerializeField] private TextMeshProUGUI _textScore;

        private GameModel _gameModel;

        public void Setup(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        private void Start()
        {
            _gameModel.ScoreChange += OnScoreChange;
        }

        private void OnScoreChange()
        {
            SetScore(_gameModel.PlayerOneScore, _gameModel.PlayerTwoScore);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetScore(0, 0);
        }

        private void SetScore(int playerOneScore, int playerTwoScore)
        {
            _textScore.text = $"{playerOneScore}:{playerTwoScore}";
        }
    }
}
