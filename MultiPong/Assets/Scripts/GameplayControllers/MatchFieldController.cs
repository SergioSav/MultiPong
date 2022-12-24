using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.GameplayControllers
{
    public class MatchFieldController : MonoBehaviour
    {
        [SerializeField] private GameObject _topBorder;

        private GameModel _gameModel;

        public void Show()
        {
            gameObject.SetActive(true);
            _topBorder.SetActive(_gameModel.IsPractice);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Setup(GameModel gameModel)
        {
            _gameModel = gameModel;
        }
    }
}
