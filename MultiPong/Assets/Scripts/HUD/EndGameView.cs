using Assets.Scripts.Data;
using Assets.Scripts.Models;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HUD
{
    public class EndGameView : MonoBehaviour, IHudElement
    {
        [SerializeField] private TextMeshProUGUI _textMatchResult;
        [SerializeField] private GameObject _buttonReturnContainer;
        [SerializeField] private Button _buttonReturn;

        private GameModel _gameModel;
        private int _buttonReturnTimeout;
        private string _winText;
        private string _loseText;

        public void Setup(GameModel gameModel, GameSettings settings)
        {
            _gameModel = gameModel;
            _buttonReturnTimeout = settings.ButtonReturnShowTimeout;
            _winText = settings.WinText;
            _loseText = settings.LoseText;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _buttonReturnContainer.SetActive(false);

            _textMatchResult.text = _gameModel.IsCurrentPlayerWinner ? _winText : _loseText;

            StartCoroutine(TimeoutForReturnShow());
        }

        private IEnumerator TimeoutForReturnShow()
        {
            yield return new WaitForSeconds(_buttonReturnTimeout);
            _buttonReturnContainer.SetActive(true);
        }

        private void Start()
        {
            _buttonReturn.onClick.AddListener(OnReturnClick);
        }

        private void OnReturnClick()
        {
            _gameModel.ReturnToMainMenu();
        }
    }
}
