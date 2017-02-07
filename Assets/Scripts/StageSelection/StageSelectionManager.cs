namespace StageSelection
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    [Serializable]
    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : MonoBehaviour
    {
        public GameObject nodePrefab;
        public List<Tree> worlds = new List<Tree>();
        public float spacingMagnitude;
        [ContextMenu("Awake")]
        protected void Awake()  //Awake for the Manager
        {
            worlds = 
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node{stageNumber = "1", normalizedPosition = new Vector2(0, 0)},
                            new Node{stageNumber = "2", normalizedPosition = new Vector2(0, 1)},
                            new Node{stageNumber = "3", normalizedPosition = new Vector2(0, 2)},
                        }
                    }
                };

            // Next Lists
            worlds[0].nodes[0].nextNodes.Add(worlds[0].nodes[1]);   // Next: 1-1 to 1-2
            worlds[0].nodes[1].nextNodes.Add(worlds[0].nodes[2]);   // Next: 1-1 to 1-2

            // Previous Lists
            worlds[0].nodes[1].prevNodes.Add(worlds[0].nodes[0]);   // Previous: 1-2 to 1-1
            worlds[0].nodes[2].prevNodes.Add(worlds[0].nodes[1]);   // Previous: 1-3 to 1-2

            //Make GameObjects
            foreach (var tree in worlds)
            {
                foreach (var n in tree.nodes)
                {
                    var nodeObject = 
                        Instantiate(nodePrefab, 
                        new Vector3(
                            n.normalizedPosition.x * spacingMagnitude, 0, n.normalizedPosition.y * spacingMagnitude
                            ), new Quaternion());

                    var monoNode = nodeObject.AddComponent<MonoNode>();
                    monoNode.node = n;
                }
            }

            var savedData = GameManager.self.playerData.worldData;
            for (var i = 0; i < savedData.Count; i++)
            {
                for (var j = 0; j < worlds[i].nodes.Count; j++)
                {
                    worlds[i].nodes[j].isComplete = savedData[i].nodes[j].isComplete;
                }
            }
        }
    }
}
