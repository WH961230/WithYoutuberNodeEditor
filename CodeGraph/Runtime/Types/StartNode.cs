using UnityEngine;

[NodeInfo("Start", "Process/Start", false)]
public class StartNode : CodeGraphNode {
    public override string OnProcess(CodeGraphAsset currentGraph) {
        Debug.Log("Start Node");
        return base.OnProcess(currentGraph);
    }
}