namespace Smokoko.Components
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    [CustomEditor(typeof(MeshCreator))]
    public class MeshCreatorEditor : Editor
    {
        private bool needToRefresh = false;

        private System.Reflection.PropertyInfo sortingLayerNamesPropInfo = null;
        private bool sortingLayerNamesChecked = false;
        string[] layers = new string[0];

        void OnEnable()
        {
            MeshCreator mc = target as MeshCreator;

            mc.collider = mc.GetComponent<PolygonCollider2D>();
            mc.meshRenderer = mc.GetComponent<MeshRenderer>();
            mc.meshFilter = mc.gameObject.GetComponent<MeshFilter>();

            if (mc.collider == null)
                mc.collider = mc.gameObject.AddComponent<PolygonCollider2D>();
            if (mc.meshRenderer == null)
                mc.meshRenderer = mc.gameObject.AddComponent<MeshRenderer>();
            if (mc.meshFilter == null)
                mc.meshFilter = mc.gameObject.AddComponent<MeshFilter>();

            EditorApplication.update += Redraw;

            Refresh();
        }

        void OnDisable()
        {
            EditorApplication.update -= Redraw;
        }

        void Redraw()
        {
            MeshCreator mc = target as MeshCreator;

            if (mc.meshFilter == null || mc.meshFilter.sharedMesh == null)
                return;


            Vector2[] uvs = new Vector2[mc.collider.points.Length];
            for (int i = 0; i < mc.collider.points.Length; i++)
                uvs[i] = (mc.collider.points[i] + new Vector2(mc.transform.position.x % mc.textureSize, mc.transform.position.y % mc.textureSize)) / mc.textureSize;
            if (mc.meshFilter.sharedMesh.uv.Length == mc.collider.points.Length)
                mc.meshFilter.sharedMesh.uv = uvs;
        }

        void Refresh()
        {
            MeshCreator mc = target as MeshCreator;

            layers = GetSortingLayerNames();

            int index = System.Array.FindIndex(layers, s => s == mc.sortingLayer);
            if (index > -1)
                mc.sortingLayer = layers[index];
            else
                mc.sortingLayer = "Default";


            if (mc.meshFilter != null)
                mc.meshFilter.hideFlags = HideFlags.HideInInspector;

            if (mc.meshRenderer != null)
            {
                mc.meshRenderer.hideFlags = HideFlags.HideInInspector;
                if (mc.meshRenderer.sharedMaterial = null)
                    mc.meshRenderer.sharedMaterial.hideFlags = HideFlags.HideInInspector;
            }

            mc.transform.hideFlags = HideFlags.None;

            needToRefresh = false;
        }

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isCompiling)
            {
                needToRefresh = true;
                return;
            }
            if (needToRefresh)
            {
                Refresh();
            }

            DrawDefaultInspector();

            MeshCreator mc = target as MeshCreator;

            int index = EditorGUILayout.Popup("Sorting Layer: ", System.Array.FindIndex(layers, s => s == mc.sortingLayer), layers);
            if (index > -1)
                mc.sortingLayer = layers[index];
            else
            {//need to fix (update values after compiling)
                Selection.activeObject = null;
                return;
            }

            mc.sortingOrder = EditorGUILayout.IntField("Sorting Order: ", mc.sortingOrder);

            Triangulator t = new Triangulator(mc.collider.points);
            int[] triangles = t.Triangulate();

            Vector3[] vertices = new Vector3[mc.collider.points.Length];
            for (int i = 0; i < mc.collider.points.Length; i++)
                vertices[i] = mc.collider.points[i];

            Vector2[] uvs = new Vector2[mc.collider.points.Length];
            for (int i = 0; i < mc.collider.points.Length; i++)
                uvs[i] = (mc.collider.points[i] + new Vector2(mc.transform.position.x % mc.textureSize, mc.transform.position.y % mc.textureSize)) / mc.textureSize;


            Mesh mesh = mc.meshFilter.sharedMesh ?? new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            mc.meshFilter.sharedMesh = mesh;
            mc.meshRenderer.material = mc.material;

            mc.meshRenderer.sortingLayerName = mc.sortingLayer;
            mc.meshRenderer.sortingOrder = mc.sortingOrder;

            mc.meshFilter.sharedMesh.uv = uvs;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private string[] GetSortingLayerNames()
        {
            if (sortingLayerNamesPropInfo == null && !sortingLayerNamesChecked)
            {
                sortingLayerNamesChecked = true;
                try
                {
                    System.Type IEU = System.Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");
                    if (IEU != null)
                    {
                        sortingLayerNamesPropInfo = IEU.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    }
                }
                catch { }
            }

            if (sortingLayerNamesPropInfo != null)
            {
                return sortingLayerNamesPropInfo.GetValue(null, null) as string[];
            }
            else
            {
                return new string[0];
            }
        }
    }

}