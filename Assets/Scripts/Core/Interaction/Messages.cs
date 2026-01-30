using CHARK.GameManagement.Messaging;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Core.Interaction.Interactors;

namespace RIEVES.GGJ2026.Core.Interaction
{
    public readonly struct InteractorHoveredEnteredMessage : IMessage
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractorHoveredEnteredMessage(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractorHoveredExitedMessage : IMessage
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractorHoveredExitedMessage(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }
}
