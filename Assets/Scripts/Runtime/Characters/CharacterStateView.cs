using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterStateView : View
    {
        [SerializeField]
        private TMP_Text debugInfoText;

        public string DebugInfoText
        {
            set => debugInfoText.text = value;
        }
    }
}
