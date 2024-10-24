using UnityEngine;
using System.Collections.Generic;

public class PathTest : MonoBehaviour
{
    [SerializeField] private Transform _startTr;
    [SerializeField] private Transform _targetTr;
    [SerializeField] private float _updateIntervalSeconds = .25f;

    private PathDisplayer _pathDisplayer;

    private float _lastUpdatedTime;

    private void Awake()
    {
        _pathDisplayer = GetComponent<PathDisplayer>();
    }

    private void Update()
    {
        if (Time.time - _lastUpdatedTime >= _updateIntervalSeconds)
        {
            _pathDisplayer.DrawPath(AStar.FindPath(_startTr.position, _targetTr.position));
            _lastUpdatedTime = Time.time;
        }
    }
}
