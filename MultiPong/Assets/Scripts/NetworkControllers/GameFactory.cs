using Unity.Netcode;

namespace Assets.Scripts.NetworkControllers
{
    public class GameFactory : NetworkBehaviour, IGameFactory
    {

        public NetworkObject SpawnBoard(NetworkObject boardPrototype)
        {
            var board = Instantiate(boardPrototype);
            board.Spawn();
            return board;
        }

        public NetworkObject SpawnBall(NetworkObject ballPrototype)
        {
            var ball = Instantiate(ballPrototype);
            ball.Spawn();
            return ball;
        }
    }
}
