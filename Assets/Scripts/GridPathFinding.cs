using System.Collections.Generic;
using UnityEngine;

public class GridPathfinding : MonoBehaviour
{
    public Grid grid;
    public LayerMask obstacleMask;   // define which layers are "blocked"
    public Vector2Int gridSize = new Vector2Int(20, 20);
    public float cellSize = 1f;

    public bool IsWalkable(Vector3Int cell)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(cell);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, obstacleMask);
        return hit == null; // walkable if no collider
    }

    public List<Vector3> FindPath(Vector3Int start, Vector3Int end)
    {
        var open = new List<Vector3Int>() { start };
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var cost = new Dictionary<Vector3Int, int>();
        cost[start] = 0;

        // Simple A* search
        while (open.Count > 0)
        {
            // Find lowest cost + heuristic
            Vector3Int current = open[0];
            foreach (var node in open)
            {
                if (cost[node] + Heuristic(node, end) < cost[current] + Heuristic(current, end))
                    current = node;
            }

            if (current == end)
                break;

            open.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!IsWalkable(neighbor)) continue;

                int newCost = cost[current] + 1;
                if (!cost.ContainsKey(neighbor) || newCost < cost[neighbor])
                {
                    cost[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    if (!open.Contains(neighbor)) open.Add(neighbor);
                }
            }
        }

        // Reconstruct path
        List<Vector3> path = new List<Vector3>();
        if (!cameFrom.ContainsKey(end)) return path; // no path

        Vector3Int c = end;
        while (c != start)
        {
            path.Insert(0, grid.GetCellCenterWorld(c));
            c = cameFrom[c];
        }
        return path;
    }

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        return new List<Vector3Int>()
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
    }

    int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }
}
