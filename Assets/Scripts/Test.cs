using System.Collections.Generic;
using OctanGames;
using OctanGames.Map;
using UnityEngine;
using UnityExtensions.Input;
using UnityExtensions.Math;

public class Test : MonoBehaviour
{
    private Pathfinding _pathfinding;

    private void Start()
    {
        _pathfinding = new Pathfinding(10, 10);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Find");
            Vector3 mouseWorldPosition = InputExtensions.GetMouseWorldPosition();
            float cellSize = _pathfinding.CellsGrid.CellSize;
            _pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = _pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (var i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine((path[i].Position.ToVector3() + Vector3.one * 0.5f) * cellSize,
                        (path[i + 1].Position.ToVector3()+ Vector3.one * 0.5f) * cellSize , Color.green, 100f);
                }
            }
        }
    }
}