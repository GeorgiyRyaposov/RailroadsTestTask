using UnityEngine;

namespace Code.Scripts
{
    /// <summary>
    /// Узел базы, куда свозят ресурсы
    /// </summary>
    public class Base : Node
    {
        [Range(0.1f, 5f)] public float ResourceMultiplier = 1f;

        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireCube(transform.position, Vector3.one * 0.55f);
        //
        //     GUI.color = Color.white;
        //     UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Base\nx{resourceMultiplier:F1}");
        // }
    }
}