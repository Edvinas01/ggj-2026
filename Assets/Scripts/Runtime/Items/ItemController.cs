using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Transforms;
using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026.Runtime.Items
{
    internal sealed class ItemController : MonoBehaviour
    {
        [Min(0)]
        [SerializeField]
        private int maxItems = 10;

        [SerializeField]
        private Vector2 spawnCooldown = new(10f, 30f);

        [Min(0f)]
        [SerializeField]
        private float randomXZOffset = 0.5f;

        [SerializeField]
        private ItemActor actorPrefab;

        [SerializeField]
        private List<ItemData> items;

        [SerializeField]
        private List<Transform> waypoints;

        private ITransformSystem transformSystem;
        private readonly List<ItemActor> activeItems = new();
        private float cooldown;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.greenYellow;
            foreach (var waypoint in waypoints)
            {
                if (waypoint)
                {
                    UnityEditor.Handles.DrawWireDisc(waypoint.position, Vector3.up, randomXZOffset);
                }
            }
        }
#endif

        private void Awake()
        {
            transformSystem = GameManager.GetSystem<ITransformSystem>();
        }

        private void Start()
        {
            var availablePoints = waypoints.OrderBy(_ => Random.value).ToList();
            for (var index = 0; index < maxItems; index++)
            {
                if (availablePoints.Count <= 0)
                {
                    return;
                }

                var point = availablePoints[^1];
                availablePoints.RemoveAt(availablePoints.Count - 1);

                SpawnRandomItem(point);
            }

            cooldown = RandomUtilities.GetRandomFloat(spawnCooldown);
        }

        private void Update()
        {
            cooldown -= Time.deltaTime;

            if (cooldown > 0)
            {
                return;
            }

            for (var index = activeItems.Count - 1; index >= 0; index--)
            {
                var item = activeItems[index];
                if (item == false)
                {
                    activeItems.RemoveAt(index);
                    cooldown = RandomUtilities.GetRandomFloat(spawnCooldown);
                }
            }

            if (cooldown > 0)
            {
                return;
            }

            if (activeItems.Count >= maxItems)
            {
                return;
            }

            if (waypoints.TryGetRandom(out var point))
            {
                SpawnRandomItem(point);
            }
        }

        private void SpawnRandomItem(Transform point)
        {
            if (items.TryGetRandom(out var item) == false)
            {
                return;
            }

            var position = point.position;
            var offset = Random.insideUnitSphere * randomXZOffset;
            offset.y = 0f;

            var instance = Instantiate(
                original: actorPrefab,
                position: position + offset,
                rotation: point.rotation,
                parent: transformSystem.GetTransform("Runtime_Items")
            );

            instance.Initialize(item);
            activeItems.Add(instance);

            cooldown = RandomUtilities.GetRandomFloat(spawnCooldown);
        }
    }
}
