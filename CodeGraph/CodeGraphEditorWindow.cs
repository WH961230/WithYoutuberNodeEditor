using System;
using LazyPan;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CodeGraphEditorWindow : EditorWindow {
    public static void Open(CodeGraphAsset target) {
        CodeGraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<CodeGraphEditorWindow>();
        foreach (var w in windows) {
            if (w.currentGraph == target) {
                w.Focus();
                return;
            }
        }

        CodeGraphEditorWindow window =
            CreateWindow<CodeGraphEditorWindow>(typeof(CodeGraphEditorWindow), typeof(SceneView));
        window.titleContent = new GUIContent($"{target.name}",
            EditorGUIUtility.ObjectContent(target, typeof(CodeGraphAsset)).image);
        window.Load(target);
    }

    [SerializeField]
    private CodeGraphAsset m_currentGraph;

    [SerializeField]
    private SerializedObject m_seriallizedObject;
    
    [SerializeField]
    private CodeGraphView m_currentView;

    public CodeGraphAsset currentGraph => m_currentGraph;

    private void OnEnable() {
        if (m_currentGraph != null) {
            DrawGraph();
        }
    }

    private void OnGUI() {
        if (m_currentGraph != null) {
            if (EditorUtility.IsDirty(m_currentGraph)) {
                this.hasUnsavedChanges = true;
            } else {
                this.hasUnsavedChanges = false;
            }
        }
    }

    private void Load(CodeGraphAsset target) {
        m_currentGraph = target;
        DrawGraph();
    }

    private void DrawGraph() {
        m_seriallizedObject = new SerializedObject(m_currentGraph);
        m_currentView = new CodeGraphView(m_seriallizedObject, this);
        m_currentView.graphViewChanged += OnChange;
        rootVisualElement.Add(m_currentView);
    }

    private GraphViewChange OnChange(GraphViewChange graphviewchange) {
        EditorUtility.SetDirty(m_currentGraph);
        return graphviewchange;
    }
}