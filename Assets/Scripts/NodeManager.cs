using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnityEngine;

using Library;

[Serializable]
public class NodeManager : MonoBehaviour
{
    [XmlIgnore]
    public int level;   //Testing Purposes only
    public List<Node> nodes = new List<Node>();

    public NodeManager() { }

    protected void Awake()
    {
        nodes = FindObjectsOfType<MonoNode>().Select(node => node.node).ToList();
        foreach (var n in nodes)
        {
            if(n.unlockedAtLevel <= level && n.nodeState == Node.NodeState.Locked)
                n.nodeState = Node.NodeState.Unlocked;
        }
        nodes.Last().nodeState = Node.NodeState.Completed;
    }

    [ContextMenu("Save Node System")]
    private void Save()
    {
        var nodePath = Environment.CurrentDirectory + "/NodeSystemData.xml";
        var nodeStream = File.Create(nodePath);

        var serializer = new XmlSerializer(typeof(List<Node>));
        serializer.Serialize(nodeStream, nodes);
        nodeStream.Close();
    }

    [ContextMenu("Load Node System")]
    private List<Node> Load()
    {
        var reader = new XmlSerializer(typeof(List<Node>));
        var file = new StreamReader(Environment.CurrentDirectory + "/NodeSystemData.xml");

        var oldNodeManager = (List<Node>)reader.Deserialize(file);
        file.Close();
        return oldNodeManager;
    }
}

[Serializable]
public class Node
{
    public enum NodeState
    {
        Completed,
        Unlocked,
        Locked
    }

    public NodeState nodeState;
    public int unlockedAtLevel;
}