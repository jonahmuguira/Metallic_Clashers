namespace StageSelection
{
    using System.Collections.Generic;

    using UnityEngine;

    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        public List<Tree> worlds = new List<Tree>();
        protected override void Init()  //Awake for the Manager
        {
            worlds = 
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0)},
                            new Node(),
                            new Node(),
                        }
                    }
                };

            // Link stage 1-1 to 1-2
            worlds[0].nodes[0].nextNodes.Add(worlds[0].nodes[1]);
        }
    }
}
