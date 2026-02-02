using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Items
{
    internal sealed class ItemPreview : MonoBehaviour
    {
        [Header("Rendering")]
        [SerializeField]
        private Renderer itemRenderer;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        public void Show(ItemData item)
        {
            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, item.Texture);
            itemRenderer.gameObject.SetActive(true);
            itemRenderer.SetPropertyBlock(block);
        }

        public void Hide()
        {
            itemRenderer.gameObject.SetActive(false);
        }
    }
}
