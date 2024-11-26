using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshUsageFinderWindow : EditorWindow
{
    private Dictionary<Object, List<GameObject>> meshToInstancesMap = new Dictionary<Object, List<GameObject>>();
    private Vector2 scrollPos;
    private bool showHiddenObjects = false;

    [MenuItem("Editor Window/Tools/Mesh Usage Finder")]
    public static void ShowWindow()
    {
        GetWindow<MeshUsageFinderWindow>("Mesh Usage Finder");
    }

    private void OnEnable()
    {
        FindAllMeshesInScene();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Meshes in Scene", EditorStyles.boldLabel);
        showHiddenObjects = EditorGUILayout.Toggle("Show Hidden Objects", showHiddenObjects);

        if (GUILayout.Button("Refresh List"))
        {
            FindAllMeshesInScene();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var meshEntry in meshToInstancesMap.OrderBy(entry => entry.Key.name))
        {
            Object mesh = meshEntry.Key;
            List<GameObject> instances = meshEntry.Value;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("Mesh", mesh, typeof(Mesh), false);

            if (GUILayout.Button("Instances", GUILayout.Width(100)))
            {
                ShowInstancesWindow(mesh, instances);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void FindAllMeshesInScene()
    {
        meshToInstancesMap.Clear();
        var allObjects = FindObjectsOfType<Transform>(showHiddenObjects).Select(obj => obj.gameObject).ToArray();

        foreach (GameObject rootObject in allObjects)
        {
            AddMeshInstancesFromGameObject(rootObject);
            AddSkinnedMeshInstancesFromGameObject(rootObject);
        }
    }

    private void AddMeshInstancesFromGameObject(GameObject obj)
    {
        if (!showHiddenObjects && (!obj.activeInHierarchy))
            return;
        
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            AddInstance(mesh, obj);
        }
    }
    private void AddSkinnedMeshInstancesFromGameObject(GameObject obj)
    {
        
        if (!showHiddenObjects && (!obj.activeInHierarchy))
            return;
        
        SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            AddInstance(mesh, obj);
            
        }

    }
    

    private void AddInstance(Mesh mesh, GameObject obj)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        
        
        if (!string.IsNullOrEmpty(assetPath))
        {
            // Load the root asset from the asset path, which should be the FBX or original model file
            Object rootAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (!meshToInstancesMap.ContainsKey(rootAsset))
            {
                meshToInstancesMap[rootAsset] = new List<GameObject>();
            }

            meshToInstancesMap[rootAsset].Add(obj);
        }
        
    }

    private void ShowInstancesWindow(Object mesh, List<GameObject> instances)
    {
        MeshInstancesWindow.ShowWindow(mesh, instances);
    }

    private void PingFbxModel(Mesh mesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        if (!string.IsNullOrEmpty(assetPath))
        {
            Object fbxModel = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (fbxModel != null)
            {
                EditorGUIUtility.PingObject(fbxModel);
            }
        }
    }
}

public class MeshInstancesWindow : EditorWindow
{
    private Object obj;
    private List<GameObject> instances;
    private Vector2 scrollPos;

    public static void ShowWindow(Object obj, List<GameObject> instances)
    {
        MeshInstancesWindow window = GetWindow<MeshInstancesWindow>("Mesh Instances");
        window.obj = obj;
        window.instances = instances;
    }

    private void OnGUI()
    {
        if (obj == null || instances == null)
        {
            EditorGUILayout.LabelField("No mesh selected.");
            return;
        }

        EditorGUILayout.LabelField($"Instances of {obj.name}", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var instance in instances.OrderBy(instance => instance.name))
        {
            if (instance.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                EditorGUILayout.ObjectField("", instance, typeof(GameObject), true);
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
