using Unity.Netcode;

public class BallNetworkController : NetworkBehaviour
{
    private NetworkVariable<BallNetworkPosition> _positionState = new NetworkVariable<BallNetworkPosition>(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        gameObject.AddComponent<BallMoveController>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            _positionState.Value = new BallNetworkPosition { Position = transform.position };
        }
        else
        {
            transform.position = _positionState.Value.Position;
        }
    }
}
