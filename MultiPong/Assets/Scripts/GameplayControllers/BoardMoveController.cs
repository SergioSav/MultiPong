using Assets.Scripts.Data;
using System;
using System.Collections;
using UnityEngine;

public class BoardMoveController : MonoBehaviour, IPosition
{
    private float _currentSpeed;
    private float _halfBoardWidth;
    private GameSettings _settings;
    private float _boardMoveMaxSpeed;
    private float _boardVelocity;
    private float _changeSizeEffectDuration;
    private IEnumerator _resizeCoroutine;

    public float PosX => transform.position.x;
    public float PosY => transform.position.y;

    public void Setup(GameSettings settings)
    {
        _settings = settings;
        _boardMoveMaxSpeed = settings.BoardMaxSpeed;
        _boardVelocity = settings.BoardVelocity;
        _halfBoardWidth = settings.BoardWidth / 2;
    }

    public void ChangeSize(float multiplier, float duration)
    {
        if (_resizeCoroutine != null)
            StopCoroutine(_resizeCoroutine);

        _halfBoardWidth = _settings.BoardWidth / 2 * multiplier;
        _changeSizeEffectDuration = duration;

        _resizeCoroutine = ResizeTimeout();
        StartCoroutine(_resizeCoroutine);
    }

    private IEnumerator ResizeTimeout()
    {
        yield return new WaitForSeconds(_changeSizeEffectDuration);
        _halfBoardWidth = _settings.BoardWidth / 2;
    }

    private void Update()
    {
        transform.localScale = new Vector3(_halfBoardWidth * 2, _settings.BoardHeight, 1f);

        var mousePosX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        var clampedMousePosX = Mathf.Clamp(mousePosX, -8 + _halfBoardWidth, 8 - _halfBoardWidth);

        _currentSpeed = Mathf.Clamp(_currentSpeed + _boardVelocity * Time.deltaTime, 0, _boardMoveMaxSpeed);
        if (transform.position.x == clampedMousePosX)
            _currentSpeed = 0;

        transform.position = Vector3.Lerp(transform.position, new Vector3(clampedMousePosX, transform.position.y), 
                Mathf.Clamp01(_currentSpeed / _boardMoveMaxSpeed));
    }
}
