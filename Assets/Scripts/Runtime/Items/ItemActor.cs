using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Items
{
    internal sealed class ItemActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private ItemData data;

        [Header("Rendering")]
        [SerializeField]
        private Renderer frontRenderer;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        [Header("Events")]
        [SerializeField]
        private UnityEvent onUsed;

        public ItemData Data => data;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying == false && data && gameObject.scene.IsValid())
            {
                Initialize(data);
            }
        }
#endif

        private void Start()
        {
            if (data)
            {
                Initialize(data);
            }
        }

        public void Initialize(ItemData newData)
        {
            data = newData;

            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, newData.Texture);
            frontRenderer.SetPropertyBlock(block);

            var namePrev = name;
            var nameNext = $"Actor_Item ({data.ItemName})";

            if (string.Equals(namePrev, nameNext) == false)
            {
                name = nameNext;
            }
        }

        public void Use()
        {
            onUsed.Invoke();
            Destroy(gameObject);
        }
    }
}
