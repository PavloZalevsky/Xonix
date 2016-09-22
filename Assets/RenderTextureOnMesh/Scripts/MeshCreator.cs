namespace Smokoko.Components
{
    using UnityEngine;
    using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif

    [RequireComponent(typeof(PolygonCollider2D), typeof(Mesh))]
    [AddComponentMenu("Smokoko/Component/Mesh")]
    public class MeshCreator : MonoBehaviour
    {
        public Material material;
        [Range(1, 100)]
        public int textureSize = 1;

        [HideInInspector]
        public PolygonCollider2D collider;
        [HideInInspector]
        public MeshRenderer meshRenderer;
        [HideInInspector]
        public MeshFilter meshFilter;
        [HideInInspector]
        public string sortingLayer;
        [HideInInspector]
        public int sortingOrder;

        void OnEnable()
        {
            UpdateRenderers();
        }

        void Start()
        {
            UpdateRenderers();
        }

        void UpdateRenderers()
        {
            if (meshRenderer == null)
                return;
            meshRenderer.sortingLayerName = sortingLayer;
            meshRenderer.sortingOrder = sortingOrder;
        }
    }
}
