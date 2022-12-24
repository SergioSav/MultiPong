using Assets.Scripts.Data;
using UnityEngine;

public class BoardMoveController : MonoBehaviour
{
    private float _currentSpeed;
    private GameSettings _settings;
    private float _boardMoveMaxSpeed;
    private float _boardVelocity;
    private BoardNetworkController _netController;

    private void Start()
    {
        _settings = GameSource.Instance.Settings;
        _boardMoveMaxSpeed = _settings.BoardMaxSpeed;
        _boardVelocity = _settings.BoardVelocity;

        _netController = GetComponent<BoardNetworkController>();
    }

    private void Update()
    {
        transform.localScale = new Vector3(_netController.HalfWidth * 2, _settings.BoardHeight, 1f);

        var mousePosX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        var clampedMousePosX = Mathf.Clamp(mousePosX, 
            -_settings.FieldWidth * 0.5f + _netController.HalfWidth, 
            _settings.FieldWidth * 0.5f - _netController.HalfWidth);

        _currentSpeed = Mathf.Clamp(_currentSpeed + _boardVelocity * Time.deltaTime, 0, _boardMoveMaxSpeed);
        if (transform.position.x == clampedMousePosX)
            _currentSpeed = 0;

        transform.position = Vector3.Lerp(transform.position, new Vector3(clampedMousePosX, transform.position.y), 
                Mathf.Clamp01(_currentSpeed / _boardMoveMaxSpeed));
    }
}
