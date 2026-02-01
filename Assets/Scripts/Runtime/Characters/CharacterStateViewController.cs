using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterStateViewController : ViewController<CharacterStateView>
    {
        [SerializeField]
        private CharacterActor character;

        [SerializeField]
        private ParticleSystem hunterParticleSystem;

        [SerializeField]
        private bool isDebug;

        private CharacterState characterStatePrev;

        protected override void Awake()
        {
            base.Awake();
            characterStatePrev = character.CurrentState;
        }

        protected override void Update()
        {
            base.Update();

            var characterStateNext = character.CurrentState;
            if (characterStatePrev == characterStateNext)
            {
                return;
            }

            characterStatePrev = characterStateNext;

            switch (characterStateNext)
            {
                case CharacterState.Hunting:
                {
                    hunterParticleSystem.Play();
                    break;
                }
                default:
                {
                    hunterParticleSystem.Stop();
                    break;
                }
            }

            if (isDebug == false)
            {
                return;
            }

            var text = "";
            var poi = character.CurrentTarget ? character.CurrentTarget.name : null;
            text += $"CurrentAnimationState: <color=red>{character.CurrentAnimationState}</color>";
            text += $"\nCurrentState: <color=red>{character.CurrentState}</color>";
            text += $"\nCurrentActivity: <color=red>{character.CurrentActivity}</color>";
            text += $"\nPOI: <color=red>{poi}</color>";
            text += $"\nStateChangeTimer: <color=red>{character.StateChangeTimer:F1}</color>";
            text += $"\nCallbackCount: <color=red>{character.CallbackCount}</color>";
            text += $"\nAnimationDelayTimer: <color=red>{character.AnimationDelayTimer:F1}</color>";
            text += $"\nStateChangeTimer: <color=red>{character.StateChangeTimer:F1}</color>";

            View.DebugInfoText = text;
        }
    }
}
