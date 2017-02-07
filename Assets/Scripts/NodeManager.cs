using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Library;

public class NodeManager : MonoSingleton<NodeManager>
{
    public int level;
    public List<Node> nodes = new List<Node>();

    protected override void Awake()
    {
        base.Awake();
        nodes = FindObjectsOfType<MonoNode>().Select(node => node.node).ToList();
        foreach (var n in nodes)
        {
            if(n.unlockedAtLevel <= level && n.nodeState == Node.NodeState.Locked)
                n.nodeState = Node.NodeState.Unlocked;
        }
        nodes[nodes.Count - 1].nodeState = Node.NodeState.Completed;
    }
}

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