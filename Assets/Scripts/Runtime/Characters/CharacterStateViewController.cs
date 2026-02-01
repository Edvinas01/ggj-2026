using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterStateViewController : ViewController<CharacterStateView>
    {
        [SerializeField]
        private CharacterActor character;

        [SerializeField]
        private bool isDebug;

        protected override void Update()
        {
            base.Update();

            if (isDebug == false)
            {
                return;
            }

            var text = "";
            var poi = character.CurrentTarget ? character.CurrentTarget.name : null;
            text += $"\nCurrentAnimationState: <color=red>{character.CurrentAnimationState}</color>";
            text += $"\nCurrentState: <color=red>{character.CurrentState}</color>";
            text += $"\nCurrentActivity: <color=red>{character.CurrentActivity}</color>";
            text += $"\nPOI: <color=red>{poi}</color>";

            View.DebugInfoText = text;
        }
    }
}
