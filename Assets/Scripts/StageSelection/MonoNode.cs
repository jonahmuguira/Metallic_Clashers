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
        [XmlIgnore]
        public List<Node> prevNodes = new List<Node>();
        [XmlIgnore]
        public List<Node> nextNodes = new List<Node>();

        [XmlIgnore]
        public Vector2 normalizedPosition;
        public bool isComplete = false;
        [XmlIgnore]
        public string stageName {get { return worldIndex + "-" + stageNumber; } }
        [XmlIgnore]
        public string stageNumber;
        [XmlIgnore]
        public int worldIndex;
    }

    public class MonoNode : MonoBehaviour
    {
        public Node node;

        private void Start()
        {
            var mat = GetComponent<Renderer>().material;
            
            // If the node is done and playable
            if (node.isComplete)    
            {
                mat.color = Color.green;
                return;
            }

            // If the node is playable
            if (node.prevNodes.Any(n => n.isComplete))
            {
                mat.color = Color.blue;
                return;
            }

            // Node is not playable
            mat.color = Color.black;
        }
    }
}
