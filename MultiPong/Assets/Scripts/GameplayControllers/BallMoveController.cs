using Assets.Scripts;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using UnityEngine;

public class BallMoveController : MonoBehaviour, IPosition
{
    private Vector3 _direction;
    private float _currentSpeed;
    private float _maxSpeed;
    private float _velocity;

    private bool _needMove;
    private Game _game;
    private GameSettings _settings;
    private RandomProvider _randomProvider;
    private float _speedUpEffectDuration;

    public float PosX => transform.position.x;
    public float PosY => transform.position.y;

    public void Setup(Game game, GameSettings settings, RandomProvider randomProvider)
    {
        _game = game;
        _settings = settings;
        _randomProvider = randomProvider;
    }
    public void ResetToInitialValues()
    {
        transform.position = Vector3.zero;

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
        // TODO: update direction after collision
        if (_game.TryGetObstacleNormalVector(transform.position, out var obstacleNormalVector))
        {
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
