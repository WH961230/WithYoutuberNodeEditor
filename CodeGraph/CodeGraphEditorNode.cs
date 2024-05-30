using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

public class CodeGraphEditorNode : Node {
    private CodeGraphNode m_graphNode;
    private Port m_outputPort;
    private List<Port> m_ports;
    private SerializedProperty m_serializedProperty;
    public CodeGraphNode Node => m_graphNode;
    public List<Port> Ports => m_ports;
    private SerializedObject m_serializedObject;

    public CodeGraphEditorNode(CodeGraphNode node, SerializedObject codeGraphObject) {
        this.AddToClassList("code-graph-node");

        m_serializedObject = codeGraphObject;
        m_graphNode = node;

        Type typeInfo = node.GetType();
        NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

        title = info.title;

        m_ports = new List<Port>();

        string[] depths = info.menuItem.Split('/');
        foreach (string depth in depths) {
            this.AddToClassList(depth.ToLower().Replace(' ', '-'));
        }
        
        this.name = typeInfo.Name;

        if (info.hasFlowOutput) {
            CreateFlowOutputPort();
        }

        if (info.hasFlowInput) {
            CreateFlowInputPort();
        }

        foreach (FieldInfo property in typeInfo.GetFields()) {
            if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty) {
                DrawProperty(property.Name);
            }
        }

        RefreshExpandedState();
    }

    private PropertyField DrawProperty(string propertyName) {
        if (m_serializedProperty == null) {
            FetchSerializedProperty();
        }

        SerializedProperty prop = m_serializedProperty.FindPropertyRelative(propertyName);
        PropertyField field = new PropertyField(prop);
        field.bindingPath = prop.propertyPath;
        extensionContainer.Add(field);
        return field;
    }

    private void FetchSerializedProperty() {
        SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");
        if (nodes.isArray) {
            int size = nodes.arraySize;
            for (int i = 0; i < size; i++) {
                var element = nodes.GetArrayElementAtIndex(i);
                var elementId = element.FindPropertyRelative("m_guid");
                if (elementId.stringValue == m_graphNode.id) {
                    m_serializedProperty = element;
                }
            }
        }
    }

    private void CreateFlowInputPort() {
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
        inputPort.portName = "In";
        inputPort.tooltip = "The flow input";
        m_ports.Add(inputPort);
        inputContainer.Add(inputPort);
    }
    
    private void CreateFlowOutputPort() {
        m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
        m_outputPort.portName = "Out";
        m_outputPort.tooltip = "The flow output";
        m_ports.Add(m_outputPort);
        outputContainer.Add(m_outputPort);
    }



    public void SavePosition() {
        m_graphNode.SetPosition(GetPosition());
    }
}