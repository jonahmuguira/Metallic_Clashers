using System.Linq;

namespace StageSelection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CustomInput.Information;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [Serializable]
    public class Tree
    {
        public List<Node> nodes = new List<Node>();
    }

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        private List<Tree> m_Worlds = new List<Tree>();

        private int m_Currentworld;
        private Node m_CurrentNode;

        [SerializeField]
        private Canvas m_Canvas;
        [SerializeField]
        private Text m_StageNameText;
        [SerializeField]
        private Text m_EnemyText;
        [SerializeField]
        private Button m_StartComabtButton;
        [SerializeField]
        private RectTransform m_NodeAnchor;

        [Space, SerializeField]
        private Color m_NodeCompleted;
        [SerializeField]
        private Color m_NodeUnlocked;
        [SerializeField]
        private Color m_NodeLocked;

        [Space, SerializeField]
        private Color m_LineFillColor;
        [SerializeField]
        private Color m_LineBorderColor;

        [Space, SerializeField]
        private float m_WorldSpacing;
        [SerializeField]
        private Vector2 m_MaxAnchorPosition;
        [SerializeField]
        private Vector2 m_MaxAnchorRubberbandPosition;
        [SerializeField]
        private float m_LerpTime;
        [SerializeField]
        private AnimationCurve m_RubberbandAnimationCurve;

        private Vector2 m_OffsetPosition;

        public UnityEvent onStageSelectionEnd = new UnityEvent();

        public GameObject nodePrefab;
        public GameObject linePrefab;

        public float spacingMagnitude = 1;

        public Color nodeCompleted { get { return m_NodeCompleted; } }
        public Color nodeUnlocked { get { return m_NodeUnlocked; } }
        public Color nodeLocked { get { return m_NodeLocked; } }

        protected override void Init()
        {
            m_Worlds =
                new List<Tree>
                {
                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0),
                                worldIndex = 1, enemyInts = new List<int> {1,1,1}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1),
                                worldIndex = 1, enemyInts = new List<int> {1,1,1}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3),
                                worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "1A", normalizedPosition = new Vector2(1, 0),
                                worldIndex = 1, enemyInts = new List<int> {0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(-2, 1),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(1, 2),
                                worldIndex = 1, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                    ,

                    new Tree
                    {
                        nodes = new List<Node>
                        {
                            new Node {stageNumber = "1", normalizedPosition = new Vector2(0, 0),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2", normalizedPosition = new Vector2(0, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3", normalizedPosition = new Vector2(0, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "Boss", normalizedPosition = new Vector2(0, 3),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2A", normalizedPosition = new Vector2(-1, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3A", normalizedPosition = new Vector2(-1, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "2B", normalizedPosition = new Vector2(1, 1),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                            new Node {stageNumber = "3B", normalizedPosition = new Vector2(1, 2),
                                worldIndex = 2, enemyInts = new List<int> {0,0,0}},
                        }
                    }
                };

            // Make Links

            // Tree 1
            LinkNodes(m_Worlds[0].nodes[0], m_Worlds[0].nodes[1]);
            LinkNodes(m_Worlds[0].nodes[1], m_Worlds[0].nodes[2]);
            LinkNodes(m_Worlds[0].nodes[2], m_Worlds[0].nodes[3]);
            LinkNodes(m_Worlds[0].nodes[0], m_Worlds[0].nodes[4]);
            LinkNodes(m_Worlds[0].nodes[1], m_Worlds[0].nodes[5]);
            LinkNodes(m_Worlds[0].nodes[5], m_Worlds[0].nodes[6]);
            LinkNodes(m_Worlds[0].nodes[2], m_Worlds[0].nodes[7]);

            //Link
            LinkNodes(m_Worlds[0].nodes[3], m_Worlds[1].nodes[0]);

            // Tree 2
            LinkNodes(m_Worlds[1].nodes[0], m_Worlds[1].nodes[1]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[2]);
            LinkNodes(m_Worlds[1].nodes[2], m_Worlds[1].nodes[3]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[4]);
            LinkNodes(m_Worlds[1].nodes[4], m_Worlds[1].nodes[5]);
            LinkNodes(m_Worlds[1].nodes[1], m_Worlds[1].nodes[6]);
            LinkNodes(m_Worlds[1].nodes[6], m_Worlds[1].nodes[7]);

            var canvas = FindObjectOfType<Canvas>();
            var treePos = Vector2.zero;

            //Make Node GameObjects.
            foreach (var tree in m_Worlds)
            {
                var nodeGameObjects = new List<GameObject>();
                foreach (var n in tree.nodes)
                {
                    var nodeObject = Instantiate(nodePrefab);

                    var monoNode = nodeObject.AddComponent<MonoNode>();
                    monoNode.node = n;

                    nodeObject.transform.SetParent(m_NodeAnchor);

                    var nodeTransform = nodeObject.GetComponent<RectTransform>();
                    nodeTransform.anchoredPosition =
                        new Vector2(n.normalizedPosition.x * spacingMagnitude,
                        n.normalizedPosition.y * spacingMagnitude) +
                        treePos;

                    var button = nodeObject.GetComponent<Button>();
                    button.onClick.AddListener(() => { SetCurrentNode(n); });
                    nodeGameObjects.Add(nodeObject);
                }

                // Move to center
                var sum = Vector2.zero;
                var numOfObjects = 0;
                foreach (var node in nodeGameObjects)
                {
                    sum += node.GetComponent<RectTransform>().anchoredPosition;
                    numOfObjects++;
                }

                var offset = sum / numOfObjects;

                foreach (var node in nodeGameObjects)
                {
                    node.GetComponent<RectTransform>().anchoredPosition += treePos - offset;
                }

                treePos.x += m_WorldSpacing;
            }

            // Get Player Data
            var savedData = GameManager.self.playerData.worldData;

            if (savedData.Count == 0)
            {
                GameManager.self.playerData.worldData = m_Worlds;
            }

            else
            {
                for (var i = 0; i < savedData.Count; i++)
                {
                    for (var j = 0; j < m_Worlds[i].nodes.Count; j++)
                    {
                        m_Worlds[i].nodes[j].isComplete = savedData[i].nodes[j].isComplete;
                    }
                }
            }

            // Was the last node completed
            if (GameManager.self.currentNode != null)
            {
                foreach (var tree in m_Worlds)
                {
                    foreach (var n in tree.nodes)
                    {
                        if (n.stageName != GameManager.self.currentNode.stageName ||
                            !GameManager.self.currentNode.isComplete)
                            continue;

                        n.isComplete = true;
                        GameManager.self.playerData.worldData = m_Worlds;
                    }
                }
            }

            // Line Renderers
            var counter = 0;
            foreach (var monoNode in FindObjectsOfType<MonoNode>())
            {
                foreach (var n in monoNode.node.nextNodes)
                {
                    if (n.worldIndex != monoNode.node.worldIndex)
                        continue;

                    var lineObject = Instantiate(linePrefab); //Create GameObject for LineRenderer
                    var lineTransform = lineObject.GetComponent<RectTransform>();
                    lineObject.name = "Line " + counter;
                    counter++;

                    lineObject.transform.SetParent(m_NodeAnchor);
                    lineObject.transform.SetAsFirstSibling();

                    GameObject nextNodeGameObject = null;
                    foreach (var g in FindObjectsOfType<MonoNode>())
                    {
                        if (g.GetComponent<MonoNode>().node == n)
                            nextNodeGameObject = g.gameObject;
                    }

                    if (nextNodeGameObject == null)
                        return;

                    var differance = nextNodeGameObject.transform.position - monoNode.transform.position;

                    var linePosition = monoNode.transform.position + (differance / 2f);

                    lineTransform.sizeDelta = Math.Abs(differance.x) > Math.Abs(differance.y)
                        ? new Vector2(differance.magnitude, 15)
                        : new Vector2(15, differance.magnitude);

                    var lineBorderColor = (monoNode.node.isComplete)
                        ? m_LineBorderColor
                        : m_LineBorderColor - new Color(0f, 0f, 0f, 0.75f);

                    var lineFillColor = (monoNode.node.isComplete)
                        ? m_LineFillColor
                        : m_LineFillColor - new Color(0f, 0f, 0f, 0.75f);

                    lineObject.GetComponent<Image>().color = lineBorderColor;
                    lineObject.transform.GetChild(0).GetComponent<Image>().color = lineFillColor;
                    // Set Material Color

                    lineTransform.position = linePosition;
                }
            }

            //Set Up UI
            m_StageNameText.text = "";
            m_EnemyText.text = "";
            m_StartComabtButton.onClick.AddListener(OnStageSelectionEnd);
            m_StartComabtButton.interactable = false;
        }

        private void OnStageSelectionEnd()
        {
            m_StartComabtButton.interactable = false;
            GameManager.self.currentNode = m_CurrentNode;
            GameManager.self.enemyIndexes = m_CurrentNode.enemyInts;
            StaminaManager.self.DamageStamina(m_CurrentNode.staminaCost);
            onStageSelectionEnd.Invoke();
        }

        private void LinkNodes(Node n1, Node n2)
        {
            n1.nextNodes.Add(n2);
            n2.prevNodes.Add(n1);
        }

        protected override void OnDrag(DragInformation dragInfo)
        {
            var dragDirection = dragInfo.delta.normalized;
            var scaledDelta = Vector2.right * dragInfo.delta.x / m_Canvas.scaleFactor;

            var currentPosition = m_NodeAnchor.anchoredPosition;

            m_OffsetPosition += scaledDelta;
            m_OffsetPosition =
                new Vector2(
                    Mathf.Clamp(
                        m_OffsetPosition.x,
                        GetWorldPosition(m_Worlds.Count - 1)
                            - m_MaxAnchorPosition.x - m_MaxAnchorRubberbandPosition.x,
                        m_MaxAnchorPosition.x + m_MaxAnchorRubberbandPosition.x),
                    Mathf.Clamp(
                        m_OffsetPosition.y,
                        m_OffsetPosition.y,
                        m_MaxAnchorPosition.y + m_MaxAnchorRubberbandPosition.y));

            var rubberbandPosition =
                new Vector2(
                    Mathf.Abs(m_OffsetPosition.x) - m_MaxAnchorPosition.x,
                    Mathf.Abs(m_OffsetPosition.y) - m_MaxAnchorPosition.y);

            if (m_OffsetPosition.x > m_MaxAnchorPosition.x)
            {
                var rubberbandDelta =
                    m_MaxAnchorRubberbandPosition.x *
                    m_RubberbandAnimationCurve.Evaluate(
                        rubberbandPosition.x / m_MaxAnchorRubberbandPosition.x);

                currentPosition.x = m_MaxAnchorPosition.x + rubberbandDelta;
            }

            else
            if (m_OffsetPosition.x < GetWorldPosition(m_Worlds.Count - 1) - m_MaxAnchorPosition.x)
            {
                var rubberbandDelta =
                    m_MaxAnchorRubberbandPosition.x *
                    m_RubberbandAnimationCurve.Evaluate(
                        -(m_OffsetPosition.x - GetWorldPosition(m_Worlds.Count - 1)
                        + m_MaxAnchorPosition.x) / m_MaxAnchorRubberbandPosition.x);

                currentPosition.x =
                    GetWorldPosition(m_Worlds.Count - 1) - m_MaxAnchorPosition.x - rubberbandDelta;
            }

            else
                currentPosition = m_OffsetPosition;

            m_NodeAnchor.anchoredPosition = currentPosition;
            StopAllCoroutines();
        }

        protected override void OnEndDrag(DragInformation dragInfo)
        {
            //TODO: Lerp Down
            StartCoroutine(MoveObject());
        }

        private int GetWorldIndex(float xPosition)
        {
            return (int)((xPosition - m_WorldSpacing / 2f) / -m_WorldSpacing);
        }
        private float GetWorldPosition(int worldIndex)
        {
            return worldIndex * -m_WorldSpacing;
        }

        private IEnumerator MoveObject()
        {
            var newPosition =
                new Vector2(
                    GetWorldPosition(GetWorldIndex(m_NodeAnchor.anchoredPosition.x)),
                    m_NodeAnchor.anchoredPosition.y);

            var deltaTime = 0f;
            while (deltaTime < m_LerpTime)
            {
                m_OffsetPosition =
                    Vector2.Lerp(
                        m_NodeAnchor.anchoredPosition, newPosition, deltaTime / m_LerpTime);
                m_NodeAnchor.anchoredPosition = m_OffsetPosition;

                deltaTime += Time.deltaTime;

                yield return null;
            }
        }

        private void SetCurrentNode(Node n)
        {
            m_CurrentNode = n;
            DisplayInformation();
        }

        private void DisplayInformation()
        {
            m_StageNameText.text = "Stage: " + m_CurrentNode.stageName;

            // Enemy Display - Text
            m_EnemyText.text = "";

            var enemyInstances = new Dictionary<string, int>();
            foreach (var index in m_CurrentNode.enemyInts)
            {
                var name = GameManager.self.enemyPrefabList[index].name;
                if (!enemyInstances.ContainsKey(name))
                    enemyInstances.Add(name, 0);
                enemyInstances[name] += 1;
            }

            foreach (var key in enemyInstances)
            {
                m_EnemyText.text += key.Key + " x" + key.Value;
            }

            // Not Enough Stamina clause
            if (StaminaManager.self.value < m_CurrentNode.staminaCost)
            {
                m_StartComabtButton.interactable = false;
                m_StartComabtButton.gameObject.GetComponentInChildren<Text>().text = "Low\nPower";
            }

            else if (m_CurrentNode.isComplete || m_CurrentNode.prevNodes.Any(n => n.isComplete)
                || m_CurrentNode.prevNodes.Count == 0)
            {
                m_StartComabtButton.interactable = true;
                m_StartComabtButton.gameObject.GetComponentInChildren<Text>().text = "Engage";
            }
            else
            {
                m_StartComabtButton.interactable = false;
                m_StartComabtButton.gameObject.GetComponentInChildren<Text>().text = "Locked";
            }
        }
    }
}
