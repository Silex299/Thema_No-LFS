using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using Object = UnityEngine.Object;

public class ScriptReferenceViewer : EditorWindow
{
    [MenuItem("Editor Window/Tools/Script Reference Viewer")]
    public static void ShowWindow()
    {
        var window = GetWindow<ScriptReferenceViewer>("Script Reference Viewer");
        window.LoadFilter();
        window.FindCustomScriptsAndReferences();
    }
    

    private Vector2 scrollPos;
    private Dictionary<MonoBehaviour, List<Object>> scriptReferences = new Dictionary<MonoBehaviour, List<Object>>();
    private string searchQuery = string.Empty; // Input field for search
    private string filterQuery = string.Empty; // Input field for filtering scripts
    private readonly string savePath = "ScriptReferenceFilter.txt"; // Path to save the filter

    private void OnGUI()
    {
        GUILayout.Label("Search and View Script References", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();

        // Automatically load filter and list scripts if available
        if (string.IsNullOrEmpty(filterQuery) && scriptReferences.Count == 0)
        {
            LoadFilter();
            FindCustomScriptsAndReferences();
        }

        // Search input field
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.Width(50));
        searchQuery = EditorGUILayout.TextField(searchQuery);
        EditorGUILayout.EndHorizontal();

        // Filter input field
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Exclude (Filter): ", GUILayout.Width(100));
        filterQuery = EditorGUILayout.TextField(filterQuery);
        EditorGUILayout.EndHorizontal();

        // Save and Load buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Filter", GUILayout.Width(100)))
        {
            SaveFilter();
        }
        if (GUILayout.Button("Load Filter", GUILayout.Width(100)))
        {
            LoadFilter();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("List All Custom Scripts and Their References"))
        {
            FindCustomScriptsAndReferences();
        }

        if (scriptReferences.Count > 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var entry in scriptReferences.OrderBy(entry => entry.Key.GetType().Name))
            {
                // Check if script is included in the search or not excluded by the filter
                if (ShouldDisplayScript(entry.Key.GetType().Name))
                {
                    EditorGUILayout.BeginHorizontal();

                    
                    GUI.backgroundColor = Color.white;
                    GUILayout.Space(5);


                    EditorGUILayout.LabelField($"{entry.Key.GetType().Name} ({entry.Key.gameObject.name})", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.ExpandWidth(true));
                    GUILayout.Space(5);
                   
                    
                     
                    GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_Transform Icon"), GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        FindObjectInScene(entry.Key.gameObject);
                    }

                    

                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Search Icon"), GUILayout.Width(25), GUILayout.Height(25), GUILayout.ExpandWidth(false)))
                    {
                        FindScriptInFolder(entry.Key);
                    }

                    GUI.backgroundColor = Color.cyan;
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_UnityEditor.SceneHierarchyWindow@2x"), GUILayout.Height(25), GUILayout.Width(25)))
                    {
                        GetAllDependencies(entry.Key);
                    }
                    
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close_a@2x"), GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        AddToExcludeFilter(entry.Key.GetType().Name);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    GUI.backgroundColor = Color.black;
                    GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
                    
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private bool ShouldDisplayScript(string scriptName)
    {
        // Check if the script matches the search query
        bool matchesSearch = string.IsNullOrEmpty(searchQuery) || scriptName.ToLower().Contains(searchQuery.ToLower());

        // Check if the script is excluded by the filter query (comma-separated list)
        bool excludedByFilter = !string.IsNullOrEmpty(filterQuery) && filterQuery.ToLower()
            .Split(',')
            .Select(f => f.Trim())
            .Contains(scriptName.ToLower());

        return matchesSearch && !excludedByFilter;
    }

    private void AddToExcludeFilter(string scriptName)
    {
        // Add the script to the exclude filter if it's not already there
        var filters = filterQuery.ToLower().Split(',').Select(f => f.Trim()).ToList();
        if (!filters.Contains(scriptName.ToLower()))
        {
            filters.Add(scriptName.ToLower());
            filterQuery = string.Join(", ", filters);
        }
    }

    private void SelectScriptInScene(MonoBehaviour script)
    {
        if (script != null)
        {
            Selection.activeGameObject = script.gameObject;
            EditorGUIUtility.PingObject(script.gameObject); // Highlight in the hierarchy
        }
    }

    private void FindObjectInScene(GameObject gameObject)
    {
        //select the gameobject int the scene
        Selection.activeGameObject = gameObject;
        EditorGUIUtility.PingObject(gameObject);
    }
    
    private void FindScriptInFolder(MonoBehaviour script)
    {
        if (script != null)
        {
            // Find the MonoScript (source file) associated with the MonoBehaviour
            MonoScript monoScript = MonoScript.FromMonoBehaviour(script);
            if (monoScript != null)
            {
                Selection.activeObject = monoScript; // Highlight the script in the Project window
                EditorGUIUtility.PingObject(monoScript); // Ensure it is visible
            }
        }
    }

    private void GetAllDependencies(MonoBehaviour script)
    {
        if (script != null)
        {
            // Create a new window to list all dependencies
            ScriptDependenciesViewer.ShowWindow(script);
        }
    }

    private void SaveFilter()
    {
        try
        {
            System.IO.File.WriteAllText(savePath, filterQuery);
            EditorUtility.DisplayDialog("Save Filter", "Filter saved successfully.", "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving filter: {ex.Message}");
            EditorUtility.DisplayDialog("Save Filter", "Failed to save the filter.", "OK");
        }
    }

    private void LoadFilter()
    {
        try
        {
            if (System.IO.File.Exists(savePath))
            {
                filterQuery = System.IO.File.ReadAllText(savePath);
                EditorUtility.DisplayDialog("Load Filter", "Filter loaded successfully.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Load Filter", "No saved filter found.", "OK");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading filter: {ex.Message}");
            EditorUtility.DisplayDialog("Load Filter", "Failed to load the filter.", "OK");
        }
    }

    private void FindCustomScriptsAndReferences()
    {
        scriptReferences.Clear();

        var allGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var go in allGameObjects.SelectMany(root => root.GetComponentsInChildren<Transform>(true)).Select(t => t.gameObject))
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue; // Skip irrelevant objects

            var scripts = go.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script == null) continue; // Skip missing scripts

                var references = new List<Object>();

                // Find fields in the script that reference other objects
                var fields = script.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (typeof(Object).IsAssignableFrom(field.FieldType))
                    {
                        var referencedObject = field.GetValue(script) as Object;
                        if (referencedObject != null)
                        {
                            references.Add(referencedObject);
                        }
                    }
                }

                scriptReferences.Add(script, references);
            }
        }
    }
}

