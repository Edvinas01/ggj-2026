using System.Collections.Generic;
using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;

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

        private readonly List<ItemActor> activeItems = new();
        private float cooldown;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            foreach (var waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 0.1f);
            }
        }

        private void Start()
        {
            var availablePoints = new List<Transform>(waypoints);
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

            var instance = Instantiate(actorPrefab, position: position + offset, rotation: point.rotation);
            instance.Initialize(item);
            activeItems.Add(instance);

            cooldown = RandomUtilities.GetRandomFloat(spawnCooldown);
        }
    }
}
