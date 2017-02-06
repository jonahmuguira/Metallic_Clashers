using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoNode : MonoBehaviour
{
    public enum NodeState
    {
        Locked,
        Unlocked,
        Completed
    }

    public NodeState nodeState;

    public MonoNode nextNode;
    public NodeCondition unlockCondition;
    public UnityEvent nodeEvent;
    private Material m_Material;

    public void Awake()
    {
        m_Material = gameObject.GetComponent<Renderer>().material;
    }

	public void Update()
    {
        switch (nodeState)
        {
            case NodeState.Completed:
                m_Material.color = Color.green;
                break;

            case NodeState.Unlocked:
                m_Material.color = Color.blue;
                break;

            case NodeState.Locked:
                m_Material.color = Color.gray;
                break;

            default:
                m_Material.color = Color.black;
                break;
        }
	}
}
