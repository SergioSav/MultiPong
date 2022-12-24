using Assets.Scripts.Data;
using Assets.Scripts.Models;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class MatchmackingView : MonoBehaviour, IHudElement
    {
        [SerializeField] private GameObject _searchingPlayersPanel;
        [SerializeField] private GameObject _matchStartPanel;
        [SerializeField] private TextMeshProUGUI _matchStartTimerText;

        private int _countdownCounter;
        private GameModel _gameModel;
        private int _matchStartTimeout;

        public void Setup(GameModel gameModel, GameSettings settings)
        {
            _gameModel = gameModel;
            _matchStartTimeout = settings.MatchStartTimeout;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _matchStartPanel.SetActive(false);
            _searchingPlayersPanel.SetActive(true);
        }

        public void HandlePlayersReady()
        {
            OnMatchmakingDone();
        }

        private void OnMatchmakingDone()
        {
            _matchStartPanel.SetActive(true);
            _searchingPlayersPanel.SetActive(false);

            _countdownCounter = _matchStartTimeout;
            UpdateView();
            StartCoroutine(CountdownTimer());
        }

        private IEnumerator CountdownTimer()
        {
            while (_countdownCounter > 0)
            {
                yield return new WaitForSeconds(1);
                _countdownCounter--;
                UpdateView();
            }
            OnMatchStartDone();
        }

        private void OnMatchStartDone()
        {
            _gameModel.StartMatch();
        }

        private void UpdateView()
        {
            _matchStartTimerText.text = _countdownCounter.ToString();
        }
    }
}
