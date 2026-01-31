using System.Collections.Generic;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public sealed class RandomActivator : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> gameObjects;

        private void Start()
        {
            foreach (var obj in gameObjects)
            {
                obj.SetActive(false);
            }

            if (gameObjects.TryGetRandom(out var randomObj))
            {
                randomObj.SetActive(true);
            }
        }
    }
}
