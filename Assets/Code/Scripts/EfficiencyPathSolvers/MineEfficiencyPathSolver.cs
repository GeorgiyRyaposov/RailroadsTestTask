using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Services;
using UnityEngine;

namespace Code.Scripts.EfficiencyPathSolvers
{
    public class MineEfficiencyPathSolver : IEfficiencyPathSolver
    {
        private readonly GraphService _graphService;
        private readonly Mine[] _mineNodes;
        private readonly Base[] _baseNodes;

        public MineEfficiencyPathSolver(GraphService graphService, Node[] nodes)
        {
            _graphService = graphService;
            _mineNodes = nodes.Where(x => x is Mine).Cast<Mine>().ToArray();
            _baseNodes = nodes.Where(x => x is Base).Cast<Base>().ToArray();
        }

        public bool CanHandle<T>() where T : Node
        {
            return typeof(T) == typeof(Mine);
        }
        
        public List<Node> FindBestPath(Node start, Train train)
        {
            Node bestTarget = null;
            List<Node> bestPath = null;
            var bestResourceEfficiency = 0f;

            foreach (var mine in _mineNodes)
            {
                var pathToMine = FindPath(start, mine);
                if (pathToMine == null || pathToMine.Count == 0)
                {
                    continue;
                }

                // Находим ближайшую базу от шахты
                if (!TryFindClosestBase(mine, train, out var closestBase, out var minBaseCost))
                {
                    continue;
                }
                
                // Общее время: путь до шахты + добыча + путь до базы
                var timeToMine = CalculatePathCost(start, pathToMine, train);
                var miningTime = train.BaseMiningTime * mine.MiningTimeMultiplier;
                var totalTime = timeToMine + miningTime + minBaseCost;

                // Эффективность: ресурс с учетом множителя базы / общее время
                var efficiency = closestBase.ResourceMultiplier / totalTime;
                if (efficiency > bestResourceEfficiency)
                {
                    bestTarget = mine;
                    bestPath = pathToMine;
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
        
        private bool TryFindClosestBase(Mine mine, Train train, out Base closestBase, out float minBaseCost)
        {
            closestBase = null;
            minBaseCost = float.MaxValue;

            foreach (var baseNode in _baseNodes)
            {
                var pathToBase = FindPath(mine, baseNode);
                var cost = CalculatePathCost(mine, pathToBase, train);
                if (cost < minBaseCost)
                {
                    minBaseCost = cost;
                    closestBase = baseNode;
                }
            }

            return closestBase != null;
        }
        
        private List<Node> FindPath(Node node, Node target)
        {
            return _graphService.FindPath(node, target);
        }
        
        private float CalculatePathCost(Node start, List<Node> path, Train train)
        {
            return _graphService.CalculatePathCost(start, path, train.Speed);
        }
    }
}