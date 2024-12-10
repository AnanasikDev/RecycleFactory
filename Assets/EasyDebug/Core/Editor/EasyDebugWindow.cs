using UnityEngine;
using UnityEditor;
using EasyDebug;
using EasyDebug.Prompts;

public class EasyDebugWindow : EditorWindow
{
    private int tab = 0;
    private string[] tabs = new string[] { "General", "CommandLine", "Prompts", "PipeConsole" };
    private static Color bgdefault;
    private static string version = "2.0.4";

    [MenuItem("Tools/EasyDebug")]
    public static void ShowWindow()
    {
        bgdefault = GUI.backgroundColor;
        EditorWindow.GetWindow<EasyDebugWindow>();
    }

    private void DrawTab_General()
    {

        // main body

        GUILayout.Space(20);
        GUILayout.Label(version + "v | Developed by Ananaseek");
    }

    private void DrawTab_CommandLine()
    {
        GUILayout.Label("Runtime Command Line");
        if (GUILayout.Button("Init Command Line"))
        {
            CommandLine.Create();
        }
        if (GUILayout.Button("Delete Command Line"))
        {
            CommandLine.Delete();
        }

        if (GUILayout.Button("Clear Unity Console"))
        {
            PipeConsole.ClearConsole();
        }
    }

    private void DrawTab_Prompts()
    {
        GUILayout.Label("Runtime gameobject prompts manager");
        TextPromptManager.ShowAll = GUILayout.Toggle(TextPromptManager.ShowAll, "Show all");
        
        GUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(1f, 0.65f, 0.68f);
        
        if (GUILayout.Button("Clear selected"))
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is GameObject)
                    TextPromptManager.DestroyAllPrompts(obj as GameObject);
            }
        }

        if (GUILayout.Button("Clear all"))
        {
            foreach (Object obj in TextPromptManager.GetAllGameobjects())
            {
                if (obj is GameObject)
                    TextPromptManager.DestroyAllPrompts(obj as GameObject);
            }
        }

        GUI.backgroundColor = bgdefault;

        GUILayout.EndHorizontal();
        TextPromptManager.transformMode = (TextPromptTransformMode)EditorGUILayout.EnumPopup(TextPromptManager.transformMode);
    }

    private void DrawTab_PipeConsole()
    {

    }

    private void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, tabs);
        switch (tab)
        {
            case 0:
                DrawTab_General();
                break;
            case 1:
                DrawTab_CommandLine();
                break;
            case 2:
                DrawTab_Prompts();
                break;
            case 3:
                DrawTab_PipeConsole();
                break;
        }
    }
}
