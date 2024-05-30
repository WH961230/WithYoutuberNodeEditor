using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "LazyPan/NodeGraph")]
public class CodeGraphAsset : ScriptableObject {
    [SerializeReference]
    private List<CodeGraphNode> m_nodes;
    [SerializeField]
    private List<CodeGraphConnection> m_connections;
    public List<CodeGraphNode> Nodes => m_nodes;
    public List<CodeGraphConnection> Connections => m_connections;

    private Dictionary<string, CodeGraphNode> m_NodeDictionary;
    public GameObject gameObject;
    public CodeGraphAsset() {
        m_nodes = new List<CodeGraphNode>();
        m_connections = new List<CodeGraphConnection>();
        m_NodeDictionary = new Dictionary<string, CodeGraphNode>();
    }

    public void Init(GameObject gameObject) {
        this.gameObject = gameObject;
        m_NodeDictionary = new Dictionary<string, CodeGraphNode>();
        foreach (CodeGraphNode node in Nodes) {
            m_NodeDictionary.Add(node.id, node);
        }
    }

    public CodeGraphNode GetStartNode() {
        StartNode[] startNodes = Nodes.OfType<StartNode>().ToArray();
        if (startNodes.Length == 0) {
            Debug.LogError("There is no start node in this graph");
        }

        return startNodes[0];
    }

    public CodeGraphNode GetNode(string nextNodeId) {
        if (m_NodeDictionary.TryGetValue(nextNodeId, out CodeGraphNode node)) {
            return node;
        }

        return null;
    }

    public CodeGraphNode GetNodeFromOutput(string outputNodeId, int index) {
        foreach (CodeGraphConnection connection in m_connections) {
            if (connection.outputPort.nodeId == outputNodeId && connection.outputPort.portIndex == index) {
                string nodeId = connection.inputPort.nodeId;
                CodeGraphNode inputNode = m_NodeDictionary[nodeId];
                return inputNode;
            }
        }
        return null;
    }
}