using UnityEngine;

namespace InSun.JamOne.Core.Utilities
{
    public sealed class GameObjectDestroyer : MonoBehaviour
    {
        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}
