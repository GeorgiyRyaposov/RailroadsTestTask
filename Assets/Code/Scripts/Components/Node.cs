using UnityEngine;

namespace Code.Scripts
{
    /// <summary>
    /// Узел графа, содержит соседей
    /// </summary>
    public class Node : MonoBehaviour
    {
        public NodeType Type;
        
        public Edge[] Edges;

        public float GetDistanceTo(Node other)
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                if (Edges[i].Node == other)
                {
                    return Edges[i].Length;
                }
            }

            return Mathf.Infinity;
        }
        
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