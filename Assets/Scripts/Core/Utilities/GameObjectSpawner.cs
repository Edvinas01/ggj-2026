using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Transforms;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public sealed class GameObjectSpawner : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private GameObject gameObjectPrefab;

        [Header("Features")]
        [SerializeField]
        private string parentTransformName;

        [SerializeField]
        private bool isSpawnOnStart;

        [SerializeField]
        private bool isIgnoreRotation;

        private void Start()
        {
            if (isSpawnOnStart)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            if (isIgnoreRotation)
            {
                Instantiate(gameObjectPrefab, transform.position, Quaternion.identity, parent: GetParentTransform());
            }
            else
            {
                Instantiate(gameObjectPrefab, transform.position, transform.rotation, parent: GetParentTransform());
            }
        }

        private Transform GetParentTransform()
        {
            var transformSystem = GameManager.GetSystem<ITransformSystem>();
            return transformSystem.GetTransform(parentTransformName);
        }
    }
}
