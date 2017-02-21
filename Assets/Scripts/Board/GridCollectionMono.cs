using UnityEngine;

namespace Board
{
    using System.Collections;
    using System.Linq;

    [RequireComponent(typeof(RectTransform))]
    public class GridCollectionMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private GridCollection m_GridCollection;

        public GridCollection gridCollection { get { return m_GridCollection; } }

        private Vector2 m_PositionOffset;
        private Vector2 m_CurrentDirection;
        private float m_ReducePositionOffsetTime = 1f;
        private Coroutine m_ReducePositionOffsetCoroutine;

        public Grid grid { get { return m_GridCollection.grid; } }
        public GridMono gridMono { get { return grid.GetComponent<GridMono>(); } }

        public Vector2 positionOffset
        {
            get { return m_PositionOffset; }
            set
            {
                m_PositionOffset = value;
                m_CurrentDirection =
                    Mathf.Abs(m_PositionOffset.x) > Mathf.Abs(m_PositionOffset.y)
                    ? m_PositionOffset.x > 0f
                        ? Vector2.right : Vector2.left
                    : m_PositionOffset.y > 0f
                        ? Vector2.up : Vector2.down;

                if (m_ReducePositionOffsetCoroutine != null)
                    StopCoroutine(m_ReducePositionOffsetCoroutine);

                m_ReducePositionOffsetCoroutine = StartCoroutine(ReducePositionOffset());
            }
        }
        public Vector2 currentDirection { get { return m_CurrentDirection; } }

        private void CheckForSlide()
        {
            var spacing = gridMono.CalculateSpacing();
            if (Mathf.Abs(m_PositionOffset.x) < spacing.x && Mathf.Abs(m_PositionOffset.y) < spacing.y)
                return;

            var gemMonos = gridCollection.gems.Select(rowGem => rowGem.GetComponent<GemMono>()).ToList();

            foreach (var gemMono in gemMonos)
            {
                var newPosition = gemMono.CalculatePosition(gemMono.position + m_CurrentDirection);

                gemMono.currentPosition = newPosition;
                gemMono.rectTransform.anchoredPosition = newPosition;
            }

            gridCollection.Slide(
                m_CurrentDirection == Vector2.right || m_CurrentDirection == Vector2.up
                    ? SlideDirection.Backward : SlideDirection.Forward);

            m_PositionOffset = Vector2.zero;
            m_CurrentDirection = Vector2.zero;
        }
        private IEnumerator ReducePositionOffset()
        {
            CheckForSlide();

            foreach (var gem in gridCollection.gems)
            {
                var gemMono = gem.GetComponent<GemMono>();
                gemMono.UpdatePosition();
            }

            yield return null;

            var deltaTime = 0f;
            while (deltaTime < m_ReducePositionOffsetTime)
            {
                var spacing = gridMono.CalculateSpacing();

                if (Mathf.Abs(m_PositionOffset.x) > spacing.x / 2f ||
                    Mathf.Abs(m_PositionOffset.y) > spacing.y / 2f)
                {
                    var newOffset =
                        new Vector2(
                            spacing.x * m_CurrentDirection.x,
                            spacing.y * m_CurrentDirection.y);
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, newOffset, deltaTime / m_ReducePositionOffsetTime);
                }
                else
                {
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, Vector2.zero, deltaTime / m_ReducePositionOffsetTime);
                }

                CheckForSlide();

                foreach (var gem in gridCollection.gems)
                {
                    var gemMono = gem.GetComponent<GemMono>();
                    gemMono.UpdatePosition();
                }

                deltaTime += Time.deltaTime;

                yield return null;
            }

            m_PositionOffset = Vector2.zero;
            m_CurrentDirection = Vector2.zero;

            foreach (var gem in gridCollection.gems)
            {
                var gemMono = gem.GetComponent<GemMono>();
                gemMono.UpdatePosition();
            }
        }

        public static void Init()
        {
            GridCollection.onCreate.AddListener(OnCreateGridCollection);
        }

        private static void OnCreateGridCollection(GridCollection newGridCollection)
        {
            var gridMono =
                newGridCollection.grid.components.First(component => component is GridMono) as GridMono;

            if (gridMono == null)
                return;

            var newGameObject = new GameObject(newGridCollection.GetType() + " " + newGridCollection.index);
            newGameObject.transform.SetParent(gridMono.transform, false);

            var newGridCollectionMono = newGameObject.AddComponent<GridCollectionMono>();

            newGridCollectionMono.m_GridCollection = newGridCollection;
            newGridCollection.components.Add(newGridCollectionMono);
        }
    }
}
