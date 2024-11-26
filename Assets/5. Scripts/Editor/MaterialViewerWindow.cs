using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class MaterialViewerWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private List<Material> allMaterials;

    [MenuItem("Editor Window/Tools/Material Viewer")]
    public static void ShowWindow()
    {
        GetWindow<MaterialViewerWindow>("Material Viewer");
    }

    private void OnEnable()
    {
        RefreshMaterialList();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh Materials List"))
        {
            RefreshMaterialList();
        }

        if (allMaterials != null && allMaterials.Count > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var material in allMaterials)
            {
                if (material != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(material, typeof(Material), false);
                    if (GUILayout.Button("Show Textures"))
                    {
                        TextureViewerWindow.ShowWindow(material);
                    }
                    if (GUILayout.Button("Show Usage"))
                    {
                        UsageViewerWindow.ShowWindow(material);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("No materials found in the scene.");
        }
    }

    private void RefreshMaterialList()
    {
        allMaterials = new List<Material>();
        Renderer[] renderers = FindObjectsOfType<Renderer>(true); // Include inactive objects

        HashSet<Material> materialSet = new HashSet<Material>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material != null && !materialSet.Contains(material))
                {
                    materialSet.Add(material);
                }
            }
        }

        allMaterials.AddRange(materialSet);
        allMaterials = allMaterials.OrderBy(material => material.name).ToList();
    }
}

public class TextureViewerWindow : EditorWindow
{
    private Material currentMaterial;
    private Vector2 scrollPosition;

    public static void ShowWindow(Material material)
    {
        TextureViewerWindow window = GetWindow<TextureViewerWindow>("Texture Viewer");
        window.currentMaterial = material;
        window.Show();
    }

    private void OnGUI()
    {
        if (currentMaterial == null)
        {
            EditorGUILayout.LabelField("No material selected.");
            return;
        }

        EditorGUILayout.LabelField("Material: " + currentMaterial.name, EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Shader", currentMaterial.shader, typeof(Shader), false);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        Shader shader = currentMaterial.shader;
        int propertyCount = ShaderUtil.GetPropertyCount(shader);

        for (int i = 0; i < propertyCount; i++)
        {
            if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                string propertyName = ShaderUtil.GetPropertyName(shader, i);
                Texture texture = currentMaterial.GetTexture(propertyName);

                if (texture != null)
                {
                    EditorGUILayout.ObjectField(propertyName, texture, typeof(Texture), false);
                }
            }
        }

        GUILayout.EndScrollView();
    }
}

public class UsageViewerWindow : EditorWindow
{
    private Material currentMaterial;
    private Vector2 scrollPosition;
    private List<GameObject> usageObjects;

    public static void ShowWindow(Material material)
    {
        UsageViewerWindow window = GetWindow<UsageViewerWindow>("Usage Viewer");
        window.currentMaterial = material;
        window.RefreshUsageList();
        window.Show();
    }

    private void RefreshUsageList()
    {
        usageObjects = new List<GameObject>();
        Renderer[] renderers = FindObjectsOfType<Renderer>(true); // Include inactive objects

        foreach (Renderer renderer in renderers)
        {
            if (renderer.sharedMaterials.Contains(currentMaterial))
            {
                usageObjects.Add(renderer.gameObject);
            }
        }
    }

    private void OnGUI()
    {
        if (currentMaterial == null)
        {
            EditorGUILayout.LabelField("No material selected.");
            return;
        }

        EditorGUILayout.LabelField("Objects using Material: " + currentMaterial.name, EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        if (usageObjects != null && usageObjects.Count > 0)
        {
            foreach (var obj in usageObjects)
            {
                EditorGUILayout.ObjectField(obj, typeof(GameObject), false);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No objects found using this material.");
        }
        GUILayout.EndScrollView();
    }
}
