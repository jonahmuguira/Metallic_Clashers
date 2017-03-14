using UnityEngine;

namespace Board
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using UnityEngine.Events;

    public class GridResizeEvent : UnityEvent<GridResizeInformation> { }

    [RequireComponent(typeof(RectTransform))]
    public class GridMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private RectTransform m_RectTransform;
        [SerializeField]
        private RectTransform m_ParentRectTransform;

        [SerializeField]
        private GridResizeEvent m_OnGridResize = new GridResizeEvent();

        private Vector2 m_PreviousRectSize;

        private bool m_GemsAreAnimating;
        private List<IEnumerator> m_GemMatchAnimations = new List<IEnumerator>();

        public Grid grid { get { return m_Grid; } }

        public RectTransform rectTransform { get { return m_RectTransform; } }
        public RectTransform parentRectTransform { get { return m_ParentRectTransform; } }

        public GridResizeEvent onGridResize { get { return m_OnGridResize; } }

        public List<IEnumerator> gemMatchAnimations { get { return m_GemMatchAnimations; } }

        private void LateUpdate()
        {
            var currentRectSize = new Vector2(m_RectTransform.rect.width, m_RectTransform.rect.height);
            if (currentRectSize == m_PreviousRectSize)
                return;

            m_OnGridResize.Invoke(new GridResizeInformation { newRect = m_RectTransform.rect });

            m_PreviousRectSize = currentRectSize;
        }

        private void OnCombatUpdate()
        {
            if (m_GemMatchAnimations.Count != 0)
            {
                m_GemsAreAnimating = true;

                var tempList = m_GemMatchAnimations.ToList();
                foreach (var gemMatchAnimation in tempList)
                    if (!gemMatchAnimation.MoveNext())
                        m_GemMatchAnimations.Remove(gemMatchAnimation);
            }
            else if (m_GemsAreAnimating)
            {
                m_Grid.ApplyGravity();
                m_Grid.Fill();

                m_Grid.CheckMatch();

                m_GemsAreAnimating = false;
            }
        }

        private void OnPlayerTurn()
        {
            m_Grid.CheckMatch();
        }

        public Vector2 CalculateSpacing()
        {
            return
                new Vector2(
                    m_RectTransform.rect.width / (grid.size.x - 1),
                    m_RectTransform.rect.height / (grid.size.y - 1));
        }

        public static void Init()
        {
            Grid.onCreate.AddListener(OnGridCreate);
        }

        private static void OnGridCreate(Grid newGrid)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.SetParent(CombatManager.self.gridParentRectTransform, false);

            var newGridMono = newGameObject.AddComponent<GridMono>();

            newGridMono.m_Grid = newGrid;
            newGrid.components.Add(newGridMono);

            newGridMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();
            newGridMono.m_ParentRectTransform =
                newGameObject.GetComponentsInParent<RectTransform>().First(
                    rectTransform => rectTransform != newGridMono.m_RectTransform);

            newGridMono.m_RectTransform.anchorMin = new Vector2(0.1f, 0.1f);
            newGridMono.m_RectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            newGridMono.m_RectTransform.sizeDelta = Vector2.zero;

            newGridMono.m_RectTransform.anchoredPosition = Vector2.zero;

            CombatManager.self.onCombatUpdate.AddListener(newGridMono.OnCombatUpdate);
            CombatManager.self.onPlayerTurn.AddListener(newGridMono.OnPlayerTurn);
        }
    }
}
