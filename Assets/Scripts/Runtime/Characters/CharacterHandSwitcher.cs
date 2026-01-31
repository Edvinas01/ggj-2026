using System;
using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterHandSwitcher : MonoBehaviour
    {
        private enum TextureType
        {
            Give,
            Punch,
        }

        [SerializeField]
        private CharacterActor character;

        [SerializeField]
        private Renderer targetRenderer;

        [SerializeField]
        private TextureType textureType;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        public void RandomizeTexture()
        {
            switch (textureType)
            {
                case TextureType.Give:
                {
                    if (character.CharacterData.GiveHandTextures.TryGetRandom(out var tex) == false)
                    {
                        return;
                    }

                    var block = new MaterialPropertyBlock();

                    block.SetTexture(texturePropertyId, tex);
                    targetRenderer.SetPropertyBlock(block);
                    break;
                }
                case TextureType.Punch:
                {
                    if (character.CharacterData.PunchHandTextures.TryGetRandom(out var tex) == false)
                    {
                        return;
                    }

                    var block = new MaterialPropertyBlock();

                    block.SetTexture(texturePropertyId, tex);
                    targetRenderer.SetPropertyBlock(block);
                    break;
                }
            }
        }
    }
}
