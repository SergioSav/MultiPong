using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.NetworkControllers
{
    public interface IGameFactory
    {
        BallMoveController SpawnedBall { get; }
        BoardNetworkController GetBoard(ulong playerId);

        void SpawnBonus(Vector3 position, BonusModel bonusModel);
        void DespawnBonus(ulong bonusNetId);
        void SpawnBoard(ulong clientId);
        void DespawnBoards();
        void SpawnBall(BallContext context);
        void DespawnBall();
        void DespawnAllOtherObjects();
    }
}