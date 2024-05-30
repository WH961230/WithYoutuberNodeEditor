using System;
using UnityEngine;

public class CodeGraphObject : MonoBehaviour {
    [SerializeField]
    private CodeGraphAsset m_graphAsset;

    private CodeGraphAsset graphInstance;
    private void OnEnable() {
        graphInstance = Instantiate(m_graphAsset);
        ExecuteAsset();
    }

    private void ExecuteAsset() {
        graphInstance.Init(gameObject);
        CodeGraphNode startNode = graphInstance.GetStartNode();
        ProcessAndMoveToNextNode(startNode);
    }

    private void ProcessAndMoveToNextNode(CodeGraphNode startNode) {
        string nextNodeId = startNode.OnProcess(graphInstance);
        if (!string.IsNullOrEmpty(nextNodeId)) {
            CodeGraphNode node = graphInstance.GetNode(nextNodeId);
            ProcessAndMoveToNextNode(node);
        }
    }
}