using UnityEngine;

namespace Code.Scripts
{
    /// <summary>
    /// Узел графа, содержит соседей
    /// </summary>
    public class Node : MonoBehaviour
    {
        public Edge[] Edges;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < Edges.Length; i++)
            {
                if (Edges[i].Node)
                {
                    var node = Edges[i].Node;
                    Gizmos.DrawLine(transform.position, node.transform.position);
                }
            }
        }
    }
}