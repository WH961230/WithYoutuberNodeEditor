using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[NodeInfo("Debug", "Debug/Debug Log Console")]
public class DebugLogNode : CodeGraphNode {
    [ExposedProperty]
    public string logMessage;
    public override string OnProcess(CodeGraphAsset currentGraph) {
        Debug.Log(logMessage);
        return base.OnProcess(currentGraph);
    }
}