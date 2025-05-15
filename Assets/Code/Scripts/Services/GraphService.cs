using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Services
{
    /// <summary>
    /// Отвечает за обычный поиск пути по узлам и расчет стоимости пути
    /// </summary>
    public class GraphService
    {
        private readonly Node[] _nodes;

        public GraphService(Node[] nodes)
        {
            _nodes = nodes;
        }
        
        public Node GetRandomNode()
        {
            var randomNodeIndex = Random.Range(0, _nodes.Length);
            return _nodes[randomNodeIndex];
        }

        public List<Node> FindPath(Node start, Node target)
        {
            // алгоритм Дейкстры
            var previous = new Dictionary<Node, Node>();
            var distances = new Dictionary<Node, float>();
            var nodes = new List<Node>();

            List<Node> path = null;

            foreach (var node in _nodes)
            {
                if (node == start)
                {
                    distances[node] = 0;
                }
                else
                {
                    distances[node] = float.MaxValue;
                }

                nodes.Add(node);
            }

            while (nodes.Count > 0)
            {
                nodes.Sort((a, b) => distances[a].CompareTo(distances[b]));
                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == target)
                {
                    path = new List<Node>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    path.Reverse();
                    return path;
                }

                if (Mathf.Approximately(distances[smallest], float.MaxValue))
                {
                    break;
                }

                for (var i = 0; i < smallest.Edges.Length; i++)
                {
                    var neighbor = smallest.Edges[i].Node;
                    var alt = distances[smallest] + smallest.Edges[i].Length;
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = smallest;
                    }
                }
            }

            return path;
        }
        
        public float CalculatePathCost(Node start, List<Node> path, float speed)
        {
            if (path == null || path.Count == 0)
            {
                return float.MaxValue;
            }

            var totalCost = 0f;

            foreach (var node in path)
            {
                totalCost += start.GetDistanceTo(node) / speed;
                start = node;
            }

            return totalCost;
        }
    }
}