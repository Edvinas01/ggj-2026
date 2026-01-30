using RIEVES.GGJ2026.Core.Interaction.Interactors;

namespace RIEVES.GGJ2026.Core.Interaction.Interactables
{
    public readonly struct InteractableHoverEnteredArgs : IInteractableEventArgs
    {
        public IInteractor Interactor { get; }

        public InteractableHoverEnteredArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }

    public readonly struct InteractableHoverExitedArgs : IInteractableEventArgs
    {
        public IInteractor Interactor { get; }

        public InteractableHoverExitedArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }

    public readonly struct InteractableSelectEnteredArgs : IInteractableEventArgs
    {
        public IInteractor Interactor { get; }

        public InteractableSelectEnteredArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }

    public readonly struct InteractableSelectExitedArgs : IInteractableEventArgs
    {
        public IInteractor Interactor { get; }

        public InteractableSelectExitedArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }

    public interface IInteractableEventArgs
    {
        public IInteractor Interactor { get; }
    }
}
