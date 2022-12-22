using Unity.Netcode;

namespace Assets.Scripts.NetworkControllers
{
    public interface IGameFactory
    {
        NetworkObject SpawnBoard(NetworkObject boardPrototype);
        NetworkObject SpawnBall(NetworkObject ballPrototype);
    }
}