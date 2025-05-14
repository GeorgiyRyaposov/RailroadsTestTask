using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts
{
    /// <summary>
    /// Отвечает за поиск наилучшего пути
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Node[] _allNodes;
        [SerializeField] private Base[] _baseNodes;
        [SerializeField] private Mine[] _mineNodes;

        private void OnValidate()
        {
            _baseNodes = _allNodes.Where(x => x is Base).Cast<Base>().ToArray();
            _mineNodes = _allNodes.Where(x => x is Mine).Cast<Mine>().ToArray();
        }

        public Node GetRandomNode()
        {
            var randomNodeIndex = Random.Range(0, _allNodes.Length);
            return _allNodes[randomNodeIndex];
        }

        public List<Node> FindBestPathToMine(Node currentNode, Train train)
        {
            return FindBestDestination(currentNode, train, _mineNodes);
        }
        public List<Node> FindBestPathToBase(Node currentNode, Train train)
        {
            return FindBestDestination(currentNode, train, _baseNodes);
        }
        private List<Node> FindBestDestination(Node currentNode, Train train, IEnumerable<Node> possibleTargets)
        {
            Node bestTarget = null;
            List<Node> bestPath = null;
            var bestResourceEfficiency = 0f;

            foreach (var target in possibleTargets)
            {
                var path = FindPathTo(currentNode, target);
                if (path == null || path.Count == 0)
                {
                    continue;
                }

                var pathCost = CalculatePathCost(currentNode, path, train);

                // Рассчитываем эффективность (ресурс/время)
                var efficiency = 0f;
                if (target is Base baseNode)
                {
                    efficiency = baseNode.ResourceMultiplier / pathCost;
                }
                else if (target is Mine mineNode)
                {
                    efficiency = CalculateMiningEfficiency(currentNode, mineNode, train);
                }

                if (efficiency > bestResourceEfficiency)
                {
                    bestTarget = target;
                    bestPath = path;
                    bestResourceEfficiency = efficiency;
                }
            }

            if (bestTarget)
            {
                return bestPath;
            }
            
            Debug.LogWarning("Не удалось найти подходящий путь!");
            return null;
        }

        private float CalculateMiningEfficiency(Node start, Mine mine, Train train)
        {
            // Находим ближайшую базу от шахты
            var bases = _baseNodes;
            Base closestBase = null;
            var minBaseCost = float.MaxValue;

            foreach (var baseNode in bases)
            {
                var pathToBase = FindPathTo(mine, baseNode);
                var cost = CalculatePathCost(mine, pathToBase, train);
                if (cost < minBaseCost)
                {
                    minBaseCost = cost;
                    closestBase = baseNode;
                }
            }

            if (!closestBase)
            {
                return 0f;
            }

            // Общее время: путь до шахты + добыча + путь до базы
            var pathToMine = FindPathTo(start, mine);
            var timeToMine = CalculatePathCost(start, pathToMine, train);
            var miningTime = train.BaseMiningTime * mine.MiningTimeMultiplier;
            var totalTime = timeToMine + miningTime + minBaseCost;

            // Эффективность: ресурс с учетом множителя базы / общее время
            return closestBase.ResourceMultiplier / totalTime;
        }

        private List<Node> FindPathTo(Node start, Node target)
        {
            // алгоритм Дейкстры
            var previous = new Dictionary<Node, Node>();
            var distances = new Dictionary<Node, float>();
            var nodes = new List<Node>();

            List<Node> path = null;

            foreach (var node in _allNodes)
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

        private float CalculatePathCost(Node start, List<Node> path, Train train)
        {
            if (path == null || path.Count == 0)
            {
                return float.MaxValue;
            }

            var totalCost = 0f;

            foreach (var node in path)
            {
                totalCost += start.GetDistanceTo(node) / train.Speed;
                start = node;
            }

            // Если конечная точка - шахта, добавляем время добычи
            if (path[path.Count - 1] is Mine mine)
            {
                totalCost += train.BaseMiningTime * mine.MiningTimeMultiplier;
            }

            return totalCost;
        }
    }
}