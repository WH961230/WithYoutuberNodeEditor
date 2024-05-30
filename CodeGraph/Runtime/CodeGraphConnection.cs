using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CodeGraphConnection {
    public CodeGraphConnectionPort inputPort;
    public CodeGraphConnectionPort outputPort;

    public CodeGraphConnection(CodeGraphConnectionPort input, CodeGraphConnectionPort output) {
        inputPort = input;
        outputPort = output;
    }

    public CodeGraphConnection(string inputPortId, int inputIndex, string outputPortId, int outputPortIndex) {
        inputPort = new CodeGraphConnectionPort(inputPortId, inputIndex);
        outputPort = new CodeGraphConnectionPort(outputPortId, outputPortIndex);
    }
}

[Serializable]
public struct CodeGraphConnectionPort {
    public string nodeId;
    public int portIndex;

    public CodeGraphConnectionPort(string id, int index) {
        this.nodeId = id;
        this.portIndex = index;
    }
}