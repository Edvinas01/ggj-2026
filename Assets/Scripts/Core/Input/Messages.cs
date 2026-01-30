using CHARK.GameManagement.Messaging;

namespace RIEVES.GGJ2026.Core.Input
{
    public readonly struct ControlSchemeChangedMessage : IMessage
    {
        public ControlScheme ControlScheme { get; }

        public ControlSchemeChangedMessage(ControlScheme controlScheme)
        {
            ControlScheme = controlScheme;
        }
    }
}