public class ScriptDependenciesViewer : EditorWindow
{
    private MonoBehaviour script;
    private Vector2 scrollPos;
    private List<Object> dependencies = new List<Object>();
    private List<string> fileDependencies = new List<string>();
    private List<string> typeDependencies = new List<string>();

    public static void ShowWindow(MonoBehaviour script)
    {
        var window = GetWindow<ScriptDependenciesViewer>("Script Dependencies Viewer");
        window.script = script;
        window.FindDependencies();
    }

    private void FindDependencies()
    {
        dependencies.Clear();
        fileDependencies.Clear();
        typeDependencies.Clear();

        if (script != null)
        {
            // Find all fields that reference other Unity objects or types
            var fields = script.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(Object).IsAssignableFrom(field.FieldType))
                {
                    var referencedObject = field.GetValue(script) as Object;
                    if (referencedObject != null)
                    {
                        dependencies.Add(referencedObject);
                    }
                }
                else
                {
                    // Add the type name to the list of type dependencies
                    typeDependencies.Add($"{field.FieldType.FullName}");
                }
            }

            // Find script file dependencies
            MonoScript monoScript = MonoScript.FromMonoBehaviour(script);
            if (monoScript != null)
            {
                string scriptPath = AssetDatabase.GetAssetPath(monoScript);
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    string[] dependentFiles = AssetDatabase.GetDependencies(scriptPath, true);
                    fileDependencies.AddRange(dependentFiles);
                }
            }
        }
    }

    private void OnGUI()
    {
        if (script == null)
        {
            GUILayout.Label("No script selected.", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label($"Dependencies for Script: {script.GetType().Name}", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Object Dependencies:", EditorStyles.boldLabel);
        foreach (var dependency in dependencies)
        {
            EditorGUILayout.LabelField($"- {dependency.GetType().Name}");
            if (GUILayout.Button("Select", GUILayout.Width(100)))
            {
                Selection.activeObject = dependency;
                EditorGUIUtility.PingObject(dependency);
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("File Dependencies (Tree View):", EditorStyles.boldLabel);
        foreach (var file in fileDependencies)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20); // Indent to simulate a tree structure
            EditorGUILayout.LabelField(file);
            if (GUILayout.Button("Select in Project", GUILayout.Width(150)))
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(file);
                if (obj != null)
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        GUILayout.Label("Type Dependencies (Tree View):", EditorStyles.boldLabel);
        
        foreach (var type in typeDependencies)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20); // Indent to simulate a tree structure
            EditorGUILayout.LabelField(type);
            if (GUILayout.Button("Find Type Definition", GUILayout.Width(150)))
            {
                // Attempt to find the file in the project using namespace and type
                string[] typeParts = type.Split('+');
                typeParts = typeParts[0].Split(".");

                foreach (var part in typeParts)
                {
                    Debug.Log(part);
                    string[] guids = AssetDatabase.FindAssets($"{part} t:Script").Where(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)) == part).ToArray();
                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        Debug.LogWarning(path);
                        if (Path.GetFileNameWithoutExtension(path) == part)
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                            if (obj != null)
                            {
                                Selection.activeObject = obj;
                                EditorGUIUtility.PingObject(obj);
                                break;
                            }
                        }
                    }
                }
                
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
