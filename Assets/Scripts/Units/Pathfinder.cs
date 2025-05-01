using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleWargame.Units
{
    /// <summary>
    /// This class handles finding a  path between 2 points in a list of coordinates
    /// </summary>
    public class Pathfinder : MonoBehaviour
    {
        private class Node
        {
            public Vector3 position;
            public float G;
            public float H;
            public float F => G + H;
            public List<Node> neighbors;
            public Node connection;
        }

        public static Pathfinder Instance { get; private set; }

        private Vector3[] neighbouringNodesOffset;

        private List<Node> nodes;
        private List<Node> toSearch;
        private List<Node> processed;
        private List<Vector3> pathReverted;
        private List<Vector3> path;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            nodes = new List<Node>();
            toSearch = new List<Node>();
            processed = new List<Node>();
            pathReverted = new List<Vector3>();
            path = new List<Vector3>();
            SetNeighbouringNodesOffset();
        }

        public List<Vector3> GetPath(Dictionary<Vector3, int> pathPointsWithExtraCost, Vector3 start, Vector3 target)
        {
            ClearAllLists();

            GetPathReverted(pathPointsWithExtraCost, start, target);
            for (int i = pathReverted.Count - 1; i >= 0; i--)
            {
                path.Add(pathReverted[i]);
            }
            return path;
        }

        private List<Vector3> GetPathReverted(Dictionary<Vector3, int> pathPointsWithExtraCost, Vector3 start, Vector3 target)
        {
            nodes.Add(new Node
            {
                position = start,
                G = Mathf.Round(Vector3.Distance(start, start) * 100),
                H = Mathf.Round(Vector3.Distance(target, start) * 100),
                neighbors = new List<Node>(),
                connection = null
            });
            toSearch.Add(nodes[0]);

            nodes.Add(new Node
            {
                position = target,
                G = Mathf.Round(Vector3.Distance(start, target) * 100),
                H = Mathf.Round(Vector3.Distance(target, target) * 100),
                neighbors = new List<Node>(),
                connection = null
            });
            foreach (Vector3 pathPoint in pathPointsWithExtraCost.Keys)
            {
                if (nodes.Exists(x => x.position.Equals(pathPoint))) continue;
                nodes.Add(new Node
                {
                    position = pathPoint,
                    G = Mathf.Round(Vector3.Distance(start, pathPoint) * 100) + (pathPointsWithExtraCost[pathPoint] * 100),
                    H = Mathf.Round(Vector3.Distance(target, pathPoint) * 100) + (pathPointsWithExtraCost[pathPoint] * 100),
                    neighbors = new List<Node>(),
                    connection = null
                });
            }

            foreach (Node node in nodes)
            {
                foreach (Vector3 neghbouringNodeOffset in neighbouringNodesOffset)
                {
                    if (nodes.Exists(x => x.position == neghbouringNodeOffset + node.position))
                        node.neighbors.Add(nodes.Find(x => x.position == neghbouringNodeOffset + node.position));
                }
            }

            //toSearch.Add(nodes.Find(x => x.position.Equals(start)));
            while (toSearch.Any())
            {
                Node current = toSearch[0];
                foreach (Node nodeToSearch in toSearch)
                    if (nodeToSearch.F < current.F || nodeToSearch.F == current.F && nodeToSearch.H < current.H) current = nodeToSearch;

                processed.Add(current);
                toSearch.Remove(current);

                if (current.position == target)
                {
                    Node currentPathTile = nodes.Find(x => x.position == target);
                    int count = 100;
                    while (currentPathTile.position != start)
                    {
                        pathReverted.Add(currentPathTile.position);
                        currentPathTile = currentPathTile.connection;
                        count--;
                        if (count < 0)
                        {
                            Debug.LogError("Pathfinding Error: Can't find path to target");
                            return null;
                        }
                    }

                    return pathReverted;
                }

                foreach (Node neighbor in current.neighbors.Where(t => !processed.Contains(t)))
                {
                    bool inSearch = toSearch.Contains(neighbor);

                    float costToNeighbor = current.G + Vector3.Distance(current.position, neighbor.position);

                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.G = costToNeighbor;
                        neighbor.connection = current;

                        if (!inSearch)
                        {
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }

        private void ClearAllLists()
        {
            path.Clear();
            path.TrimExcess();
            nodes.Clear();
            nodes.TrimExcess();
            toSearch.Clear();
            toSearch.TrimExcess();
            processed.Clear();
            processed.TrimExcess();
            pathReverted.Clear();
            pathReverted.TrimExcess();
        }

        private void SetNeighbouringNodesOffset()
        {
            neighbouringNodesOffset = new Vector3[8];

            neighbouringNodesOffset[0].y++;

            neighbouringNodesOffset[1].x++;
            neighbouringNodesOffset[1].y++;

            neighbouringNodesOffset[2].x++;

            neighbouringNodesOffset[3].x++;
            neighbouringNodesOffset[3].y--;

            neighbouringNodesOffset[4].y--;

            neighbouringNodesOffset[5].x--;
            neighbouringNodesOffset[5].y--;

            neighbouringNodesOffset[6].x--;

            neighbouringNodesOffset[7].x--;
            neighbouringNodesOffset[7].y++;
        }
    }
}
