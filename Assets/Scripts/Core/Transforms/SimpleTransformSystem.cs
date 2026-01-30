using System.Collections.Generic;
using CHARK.GameManagement.Systems;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Transforms
{
    public sealed class SimpleTransformSystem : SimpleSystem, ITransformSystem
    {
        private readonly IDictionary<string, Transform> transforms = new Dictionary<string, Transform>();

        public Transform GetTransform(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (transforms.TryGetValue(name, out var cachedTransform) && cachedTransform)
            {
                return cachedTransform;
            }

            var transformGameObject = new GameObject(name);
            var transform = transformGameObject.transform;
            transforms[name] = transform;

            return transform;
        }
    }
}
