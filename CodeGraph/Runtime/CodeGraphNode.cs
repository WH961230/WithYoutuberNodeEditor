using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CodeGraphNode {
    [SerializeField]
    private string m_guid;

    [SerializeField]
    private Rect m_position;

    public string typeName;

    public string id => m_guid;

    public Rect position => m_position;
    public CodeGraphNode() {
        NewGUID();
    }

    private void NewGUID() {
        m_guid = Guid.NewGuid().ToString();
    }

    public void SetPosition(Rect position) {
        m_position = position;
    }

    public virtual string OnProcess(CodeGraphAsset currentGraph) {
        CodeGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, 0);
        if (nextNodeInFlow != null) {
            return nextNodeInFlow.id;
        }

        return string.Empty;
    }

    public virtual string OnProcessUpdate(CodeGraphAsset currentGraph) {
        CodeGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, 0);
        if (nextNodeInFlow != null) {
            return nextNodeInFlow.id;
        }

        return string.Empty;
    }
}