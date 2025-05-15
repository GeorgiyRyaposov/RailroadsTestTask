using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Services;
using UnityEngine;

namespace Code.Scripts.EfficiencyPathSolvers
{
    public class BaseEfficiencyPathSolver : IEfficiencyPathSolver
    {
        private readonly GraphService _graphService;
        private readonly Base[] _baseNodes;

        public BaseEfficiencyPathSolver(GraphService graphService, Node[] nodes)
        {
            _graphService = graphService;
            _baseNodes = nodes.Where(x => x is Base).Cast<Base>().ToArray();
        }
        
        public bool CanHandle<T>() where T : Node
        {
            return typeof(T) == typeof(Base);
        }

        public List<Node> FindBestPath(Node start, Train train)
        {
            Node bestTarget = null;
            List<Node> bestPath = null;
            var bestResourceEfficiency = 0f;

            foreach (var target in _baseNodes)
            {
                var path = _graphService.FindPath(start, target);
                if (path == null || path.Count == 0)
                {
                    continue;
                }

                var pathCost = _graphService.CalculatePathCost(start, path, train.Speed);

                // Рассчитываем эффективность (ресурс/время)
                var efficiency = target.ResourceMultiplier / pathCost;

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
    }
}