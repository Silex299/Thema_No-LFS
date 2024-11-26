using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class PrefabFinderWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private Dictionary<GameObject, List<GameObject>> groupedPrefabsInScene = new Dictionary<GameObject, List<GameObject>>();
    private bool includeOnlyActive = true;

    [MenuItem("Editor Window/Tools/Prefab Finder")]
    public static void ShowWindow()
    {
        PrefabFinderWindow window = GetWindow<PrefabFinderWindow>("Prefab Finder");
        window.FindPrefabsInScene();
        EditorSceneManager.sceneOpened += (scene, mode) => window.FindPrefabsInScene();
    }

    private void OnGUI()
    {
        includeOnlyActive = EditorGUILayout.Toggle("Include Only Active Objects", includeOnlyActive);

        if (GUILayout.Button("Refresh Prefabs In Scene"))
        {
            FindPrefabsInScene();
        }

        // Automatically load prefabs on window load or after scene changes

        if (groupedPrefabsInScene.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var entry in groupedPrefabsInScene.OrderBy(e => e.Key.name))
            {
                // Display parent prefab with ObjectField and Find button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("", entry.Key, typeof(GameObject), true);
                if (GUILayout.Button("Find Prefab"))
                {
                    Selection.activeObject = entry.Key;
                }
                if (entry.Value.Count > 0 && GUILayout.Button("Show Child Prefabs"))
                {
                    PrefabChildFinderWindow.ShowWindow(entry.Key, entry.Value);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void FindPrefabsInScene()
    {
        groupedPrefabsInScene.Clear();
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (includeOnlyActive && !obj.activeInHierarchy)
            {
                continue;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected)
            {
                GameObject prefabAsset = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (prefabAsset != null)
                {
                    GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                    if (root != null)
                    {
                        if (!groupedPrefabsInScene.ContainsKey(root))
                        {
                            groupedPrefabsInScene[root] = new List<GameObject>();
                        }
                        if (!groupedPrefabsInScene[root].Contains(prefabAsset))
                        {
                            groupedPrefabsInScene[root].Add(prefabAsset);
                        }
                    }
                }
            }
        }
    }
}

public class PrefabChildFinderWindow : EditorWindow
{
    private GameObject parentPrefab;
    private List<GameObject> childPrefabs = new List<GameObject>();
    private Vector2 scrollPosition;

    public static void ShowWindow(GameObject parent, List<GameObject> children)
    {
        PrefabChildFinderWindow window = GetWindow<PrefabChildFinderWindow>("Child Prefabs");
        window.parentPrefab = parent;
        window.childPrefabs = children;
    }

    private void OnGUI()
    {
        if (parentPrefab == null)
        {
            EditorGUILayout.LabelField("No parent prefab selected.");
            return;
        }

        EditorGUILayout.LabelField("Child Prefabs of: " + parentPrefab.name, EditorStyles.boldLabel);

        if (childPrefabs.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (GameObject childPrefab in childPrefabs.OrderBy(p => p.name))
            {
                EditorGUILayout.ObjectField("", childPrefab, typeof(GameObject), true);
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("No child prefabs found.");
        }
    }
}

public class PrefabInstanceFinderWindow : EditorWindow
{
    private GameObject selectedPrefab;
    private List<GameObject> instancesInScene = new List<GameObject>();
    private Vector2 scrollPosition;
    private bool includeOnlyActive;

    public static void ShowWindow(GameObject prefab, bool includeOnlyActiveObjects)
    {
        PrefabInstanceFinderWindow window = GetWindow<PrefabInstanceFinderWindow>("Prefab Instances");
        window.selectedPrefab = prefab;
        window.includeOnlyActive = includeOnlyActiveObjects;
        window.FindInstancesInScene();
    }

    private void OnGUI()
    {
        if (selectedPrefab == null)
        {
            EditorGUILayout.LabelField("No prefab selected.");
            return;
        }

        EditorGUILayout.LabelField("Instances of: " + selectedPrefab.name, EditorStyles.boldLabel);

        if (instancesInScene.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (GameObject instance in instancesInScene.OrderBy(i => i.name))
            {
                EditorGUILayout.ObjectField("", instance, typeof(GameObject), true);
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("No instances found in the scene.");
        }
    }

    private void FindInstancesInScene()
    {
        instancesInScene.Clear();
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (includeOnlyActive && !obj.activeInHierarchy)
            {
                continue;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected)
            {
                GameObject prefabAsset = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (prefabAsset == selectedPrefab)
                {
                    instancesInScene.Add(obj);
                }
            }
        }
    }
}
