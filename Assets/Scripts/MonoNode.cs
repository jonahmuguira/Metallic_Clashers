using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoNode : MonoBehaviour
{
    public int levelUnlockedAt;
    public Node node;
	private void Awake()
	{
	    node = new Node {nodeState = Node.NodeState.Locked, unlockedAtLevel = levelUnlockedAt};
	}

	private void Start()
	{
	    var m = GetComponent<Renderer>().material;
	    switch (node.nodeState)
	    {
	        case Node.NodeState.Completed:
                m.color = Color.green;
	            break;

            case Node.NodeState.Unlocked:
                m.color = Color.blue;
                break;

            case Node.NodeState.Locked:
                m.color = Color.red;
                break;

            default:
                m.color = Color.black;
                break;
	    }
	}
}
