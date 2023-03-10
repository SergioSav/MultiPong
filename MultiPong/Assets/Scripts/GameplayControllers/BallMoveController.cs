using Assets.Scripts.Data;
using Assets.Scripts.Services;
using Assets.Scripts.Utils;
using System.Collections;
using UnityEngine;

public struct BallContext
{
    public GameSettings Settings;
    public GameplayService GameplayService;
    public RandomProvider RandomProvider;
}

public class BallMoveController : MonoBehaviour
{
    private Vector3 _cachedObstacleNormalVector;
    private Vector3 _direction;
    private float _currentSpeed;
    private float _maxSpeed;
    private float _velocity;

    private bool _needMove;
    private GameSettings _settings;
    private GameplayService _gameplayService;
    private RandomProvider _randomProvider;
    private float _speedUpEffectDuration;

    public float PosX => transform.position.x;
    public float PosY => transform.position.y;

    public bool HasPositiveDirection => _direction.y > 0;

    public void Setup(BallContext context)
    {
        _settings = context.Settings;
        _gameplayService = context.GameplayService;
        _randomProvider = context.RandomProvider;
    }
    public void ResetToInitialValues()
    {
        transform.position = Vector3.zero;
        _cachedObstacleNormalVector = Vector3.zero;

        _randomProvider.GetRandom(-1f, 1f, out var randomX);
        _randomProvider.GetRandom(-1f, 1f, out var randomY);
        while (randomY <= 0.001f && randomY >= 0.001f) // except 0
            _randomProvider.GetRandom(-1f, 1f, out randomY);

        _direction = new Vector3(randomX, randomY);
        _currentSpeed = 0f;
        _maxSpeed = _settings.BallMaxSpeed;
        _velocity = _settings.BallVelocity;

        StartCoroutine(AppearEffects());
    }

    private void Start()
    {
        ResetToInitialValues();
    }

    private IEnumerator AppearEffects()
    {
        yield return new WaitForSeconds(0.5f);
        _needMove = true;
    }

    private void Update()
    {
        if (!_needMove)
            return;

        _currentSpeed = Mathf.Clamp(_currentSpeed + _velocity * Time.deltaTime, 0, _maxSpeed);
        transform.position += _direction * _currentSpeed;
        UpdateDirection();
    }

    private void UpdateDirection()
    {
        if (_gameplayService.TryGetObstacleNormalVector(transform.position, out var obstacleNormalVector))
        {
            if (obstacleNormalVector == _cachedObstacleNormalVector)
                return;
            _cachedObstacleNormalVector = obstacleNormalVector;
            _direction = Vector3.Reflect(_direction, obstacleNormalVector);
        }
    }

    public void SpeedUp(float multiplier, float duration)
    {
        StopCoroutine(SpeedUpTimeout());
        _velocity *= multiplier;
        _speedUpEffectDuration = duration;
        StartCoroutine(SpeedUpTimeout());
    }

    private IEnumerator SpeedUpTimeout()
    {
        yield return new WaitForSeconds(_speedUpEffectDuration);
        _velocity = _settings.BallVelocity;
    }
}
