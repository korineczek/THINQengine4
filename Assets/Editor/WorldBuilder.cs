using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class WorldBuilder : EditorWindow
{
    [MenuItem("Window/THINQ Engine 4")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WorldBuilder));
    }


    private void OnGUI()
    {
        if (GUILayout.Button("FindWorld"))
        {
            if (GameObject.FindWithTag("World") != null)
            {
                Debug.Log("found world");
            }
        }
        Rect position = new Rect(25, 60, 100, 100);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Button1")) { }
        if (GUILayout.Button("Button2")) { }
        EditorGUI.DrawPreviewTexture(position, Resources.Load("TestAtlas") as Texture);
        GUILayout.EndHorizontal();


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one*10);
    }
}
