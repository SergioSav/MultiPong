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
            SetScore(_gameModel.PlayerScore, _gameModel.OpponentScore);
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

        private void Update()
        {
            // TODO: temp hack
            if (Input.GetKeyDown(KeyCode.E))
            {
                for (int i = 0; i < 10; i++)
                {
                    _gameModel.AddScoreToPlayer();
                }
                _gameModel.EndMatch();
            }
        }
    }
}
