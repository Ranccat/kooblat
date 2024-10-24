using UnityEngine;
using System.Collections.Generic;

public class ChasePlayer : MonoBehaviour
{
    public delegate void OnReachedEvent();

    private float _speed = 3f;
    private float _pathUpdateIntervalSeconds = .5f;
    private float _stoppingDistance = 1f;
    private Vector2 _offset;
    private OnReachedEvent _onStoppingDistanceReached;

    private Rigidbody2D _rb;

    private Transform _playerTr;
    private float _pathLastUpdatedTime;
    private List<Vector2> _path = new List<Vector2>();
    private int _pathIndex;

    private Vector2 _direction;
    public Vector2 GetDirection()
    {
        return _direction;
    }

    #region Setters

    public ChasePlayer SetSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    public ChasePlayer SetUpdateInterval(float updateInterval)
    {
        _pathUpdateIntervalSeconds = updateInterval;
        return this;
    }

    public ChasePlayer SetStoppingDistance(float stoppingDistance)
    {
        _stoppingDistance = stoppingDistance;
        return this;
    }

    public ChasePlayer SetOnDistanceReached(OnReachedEvent onReached)
    {
        _onStoppingDistanceReached = onReached;
        return this;
    }

    public ChasePlayer SetOffset(Vector2 offset)
    {
        _offset = offset;
        return this;
    }

    #endregion

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerTr = Game.Player.transform;
    }

    private void Update()
    {
        if (Time.time - _pathLastUpdatedTime >= _pathUpdateIntervalSeconds)
        {
            _path = AStar.FindPath(transform.position, _playerTr.position);
            _pathIndex = 0;

            _pathLastUpdatedTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, _playerTr.position) <= _stoppingDistance)
        {
            _onStoppingDistanceReached?.Invoke();
            return;
        }

        if (_path.Count == 0 || _pathIndex >= _path.Count)
            return;

        Vector2 nextPoint = _path[_pathIndex] + _offset;
        _direction = nextPoint - (Vector2)transform.position;
        float distanceToNextPoint = _direction.magnitude;
        float moveDistance = _speed * Time.fixedDeltaTime;
        
        if (distanceToNextPoint < moveDistance)
        {
            _rb.MovePosition(nextPoint);
            _pathIndex++;
        }
        else
        {
            _rb.MovePosition((Vector2)transform.position + _direction / distanceToNextPoint * moveDistance);
        }
    }
}
