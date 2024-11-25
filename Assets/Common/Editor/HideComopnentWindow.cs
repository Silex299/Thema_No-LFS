using System;
using UnityEditor;
using UnityEngine;

public class HideComopnentWindow : EditorWindow
{  
   Vector2 scrollPos;Vector2 scrollPos1;
   
   [MenuItem("Editor Window/Hide Components")]
   public static void ShowWindow()
   {
      EditorWindow.GetWindow<HideComopnentWindow>("Hide Components"); 
   }
   

   void OnGUI()
   {
     
      
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(0), GUILayout.Height(0));
      EditorGUILayout.BeginVertical();

      
      /*** Get All selected gameobjects ***/
      
      foreach (GameObject obj in Selection.gameObjects)
      {
         Component[] comps = obj.GetComponents<Component>();
         
         GUILayout.Space(20);
         
         var style = new GUIStyle();
         style.padding = new RectOffset(10, 10, 10, 10);
         style.normal.background = new Texture2D(100, 20);
         GUI.backgroundColor = Color.blue;
         style.normal.textColor = Color.white;;
         GUILayout.Box(obj.name, style);
         GUILayout.Space(20);
         
         // ### Get Each elements of a gameobject
         
         foreach (var comp in comps)
         {
            EditorGUILayout.BeginHorizontal();
            {
               string str = comp.ToString();
               str = str.Replace(comp.name, "");

               GUILayout.Label(str);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            {
               if (comp.hideFlags == HideFlags.HideInInspector)
               {
                  GUI.backgroundColor = Color.green;
                  if (GUILayout.Button("Show", GUILayout.Width(50), GUILayout.Height(20)))
                  {
                     comp.hideFlags = HideFlags.None;
                  }
               }
               else
               {
                  GUI.backgroundColor = Color.red;
                  if (GUILayout.Button("Hide", GUILayout.Width(50), GUILayout.Height(20)))
                  {
                     comp.hideFlags = HideFlags.HideInInspector;
                  }
               }
               

               
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(20);
         }
      }
      
         
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
      
     
   }
   
   
   
   
}
