using CHARK.GameManagement.Messaging;
using CHARK.ScriptableScenes;

namespace RIEVES.GGJ2026.Core.Scenes
{
    public readonly struct SceneLoadEnteredMessage : IMessage
    {
        public ScriptableSceneCollection Collection { get; }

        public SceneLoadEnteredMessage(ScriptableSceneCollection collection)
        {
            Collection = collection;
        }
    }

    public readonly struct SceneLoadExitedMessage : IMessage
    {
        public ScriptableSceneCollection Collection { get; }

        public SceneLoadExitedMessage(ScriptableSceneCollection collection)
        {
            Collection = collection;
        }
    }

    public readonly struct SceneUnloadEnteredMessage : IMessage
    {
        public ScriptableSceneCollection Collection { get; }

        public SceneUnloadEnteredMessage(ScriptableSceneCollection collection)
        {
            Collection = collection;
        }
    }
}
