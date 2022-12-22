using Unity.Netcode;

public class BoardNetworkController : NetworkBehaviour
{
    private NetworkVariable<BoardNetworkPosition> _positionState = new NetworkVariable<BoardNetworkPosition>(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            gameObject.AddComponent<BoardMoveController>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            _positionState.Value = new BoardNetworkPosition { Position = transform.position };
        }
        else
        {
            transform.position = _positionState.Value.Position;
        }
    }
}
