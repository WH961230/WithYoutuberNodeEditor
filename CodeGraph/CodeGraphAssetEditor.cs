using LazyPan;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(CodeGraphAsset))]
public class CodeGraphAssetEditor : Editor {
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int index) {
        Object asset = EditorUtility.InstanceIDToObject(instanceId);
        if (asset.GetType() == typeof(CodeGraphAsset)) {
            CodeGraphEditorWindow.Open((CodeGraphAsset)asset);
            return true;
        }

        return false;
    }
    
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Edit")) {
            CodeGraphEditorWindow.Open((CodeGraphAsset)target);
        }
    }
}