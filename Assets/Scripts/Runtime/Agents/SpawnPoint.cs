using UnityEngine;

namespace RIEVES.GGJ2026
{
    public class SpawnPoint : MonoBehaviour
    {
        public float Radius = 2f;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, Radius);
        }
#endif
    }
}
