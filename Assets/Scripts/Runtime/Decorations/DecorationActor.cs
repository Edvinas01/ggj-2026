using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Decorations
{
    internal sealed class DecorationActor : MonoBehaviour
    {
        [Header("Destruction")]
        [SerializeField]
        private bool isDestructible = true;

        [SerializeField]
        private Vector2Int maxSelectCountRange = new(10, 100);

        [Header("Events")]
        [SerializeField]
        private UnityEvent onDestroy;

        [SerializeField]
        private UnityEvent onHit;

        private bool isDestroyed;
        private int maxSelectCount;
        private int selectCount;


        private void Start()
        {
            maxSelectCount = RandomUtilities.GetRandomInt(maxSelectCountRange);
        }

        public void Punch()
        {
            if (isDestructible == false)
            {
                return;
            }

            if (isDestroyed)
            {
                return;
            }

            selectCount++;

            if (selectCount >= maxSelectCount)
            {
                onDestroy.Invoke();
                isDestroyed = true;
            }
            else
            {
                onHit.Invoke();
            }
        }
    }
}
