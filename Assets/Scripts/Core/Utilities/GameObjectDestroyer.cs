using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public sealed class GameObjectDestroyer : MonoBehaviour
    {
        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}
