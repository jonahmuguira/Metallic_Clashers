﻿namespace StageSelection
{
    using System;

    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using UnityEngine.UI;

    [Serializable]
    public class Node
    {
        [XmlIgnore, NonSerialized]
        public List<Node> prevNodes = new List<Node>();
        [XmlIgnore, NonSerialized]
        public List<Node> nextNodes = new List<Node>();

        [XmlIgnore]
        public Vector2 normalizedPosition;
        public bool isComplete = false;
        [XmlIgnore]
        public string stageName { get { return worldIndex + " - " + stageNumber; } }
        [XmlIgnore]
        public string stageNumber;
        [XmlIgnore]
        public int worldIndex;
        [XmlIgnore]
        public List<int> enemyInts = new List<int>();
        [XmlIgnore]
        public uint staminaCost = 2;
    }

    public class MonoNode : MonoBehaviour
    {
        public Node node;

        //Testing If the Node is active or not.
        private void Start()
        {
            var image = GetComponent<Image>();

            // If the node is done and playable
            if (node.isComplete)
                image.color = StageSelectionManager.self.nodeCompleted;

            // If the node is playable
            else if (node.prevNodes.Any(n => n.isComplete) || node.prevNodes.Count == 0)
                image.color = StageSelectionManager.self.nodeUnlocked;

            // Node is not playable
            else
                image.color = StageSelectionManager.self.nodeLocked;
        }
    }
}
