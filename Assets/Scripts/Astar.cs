using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Vector2Int> posList = new List<Vector2Int>();

        int startEndGHCost = Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.y - endPos.y);

        Node startNode = new Node(startPos, null, 0, startEndGHCost);
        Node targetNode = new Node(endPos, null, startEndGHCost, 0);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                //add the next node to the list only if it's f score is smaller than the current node, or if it's f score is equal but h score is lower
                if (openList[i].FScore < currentNode.FScore || (openList[i].FScore == currentNode.FScore && openList[i].HScore < currentNode.HScore))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                posList = GetFinalPath(startNode, targetNode);
            }

            foreach (Cell neighbour in GetAvailableNeighbours(currentNode, grid, closedList))
            {
                Node neighbourNode = CellToNode(neighbour);

                if (neighbourNode.position == targetNode.position)
                {
                    targetNode.parent = currentNode;
                }

                float newGScore = currentNode.GScore + CalculateDistanceCost(currentNode, neighbourNode);

                if (newGScore < neighbourNode.GScore)
                {
                    neighbourNode.parent = currentNode;
                    neighbourNode.GScore = newGScore;
                    neighbourNode.HScore = CalculateDistanceCost(neighbourNode, targetNode);

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return posList;
    }

    private List<Vector2Int> GetFinalPath(Node _startNode, Node _endNode)
    {
        List<Vector2Int> finalPath = new List<Vector2Int>();
        Node currentNode = _endNode;

        while (currentNode != _startNode)
        {
            finalPath.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        finalPath.Reverse();

        return finalPath;
    }

    private List<Cell> GetAvailableNeighbours(Node _node, Cell[,] _grid, HashSet<Node> _closed)
    {
        Cell cell = NodeToCell(_node, _grid);

        List<Cell> neighbours = new List<Cell>();

        if (!cell.HasWall(Wall.UP))
        {
            neighbours.Add(_grid[cell.gridPosition.x, cell.gridPosition.y + 1]);
        }

        if (!cell.HasWall(Wall.RIGHT))
        {
            neighbours.Add(_grid[cell.gridPosition.x + 1, cell.gridPosition.y]);
        }

        if (!cell.HasWall(Wall.DOWN))
        {
            neighbours.Add(_grid[cell.gridPosition.x, cell.gridPosition.y - 1]);
        }

        if (!cell.HasWall(Wall.LEFT))
        {
            neighbours.Add(_grid[cell.gridPosition.x - 1, cell.gridPosition.y]);
        }

        for (int i = 0; i < neighbours.Count;)
        {
            if (_closed.Any(x => x.position == neighbours[i].gridPosition))
            {
                neighbours.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        return neighbours;
    }

    private int CalculateDistanceCost(Node _a, Node _b)
    {
        int xDistance = Mathf.Abs(_a.position.x - _b.position.x);
        int yDistance = Mathf.Abs(_a.position.y - _b.position.y);

        return xDistance + yDistance;
    }

    private Node CellToNode(Cell _cell)
    {
        Node node = new Node();

        node.position = _cell.gridPosition;
        node.GScore = float.PositiveInfinity;

        return node;
    }

    private Cell NodeToCell(Node _node, Cell[,] _grid)
    {
        Cell cell = new Cell();

        cell.gridPosition = _node.position;
        cell.walls = _grid[cell.gridPosition.x, cell.gridPosition.y].walls;

        return cell;
    }
    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
