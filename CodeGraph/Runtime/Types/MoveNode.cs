using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Move", "Process/Move", true, false)]
public class MoveNode : CodeGraphNode {
    [ExposedProperty]
    public Vector3 direction;

    public override string OnProcess(CodeGraphAsset currentGraph) {
        currentGraph.gameObject.transform.position += direction;
        return base.OnProcess(currentGraph);
    }
}