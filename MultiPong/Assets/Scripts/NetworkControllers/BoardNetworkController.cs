using Assets.Scripts.Data;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BoardNetworkController : NetworkBehaviour
{
    private NetworkVariable<float> _xPos = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _halfBoardWidthNet = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

    private float _yPos;
    private float _halfBoardWidth;
    private float _changeSizeEffectDuration;
    private IEnumerator _resizeCoroutine;

    public float HalfWidth => _halfBoardWidth;
    public float PosX => transform.position.x;
    public float PosY => transform.position.y;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            gameObject.AddComponent<BoardMoveController>();

        _halfBoardWidth = GameSource.Instance.Settings.BoardWidth / 2;
        _yPos = (IsOwnedByServer ? -1 : 1) * GameSource.Instance.InitialBoardPosY();
        transform.position = new Vector3(0, _yPos);
    }

    public void ChangeSize(float multiplier, float duration)
    {
        if (_resizeCoroutine != null)
            StopCoroutine(_resizeCoroutine);

        _halfBoardWidth = GameSource.Instance.Settings.BoardWidth / 2 * multiplier;
        _changeSizeEffectDuration = duration;

        _resizeCoroutine = ResizeTimeout();
        StartCoroutine(_resizeCoroutine);
    }

    private IEnumerator ResizeTimeout()
    {
        yield return new WaitForSeconds(_changeSizeEffectDuration);
        _halfBoardWidth = GameSource.Instance.Settings.BoardWidth / 2;
    }

    private void Update()
    {
        if (IsOwner)
        {
            _xPos.Value = transform.position.x;
            _halfBoardWidthNet.Value = _halfBoardWidth;
        }
        else
        {
            transform.position = new Vector3(_xPos.Value, _yPos, 0);
            transform.localScale = new Vector3(_halfBoardWidthNet.Value * 2, GameSource.Instance.Settings.BoardHeight, 1f);
        }
    }
}
