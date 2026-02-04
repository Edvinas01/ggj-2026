using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Decorations
{
    internal sealed class DecorationActor : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField]
        private Interactable interactable;

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

        private void OnEnable()
        {
            interactable.OnSelectEntered += OnSelectEntered;
        }

        private void OnDisable()
        {
            interactable.OnSelectEntered -= OnSelectEntered;
        }

        private void OnSelectEntered(InteractableSelectEnteredArgs args)
        {
            args.Interactor.Deselect();

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
