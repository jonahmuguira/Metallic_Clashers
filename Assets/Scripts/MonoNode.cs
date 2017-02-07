using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoNode : MonoBehaviour
{
    public string sceneName;
    public int levelUnlockedAt;
    public Node node;

    private bool canProgress = false;

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
	            canProgress = true;
                break;

            case Node.NodeState.Unlocked:
                m.color = Color.blue;
                canProgress = true;
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
