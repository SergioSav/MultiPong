using Assets.Scripts.Models;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.NetworkControllers
{
    public class GameFactory : NetworkBehaviour, IGameFactory
    {
        [SerializeField] private BonusController _bonusPrototype;
        [SerializeField] private BoardNetworkController _boardPrototype;
        [SerializeField] private BallMoveController _ballPrototype;

        //[SerializeField] private BallMoveController _localBall;
        //[SerializeField] private BoardMoveController _localBoard;

        private Dictionary<ulong, NetworkObject> _spawnedObjects;
        private Dictionary<ulong, BoardNetworkController> _spawnedBoards;
        private BallMoveController _spawnedBall;

        public BallMoveController SpawnedBall => _spawnedBall;
        
        private void Start()
        {
            _spawnedObjects = new Dictionary<ulong, NetworkObject>();
            _spawnedBoards = new Dictionary<ulong, BoardNetworkController>();
        }

        public void SpawnBoard(ulong clientId)
        {
            if (!IsServer || _boardPrototype == null)
            {
                return;
            }
            var board = Instantiate(_boardPrototype);
            board.NetworkObject.SpawnAsPlayerObject(clientId);

            _spawnedBoards[clientId] = board;
        }

        public void SpawnBall(BallContext context)
        {
            if (!IsServer || _ballPrototype == null)
            {
                return;
            }
            var ball = Instantiate(_ballPrototype);
            ball.Setup(context);
            ball.GetComponent<NetworkObject>().Spawn();
            _spawnedBall = ball;
        }

        public void SpawnBonus(Vector3 position, BonusModel bonusModel)
        {
            if (!IsServer || _bonusPrototype == null)
            {
                return;
            }
            var bonus = Instantiate(_bonusPrototype, position, Quaternion.identity);
            bonus.Setup(bonusModel);

            var bonusNetworkObject = bonus.GetComponent<NetworkObject>();
            bonusNetworkObject.Spawn();

            _spawnedObjects[bonusNetworkObject.NetworkObjectId] = bonusNetworkObject;
        }

        public void DespawnBonus(ulong bonusNetId)
        {
            if (IsServer && _spawnedObjects.TryGetValue(bonusNetId, out var bonusNetObject))
            {
                bonusNetObject.Despawn();
                _spawnedObjects.Remove(bonusNetId);
            }
        }

        public BoardNetworkController GetBoard(ulong playerId)
        {
            return _spawnedBoards[playerId];
        }

        public void DespawnBoards()
        {
            foreach (var board in _spawnedBoards.Values)
            {
                board.NetworkObject.Despawn();
            }
            _spawnedBoards.Clear();
        }

        public void DespawnBall()
        {
            _spawnedBall.GetComponent<NetworkObject>().Despawn();
        }

        public void DespawnAllOtherObjects()
        {
            foreach (var obj in _spawnedObjects.Values)
            {
                obj.Despawn();
            }
            _spawnedObjects.Clear();
        }

        // FOR PRACTICE MODE / temporary broken
        //private void Update()
        //{
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SpawnBall();
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    SpawnBoard();
        //}
        //}

        //private void SpawnBall()
        //{
        //    _spawnedBall = Instantiate(_localBall, _matchField.transform);
        //    _spawnedBall.Setup(this, _gameSettings, _randomProvider);
        //}

        //private void SpawnBoard()
        //{
        //    var boardPosY = -(_halfHeightAccessibleField - (_gameSettings.BoardHeight - _gameSettings.BoardShiftPosY) / 2);
        //    _playerBoard = Instantiate(_localBoard, new Vector3(0, boardPosY), Quaternion.identity, _matchField.transform);
        //    _playerBoard.Setup(_gameSettings);
        //}
    }
}
