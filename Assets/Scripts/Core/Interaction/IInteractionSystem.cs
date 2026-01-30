using System.Collections.Generic;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Interaction.Interactors;

namespace RIEVES.GGJ2026.Core.Interaction
{
    public interface IInteractionSystem : ISystem
    {
        public IReadOnlyList<IInteractor> Interactors { get; }

        public void AddInteractor(IInteractor interactor);

        public void RemoveInteractor(IInteractor interactor);
    }
}
