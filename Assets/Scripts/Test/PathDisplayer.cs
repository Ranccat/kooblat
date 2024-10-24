using UnityEngine;
using System.Collections.Generic;

public class PathDisplayer : MonoBehaviour
{
    [SerializeField] private float _lineWidth = .15f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = _lineRenderer.endWidth = _lineWidth;
    }

    public void DrawPath(List<Vector2> path)
    {
        _lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, path[i]);
        }
    }
}
