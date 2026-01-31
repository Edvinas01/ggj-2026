using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterHandSwitcher : MonoBehaviour
    {
        [SerializeField]
        private CharacterActor character;

        [SerializeField]
        private Renderer targetRenderer;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        public void RandomizeTexture()
        {
            if (character.CharacterData.HandTextures.TryGetRandom(out var tex) == false)
            {
                return;
            }

            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, tex);
            targetRenderer.SetPropertyBlock(block);
        }
    }
}
