using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool NodeCondition(PlayerData playerData);

public class AreaSelectionManager : MonoBehaviour
{
    public List<MonoNode> nodes = new List<MonoNode>();

    public void Awake() 
    {
        foreach (var monoNode in FindObjectsOfType<MonoNode>())
        {
            nodes.Add(monoNode);
        }
    }

    private void Update()
    {
    }
}

