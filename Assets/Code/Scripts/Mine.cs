using UnityEngine;

namespace Code.Scripts
{
    public class Mine : Node
    {
        [Range(0.1f, 5f)] public float miningTimeMultiplier = 1f;

        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.magenta;
        //     Gizmos.DrawSphere(transform.position, 0.3f);
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position, 0.35f);
        //
        //     GUI.color = Color.white;
        //     UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Mine\nx{miningTimeMultiplier:F1}");
        // }
    }
}