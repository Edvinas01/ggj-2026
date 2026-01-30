using RIEVES.GGJ2026.Core.Interaction.Interactables;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    public readonly struct InteractorHoverEnteredArgs : IInteractorEventArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractorHoverEnteredArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractorHoverExitedArgs : IInteractorEventArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractorHoverExitedArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractorSelectEnteredArgs : IInteractorEventArgs
    {
        public IInteractable Interactable { get; }

        public InteractorSelectEnteredArgs(IInteractable interactable)
        {
            Interactable = interactable;
        }
    }

    public readonly struct InteractorSelectExitedArgs : IInteractorEventArgs
    {
        public IInteractable Interactable { get; }

        public InteractorSelectExitedArgs(IInteractable interactable)
        {
            Interactable = interactable;
        }
    }

    public interface IInteractorEventArgs
    {
        public IInteractable Interactable { get; }
    }
}
