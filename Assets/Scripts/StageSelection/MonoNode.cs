namespace StageSelection
{
    using System;

    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [Serializable]
    public class Node
    {
        public Vector2 normalizedPosition;
        public bool isComplete = false;
        public string stageName;
        public string stageNumber;
        public int worldIndex;
    }

    public class MonoNode : MonoBehaviour
    {
        public Node node;

        public List<MonoNode> prevNodes = new List<MonoNode>();
        public List<MonoNode> nextNodes = new List<MonoNode>();

        private void Awake()
        {
            var mat = GetComponent<Renderer>().material;

            if (node.isComplete)    // If the node is done and playable
            {
                mat.color = Color.green;
                return;
            }

            // If the node is playable
            if (prevNodes.Any(n => n.node.isComplete))
            {
                mat.color = Color.blue;
                return;
            }

            // Node is not playable
            mat.color = Color.black;
        }
    }
}
