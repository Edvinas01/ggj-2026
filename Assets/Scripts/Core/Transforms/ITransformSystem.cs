using CHARK.GameManagement.Systems;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Transforms
{
    public interface ITransformSystem : ISystem
    {
        public Transform GetTransform(string name);
    }
}
