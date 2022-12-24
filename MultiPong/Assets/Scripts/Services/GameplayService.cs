using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkControllers;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class GameplayService
    {
        private const float _VERTICAL_PADDING = 0.1f;
        private const float _HORIZONTAL_PADDING = 0.1f;

        private readonly GameModel _gameModel;
        private readonly GameSettings _gameSettings;
        private readonly IGameFactory _gameFactory;
        private float _halfWidthAccessibleField;
        private float _halfHeightAccessibleField;
        private float _halfHeightAccessibleFieldWithBoard;

        public GameplayService(GameModel gameModel, GameSettings gameSettings, IGameFactory gameFactory)
        {
            _gameModel = gameModel;
            _gameSettings = gameSettings;
            _gameFactory = gameFactory;

            _halfWidthAccessibleField = (_gameSettings.FieldWidth - _gameSettings.BallDiameter - _HORIZONTAL_PADDING) / 2;
            _halfHeightAccessibleField = (_gameSettings.FieldHeight - _gameSettings.BallDiameter - _VERTICAL_PADDING) / 2;
            _halfHeightAccessibleFieldWithBoard = (_gameSettings.FieldHeight - _gameSettings.BallDiameter -
            _VERTICAL_PADDING) / 2 - _gameSettings.BoardHeight - _gameSettings.BoardShiftPosY;
        }

        public bool TryGetObstacleNormalVector(Vector3 position, out Vector3 obstacleNormalVector)
        {
            obstacleNormalVector = Vector3.zero;

            if (position.x >= _halfWidthAccessibleField)
            {
                obstacleNormalVector = Vector3.left;
                return true;
            }
            if (position.x <= -_halfWidthAccessibleField)
            {
                obstacleNormalVector = Vector3.right;
                return true;
            }
            if (_gameModel.IsPractice && position.y >= _halfHeightAccessibleField)
            {
                obstacleNormalVector = Vector3.down;
                return true;
            }
            if (position.y >= _halfHeightAccessibleFieldWithBoard && HasBoardOnWay(_gameModel.MatchData.PlayerTwoBoard))
            {
                obstacleNormalVector = Vector3.down;
                return true;
            }
            if (position.y <= -_halfHeightAccessibleFieldWithBoard && HasBoardOnWay(_gameModel.MatchData.PlayerOneBoard))
            {
                obstacleNormalVector = Vector3.up;
                return true;
            }
            return false;
        }

        public void CheckRoundEnd()
        {
            if (IsRoundEnd(_gameModel.MatchData.SpawnedBall))
            {
                _gameModel.AddScore();
                _gameFactory.DespawnAllOtherObjects();
                _gameModel.MatchData.SpawnedBall.ResetToInitialValues();
            }
        }

        private bool IsRoundEnd(BallMoveController ball)
        {
            if (!ball)
                return false;

            if (ball.PosY >= _halfHeightAccessibleField)
            {
                if (_gameModel.IsPractice)
                    return false;
                else
                    return !HasBoardOnWay(_gameModel.MatchData.PlayerTwoBoard);
            }
            if (ball.PosY <= -_halfHeightAccessibleField)
            {
                return !HasBoardOnWay(_gameModel.MatchData.PlayerOneBoard);
            }
            return false;
        }

        private bool HasBoardOnWay(BoardNetworkController board)
        {
            return _gameModel.MatchData.SpawnedBall.PosX <= board.PosX + board.HalfWidth &&
                _gameModel.MatchData.SpawnedBall.PosX >= board.PosX - board.HalfWidth;
        }
    }
}
