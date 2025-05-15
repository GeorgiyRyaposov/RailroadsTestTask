using System.Collections.Generic;
using Code.Scripts.EfficiencyPathSolvers;
using UnityEngine;

namespace Code.Scripts.Services
{
    /// <summary>
    /// Подбирает подходящий PathSolver для нахождения наилучшего пути 
    /// </summary>
    public class EfficiencyPathService
    {
        private readonly IReadOnlyList<IEfficiencyPathSolver> _efficiencyPathSolvers;

        public EfficiencyPathService(IReadOnlyList<IEfficiencyPathSolver> efficiencyPathSolvers)
        {
            _efficiencyPathSolvers = efficiencyPathSolvers;
        }

        public List<Node> FindBestPath<T>(Node start, Train train) where T : Node
        {
            foreach (var solver in _efficiencyPathSolvers)
            {
                if (solver.CanHandle<T>())
                {
                    return solver.FindBestPath(start, train);
                }
            }

            Debug.Log($"Failed to find path for {typeof(T).Name}");
            return null;
        }
    }
}