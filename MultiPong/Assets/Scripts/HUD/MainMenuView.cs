using Assets.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HUD
{
    public class MainMenuView : MonoBehaviour, IHudElement
    {
        [SerializeField] private Button _startMatchButton;
        [SerializeField] private Button _joinMatchButton;
        [SerializeField] private TMP_InputField _joinMatchIpAddress;
        [SerializeField] private Button _startPracticeButton;

        private GameModel _gameModel;

        public void Setup(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        private void Start()
        {
            _startMatchButton.onClick.AddListener(OnStartMatchAsHostClick);
            _joinMatchButton.onClick.AddListener(OnStartMatchAsClientClick);
            _startPracticeButton.onClick.AddListener(OnStartPracticeClick);
        }

        private void OnStartPracticeClick()
        {
            _gameModel.StartPractice();
        }

        private void OnStartMatchAsHostClick()
        {
            _gameModel.StartMatchmaking();
        }

        private void OnStartMatchAsClientClick()
        {
            _gameModel.ConnectTo(_joinMatchIpAddress.text);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
