using Unity.Netcode;
using UnityEngine;

public struct BoardNetworkPosition : INetworkSerializable
{
    private float _xPos;

    public Vector3 Position 
    { 
        get => new Vector3(_xPos, 0, 0);
        set => _xPos = value.x;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _xPos);
    }
}
