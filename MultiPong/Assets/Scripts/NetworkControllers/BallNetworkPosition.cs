using Unity.Netcode;
using UnityEngine;

public struct BallNetworkPosition : INetworkSerializable
{
    private float _xPos;
    private float _yPos;

    public Vector3 Position
    {
        get => new Vector3(_xPos, _yPos);
        set {
            _yPos = value.y;
            _xPos = value.x;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _xPos);
        serializer.SerializeValue(ref _yPos);
    }
}
