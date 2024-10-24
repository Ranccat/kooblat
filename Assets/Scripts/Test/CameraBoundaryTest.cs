using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraBoundaryTest : MonoBehaviour
{
    [SerializeField] private int _rowDistanceFromPlayer = 5;
    [SerializeField] private int _colDistanceFromPlayer = 10;

    private Transform _playerTr;
    private Maze _maze;
    private List<Coords> _boundaryCoords;

    private Camera _cam;
    private float _camHalfHeight;
    private float _camHalfWidth;

    private PlayerCamera _playerCamera;

    private void Start()
    {
        _playerTr = Game.Player.transform;
        _maze = Game.Maze;
        _boundaryCoords = new List<Coords>();

        _cam = Camera.main;
        _camHalfHeight = _cam.orthographicSize;
        _camHalfWidth = _cam.aspect * _camHalfHeight;

        _playerCamera =_cam.gameObject.GetComponent<PlayerCamera>();
    }

    //private void Update()
    //{
    //    UpdateBoundaries();
    //}

    private void UpdateBoundaries()
    {
        List<Coords> newCoords = new List<Coords>();

        Coords playerCoords = _maze.GetCoords(_playerTr.position);

        int playerRow = playerCoords.Row;
        int playerCol = playerCoords.Col;

        int topRow = playerRow - _rowDistanceFromPlayer;
        int leftCol = playerCol - _colDistanceFromPlayer;

        int bottomRow = playerRow + _rowDistanceFromPlayer;
        int rightCol = playerCol + _colDistanceFromPlayer;

        int height = 2 * _rowDistanceFromPlayer + 1;
        int width = 2 * _colDistanceFromPlayer + 1;

        newCoords.AddRange(GetEmptyCoordsInRange(topRow, leftCol, 0, 1, width - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(topRow, rightCol, 1, 0, height - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(bottomRow, rightCol, 0, -1, width - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(bottomRow, leftCol, -1, 0, height - 1));

        _boundaryCoords = newCoords;
    }

    private void LateUpdate()
    {
        UpdateBoundaries1();    
    }

    [SerializeField] private float _margin = 1f;

    private void UpdateBoundaries1()
    {
        Coords camTopLeftCoords = _maze.GetCoords(_playerCamera.TopLeftBounds + new Vector2(-1, 1) * _margin);
        Coords camBottomRightCoords = _maze.GetCoords(_playerCamera.BottomRightBounds + new Vector2(1, -1) * _margin);

        int topRow = camTopLeftCoords.Row;
        int leftCol = camTopLeftCoords.Col;

        int bottomRow = camBottomRightCoords.Row;
        int rightCol = camBottomRightCoords.Col;

        int height = bottomRow - topRow + 1;
        int width = rightCol - leftCol + 1;

        List<Coords> newCoords = new List<Coords>();
        newCoords.AddRange(GetEmptyCoordsInRange(topRow, leftCol, 0, 1, width - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(topRow, rightCol, 1, 0, height - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(bottomRow, rightCol, 0, -1, width - 1));
        newCoords.AddRange(GetEmptyCoordsInRange(bottomRow, leftCol, -1, 0, height - 1));

        _boundaryCoords = newCoords;
    }

    private List<Coords> GetEmptyCoordsInRange(int startRow, int startCol, int dRow, int dCol, int length)
    {
        List<Coords> coords = new List<Coords>();

        int row = startRow;
        int col = startCol;
        for (int i = 0; i < length; i++)
        {
            if (!_maze.IsOutOfBounds(row, col) && _maze.GetBlockAt(row, col) == null)
                coords.Add(new Coords(row, col));
            row += dRow;
            col += dCol;
        }

        return coords;
    }

    private void OnDrawGizmos()
    {
        if (_boundaryCoords == null)
            return;

        Gizmos.color = new Color(1, 0, 0, .5f);
        foreach (Coords coords in _boundaryCoords)
        {
            Vector2 position = _maze.GetPosition(coords);
            Gizmos.DrawCube(position, Vector2.one);
        }
    }
}
