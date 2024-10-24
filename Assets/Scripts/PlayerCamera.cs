using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform _playerTr;
    private float _zPos;

    private Maze _maze;

    private float _camHalfHeight;
    private float _camHalfWidth;

    public Vector2 TopLeftBounds => (Vector2)transform.position + Vector2.left * _camHalfWidth + Vector2.up * _camHalfHeight;
    public Vector2 BottomRightBounds => (Vector2)transform.position + Vector2.right * _camHalfWidth + Vector2.down * _camHalfHeight;

    private void Start()
    {
        _playerTr = Game.Player.transform;
        _zPos = transform.position.z;

        _maze = Game.Maze;

        Camera cam = Camera.main;
        _camHalfHeight = cam.orthographicSize;
        _camHalfWidth = cam.aspect * _camHalfHeight;
    }

    private void LateUpdate()
    {
        // not so smart to recalculate these bounds every single update
        Vector2 mazeTopLeftBounds = _maze.TopLeftBounds;
        Vector2 mazeBottomRightBounds = _maze.BottomRightBounds;

        float x = Mathf.Clamp(_playerTr.position.x, mazeTopLeftBounds.x + _camHalfWidth, mazeBottomRightBounds.x - _camHalfWidth);
        float y = Mathf.Clamp(_playerTr.position.y, mazeBottomRightBounds.y + _camHalfHeight, mazeTopLeftBounds.y - _camHalfHeight);

        transform.position = new Vector3(x, y, _zPos);
    }
}
