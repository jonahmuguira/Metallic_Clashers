namespace StageSelection
{
    using System;
    using System.Collections.Generic;

    using Input.Information;

    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        public UnityEvent onStageSelectionEnd = new UnityEvent();

        public GameObject nodePrefab;
        public Material lineRenderMaterial;
        [HideInInspector]
        public List<Tree> worlds = new List<Tree>();
        public float spacingMagnitude;

        protected override void Init()
        {
            
        }

        protected void Start()  //Awake for the Manager
        {
            worlds = 
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node{stageNumber = "1", normalizedPosition = new Vector2(0, 0), isComplete = true},
                            new Node{stageNumber = "2", normalizedPosition = new Vector2(0, 1)},
                            new Node{stageNumber = "3", normalizedPosition = new Vector2(0, 2)},
                            new Node{stageNumber = "Boss", normalizedPosition = new Vector2(0, 3)},
                            new Node{stageNumber = "1A", normalizedPosition = new Vector2(1, 0)},
                            new Node{stageNumber = "2A", normalizedPosition = new Vector2(-1, 1)},
                            new Node{stageNumber = "2B", normalizedPosition = new Vector2(-2, 1)},
                            new Node{stageNumber = "3A", normalizedPosition = new Vector2(1, 2)},
                        }
                    }
                };

            // Make Links
            LinkNodes(worlds[0].nodes[0], worlds[0].nodes[4]);
            LinkNodes(worlds[0].nodes[0], worlds[0].nodes[1]);
            LinkNodes(worlds[0].nodes[1], worlds[0].nodes[5]);
            LinkNodes(worlds[0].nodes[5], worlds[0].nodes[6]);
            LinkNodes(worlds[0].nodes[1], worlds[0].nodes[2]);
            LinkNodes(worlds[0].nodes[2], worlds[0].nodes[3]);
            LinkNodes(worlds[0].nodes[2], worlds[0].nodes[7]);

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

            // Get Player Data
            var savedData = GameManager.self.playerData.worldData;

            if (savedData.Count == 0)
            {
                GameManager.self.playerData.worldData = worlds;
            }

            else
            {
                for (var i = 0; i < savedData.Count; i++)
                {
                    for (var j = 0; j < worlds[i].nodes.Count; j++)
                    {
                        worlds[i].nodes[j].isComplete = savedData[i].nodes[j].isComplete;
                    }
                }
            }

            // Line Renderers
            var counter = 0;
            foreach (var monoNode in FindObjectsOfType<MonoNode>())
            {
                foreach (var n in monoNode.node.nextNodes)
                {
                    var lineObject = new GameObject { name = "Line Renderer " + counter };  //Create GameObject for LineRenderer
                    counter++;
                    var lineRenderer = lineObject.AddComponent<LineRenderer>(); //Attach Line Renderer
                    lineRenderer.startWidth = .1f;  //Set width
                    lineRenderer.endWidth = .1f;
                    lineRenderer.material = new Material(lineRenderMaterial) { color = (monoNode.node.isComplete) ? Color.blue : Color.red };   // Set Material Color

                    lineRenderer.SetPosition(0, new Vector3(
                        monoNode.node.normalizedPosition.x * spacingMagnitude, 0, monoNode.node.normalizedPosition.y * spacingMagnitude));  // Set Starting position
                    lineRenderer.SetPosition(1, new Vector3(
                        n.normalizedPosition.x * spacingMagnitude, 0, n.normalizedPosition.y * spacingMagnitude));  // Set Ending position
                }
            }
        }

        protected override void OnPress(TouchInformation touchInfo)     // Once Pressed
        {
            var ray = Camera.main.ScreenPointToRay(touchInfo.position); 
            var hit = new RaycastHit();                                     // Make a Hit

            //See if the ray hit anything
            if (!(Physics.Raycast(ray.origin, ray.direction, out hit)))       // If not, stop execution
                return;
            onStageSelectionEnd.Invoke();               // If so, got what we want. Let's go home boys.
        }

        private void LinkNodes(Node n1, Node n2)
        {
            n1.nextNodes.Add(n2);
            n2.prevNodes.Add(n1);
        }
    }
}
