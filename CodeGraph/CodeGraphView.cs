using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeGraphView : GraphView {
    private CodeGraphAsset m_codeGraph;
    private SerializedObject m_serializedObject;
    private CodeGraphEditorWindow m_window;
    public CodeGraphEditorWindow window => m_window;

    public List<CodeGraphEditorNode> m_graphNodes;
    public Dictionary<string, CodeGraphEditorNode> m_nodeDictionary;
    public Dictionary<Edge, CodeGraphConnection> m_connectionDictionary;

    private CodeGraphWindowSearchProvider m_searchProvider;
    
    public CodeGraphView(SerializedObject serializedObject, CodeGraphEditorWindow window) {
        m_serializedObject = serializedObject;
        m_codeGraph = (CodeGraphAsset)serializedObject.targetObject;
        m_window = window;

        m_graphNodes = new List<CodeGraphEditorNode>();
        m_nodeDictionary = new Dictionary<string, CodeGraphEditorNode>();
        m_connectionDictionary = new Dictionary<Edge, CodeGraphConnection>();

        m_searchProvider = ScriptableObject.CreateInstance<CodeGraphWindowSearchProvider>();
        m_searchProvider.graph = this;
        this.nodeCreationRequest = ShowSearchWindow;
        
        StyleSheet style =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/evoreek.lazypan/Editor/CodeGraph/USS/CodeGraphEditor.uss");
        styleSheets.Add(style);
        
        GridBackground background = new GridBackground();
        background.name = "Grid";
        Add(background);
        background.SendToBack();

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ClickSelector());

        DrawNodes();
        DrawConnections();
        // DrawProperties();

        graphViewChanged += OnGraphViewChangedEvent;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        List<Port> allPorts = new List<Port>();
        List<Port> ports = new List<Port>();

        foreach (var node in m_graphNodes) {
            allPorts.AddRange(node.Ports);
        }

        foreach (Port p in allPorts) {
            if (p == startPort) { continue; }
            if (p.node == startPort.node) { continue; }
            if (p.direction == startPort.direction) { continue; }
            if (p.portType == startPort.portType) { ports.Add(p); }
        }
        return ports;
    }

    private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange) {
        if (graphViewChange.movedElements != null) {
            Undo.RecordObject(m_serializedObject.targetObject, "Move Elements");
            foreach (CodeGraphEditorNode editorNode in graphViewChange.movedElements.OfType<CodeGraphEditorNode>()) {
                editorNode.SavePosition();
            }
        }
        if (graphViewChange.elementsToRemove != null) {
            Undo.RecordObject(m_serializedObject.targetObject, "Remove Stuff From Graph");

            List<CodeGraphEditorNode> nodes = graphViewChange.elementsToRemove.OfType<CodeGraphEditorNode>().ToList();
            if (nodes.Count > 0) {
                for (int i = nodes.Count - 1; i >= 0; i--) {
                    RemoveNode(nodes[i]);
                }
            }

            foreach (Edge e in graphViewChange.elementsToRemove.OfType<Edge>()) {
                RemoveConnection(e);
            }
        }

        if (graphViewChange.edgesToCreate != null) {
            foreach (Edge edge in graphViewChange.edgesToCreate) {
                CreateEdge(edge);
            }
        }

        return graphViewChange;
    }

    private void CreateEdge(Edge edge) {
        CodeGraphEditorNode inputNode = (CodeGraphEditorNode)edge.input.node;
        int inputIndex = inputNode.Ports.IndexOf(edge.input);
        CodeGraphEditorNode outputNode = (CodeGraphEditorNode)edge.output.node;
        int outputIndex = outputNode.Ports.IndexOf(edge.output);
        CodeGraphConnection connection =
            new CodeGraphConnection(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);
        m_codeGraph.Connections.Add(connection);
    }

    private void RemoveConnection(Edge e) {
        if (m_connectionDictionary.TryGetValue(e, out CodeGraphConnection connection)) {
            m_codeGraph.Connections.Remove(connection);
            m_connectionDictionary.Remove(e);
        }
    }
    
    private void RemoveNode(CodeGraphEditorNode editorNode) {
        m_codeGraph.Nodes.Remove(editorNode.Node);
        m_nodeDictionary.Remove(editorNode.Node.id);
        m_graphNodes.Remove(editorNode);
        m_serializedObject.Update();
    }

    private void DrawNodes() {
        foreach (CodeGraphNode node in m_codeGraph.Nodes) {
            if (node != null) {
                AddNodeToGraph(node);
            }
        }
        Bind();
    }
    
    private void DrawConnections() {
        if (m_codeGraph.Connections == null) {
            return;
        }

        foreach (CodeGraphConnection connection in m_codeGraph.Connections) {
            DrawConnection(connection);
        }
    }

    private void DrawConnection(CodeGraphConnection connection) {
        CodeGraphEditorNode inputNode = GetNode(connection.inputPort.nodeId);
        CodeGraphEditorNode outputNode = GetNode(connection.outputPort.nodeId);
        if (inputNode == null) { return; }
        if (outputNode == null) { return; }

        Port inPort = inputNode.Ports[connection.inputPort.portIndex];
        Port outPort = outputNode.Ports[connection.outputPort.portIndex];
        Edge edge = inPort.ConnectTo(outPort);
        AddElement(edge);
        
        m_connectionDictionary.Add(edge, connection);
    }

    private CodeGraphEditorNode GetNode(string nodeId) {
        CodeGraphEditorNode node = null;
        m_nodeDictionary.TryGetValue(nodeId, out node);
        return node;
    }

    private void ShowSearchWindow(NodeCreationContext obj) {
        m_searchProvider.target = (VisualElement)focusController.focusedElement;
        SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), m_searchProvider);
    }

    public void Add(CodeGraphNode node) {
        Undo.RecordObject(m_serializedObject.targetObject, "Added Node");
        m_codeGraph.Nodes.Add(node);
        m_serializedObject.Update();
        AddNodeToGraph(node);
        Bind();
    }

    private void AddNodeToGraph(CodeGraphNode node) {
        node.typeName = node.GetType().AssemblyQualifiedName;
        CodeGraphEditorNode editorNode = new CodeGraphEditorNode(node, m_serializedObject);
        editorNode.SetPosition(node.position);
        m_graphNodes.Add(editorNode);
        m_nodeDictionary.Add(node.id, editorNode);
        
        AddElement(editorNode);
    }

    private void Bind() {
        m_serializedObject.Update();
        this.Bind(m_serializedObject);
    }
}