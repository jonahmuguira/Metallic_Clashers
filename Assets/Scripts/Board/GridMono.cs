using UnityEngine;

namespace Board
{
    using System.Linq;

    using Information;

    using UnityEngine.Events;

    public class GridResizeEvent : UnityEvent<GridResizeInformation> { }

    [RequireComponent(typeof(RectTransform))]
    public class GridMono : MonoBehaviour
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

        public Grid grid { get { return m_Grid; } }

        public RectTransform rectTransform { get { return m_RectTransform; } }

        public GridResizeEvent onGridResize { get { return m_OnGridResize; } }

        private void LateUpdate()
        {
            var currentRectSize = new Vector2(m_RectTransform.rect.width, m_RectTransform.rect.height);
            if (currentRectSize == m_PreviousRectSize)
                return;

            m_OnGridResize.Invoke(new GridResizeInformation { newRect = m_RectTransform.rect });

            m_PreviousRectSize = currentRectSize;
        }

        public static GridMono Create(RectTransform parentTransform, Vector2 newSize)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.SetParent(parentTransform);

            var newGridMono = newGameObject.AddComponent<GridMono>();

            newGridMono.m_Grid = new Grid(newGridMono, newSize);

            newGridMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();
            newGridMono.m_ParentRectTransform =
                newGameObject.GetComponentsInParent<RectTransform>().First(
                    rectTransform => rectTransform != newGridMono.m_RectTransform);

            newGridMono.m_RectTransform.anchorMin = Vector2.zero;
            newGridMono.m_RectTransform.anchorMax = Vector2.one;
            newGridMono.m_RectTransform.sizeDelta = new Vector2(-60f, -60f);

            newGridMono.m_RectTransform.anchoredPosition = Vector2.zero;
            newGridMono.m_Grid.InitializeGrid();

            return newGridMono;
        }
    }
}
