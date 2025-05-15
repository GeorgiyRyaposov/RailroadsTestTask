using System.Collections.Generic;

namespace Code.Scripts.EfficiencyPathSolvers
{
    /// <summary>
    /// Находит наилучший путь для какого-то вида узла
    /// </summary>
    public interface IEfficiencyPathSolver
    {
        bool CanHandle<T>() where T : Node; 
        List<Node> FindBestPath(Node start, Train train);
    }
}