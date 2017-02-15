using System.Collections;

namespace Board
{
    using System;
    using System.Linq;

    using UnityEngine;

    using Information;

    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class GemMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private Gem m_Gem;

        private Vector2 m_PositionOffset;
        private float m_ReducePositionOffsetTime = 0.5f;
        private Vector2 m_CurrentDirection;
        private Coroutine m_ReducePositionOffsetCoroutine;

        private Vector2 m_CurrentPosition;
        private float m_MoveToPositionTime = 1f;
        private Coroutine m_MoveToPositionCoroutine;

        private GemMono m_DuplicateGemMono;
        private Coroutine m_UpdatePositionCoroutine;

        public Gem gem
        {
            get { return m_Gem; }
            private set { m_Gem = value; }
        }

        public Grid grid { get { return m_Gem.grid; } }
        public GridMono gridMono { get { return grid.GetComponent<GridMono>(); } }

        public Vector2 position { get { return m_Gem.position; } set { m_Gem.position = value; } }
        public GemType gemType { get { return m_Gem.gemType; } set { m_Gem.gemType = value; } }

        public Row row { get { return m_Gem.row; } }
        public GridCollectionMono rowMono
        {
            get { return m_Gem.row.GetComponent<GridCollectionMono>(); }
        }

        public Column column { get { return m_Gem.column; } }
        public GridCollectionMono columnMono
        {
            get { return m_Gem.column.GetComponent<GridCollectionMono>(); }
        }

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
                if (m_UpdatePositionCoroutine == null)
                    m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

                m_ReducePositionOffsetCoroutine = StartCoroutine(ReducePositionOffset());
            }
        }

        private void Start()
        {
            // Now that everything should be initialized, we can show the gem visually
            m_Image.enabled = true;

            m_CurrentPosition =
                new Vector2(
                    m_Gem.position.x
                    * (gridMono.rectTransform.rect.width
                    / (grid.size.x - 1)),

                    gridMono.rectTransform.rect.height);

            m_RectTransform.anchoredPosition = m_CurrentPosition;

            OnPositionChange(new PositionChangeInformation { gem = gem, newPosition = gem.position });
        }

        private void CheckPositionOffset()
        {
            var spacing = CalculateSpacing();
            if (Mathf.Abs(m_PositionOffset.x) < spacing.x && Mathf.Abs(m_PositionOffset.y) < spacing.y)
                return;

            var gridCollection =
                m_CurrentDirection == Vector2.left || m_CurrentDirection == Vector2.right
                    ? row as GridCollection : column as GridCollection;

            var gemMonos = gridCollection.gems.Select(rowGem => rowGem.GetComponent<GemMono>()).ToList();

            foreach (var gemMono in gemMonos)
            {
                var newPosition = gemMono.CalculatePosition(gemMono.position + gemMono.m_CurrentDirection);

                gemMono.m_CurrentPosition = newPosition;
                gemMono.m_RectTransform.anchoredPosition = newPosition;

                gemMono.m_PositionOffset = Vector2.zero;
            }

            gridCollection.Slide(
                m_CurrentDirection == Vector2.right || m_CurrentDirection == Vector2.up
                    ? SlideDirection.Backward : SlideDirection.Forward);
        }

        private Vector2 CalculateSpacing()
        {
            return
                new Vector2(
                    gridMono.rectTransform.rect.width / (grid.size.x - 1),
                    gridMono.rectTransform.rect.height / (grid.size.y - 1));
        }

        private Vector2 CalculatePosition(Vector2 newPosition)
        {
            newPosition = grid.ClampPosition(newPosition);

            var spacing = CalculateSpacing();
            return new Vector2(newPosition.x * spacing.x, newPosition.y * spacing.y);
        }

        private void OnTypeChange(TypeChangeInformation typeChangeInfo)
        {
            // This should not happen because we only subscribe to the gem we care about but still...
            if (typeChangeInfo.gem != gem)
                return;

            var spriteIndex = (int)typeChangeInfo.newType;

            m_Image.sprite = CombatManager.self.gemSprites[spriteIndex];
        }
        private void OnPositionChange(PositionChangeInformation positionChangeInfo)
        {
            if (m_MoveToPositionCoroutine != null)
                StopCoroutine(m_MoveToPositionCoroutine);

            name = "Gem " + positionChangeInfo.newPosition;

            var spacing = CalculateSpacing();
            var moveToPosition =
                new Vector2(
                    positionChangeInfo.newPosition.x * spacing.x,
                    positionChangeInfo.newPosition.y * spacing.y);

            m_MoveToPositionCoroutine = StartCoroutine(MoveToPosition(moveToPosition));
        }

        private void OnMatch(MatchInformation matchInfo)
        {
            //TODO: Check if this gem matched and play an animation if so. Delete afterwards
        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {
            //TODO: Check to see if this gem was changed in the grid
        }

        private IEnumerator MoveToPosition(Vector2 newPosition)
        {
            var deltaTime = 0f;
            while (deltaTime < m_MoveToPositionTime)
            {
                m_CurrentPosition =
                    Vector2.Lerp(
                        m_CurrentPosition,
                        newPosition,
                        deltaTime / m_MoveToPositionTime);

                deltaTime += Time.deltaTime;

                if (m_UpdatePositionCoroutine == null)
                    m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

                yield return null;
            }

            if (m_UpdatePositionCoroutine == null)
                m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

            m_MoveToPositionCoroutine = null;
        }

        private IEnumerator ReducePositionOffset()
        {
            CheckPositionOffset();

            yield return null;

            var deltaTime = 0f;
            while (m_PositionOffset != Vector2.zero && deltaTime < m_ReducePositionOffsetTime)
            {
                var spacing = CalculateSpacing();

                if (Mathf.Abs(m_PositionOffset.x) > spacing.x / 2f ||
                    Mathf.Abs(m_PositionOffset.y) > spacing.y / 2f)
                {
                    var newOffset = new Vector2(spacing.x * m_CurrentDirection.x, spacing.y * m_CurrentDirection.y);
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, newOffset, deltaTime / m_ReducePositionOffsetTime);
                }
                else
                {
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, Vector2.zero, deltaTime / m_ReducePositionOffsetTime);
                }

                deltaTime += Time.deltaTime;

                CheckPositionOffset();

                if (m_UpdatePositionCoroutine == null)
                    m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

                yield return null;
            }
            m_CurrentDirection = Vector2.zero;

            if (m_UpdatePositionCoroutine == null)
                m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

            m_ReducePositionOffsetCoroutine = null;
        }

        private IEnumerator UpdatePosition()
        {
            yield return new WaitForEndOfFrame();

            var spacing = CalculateSpacing();
            var coefficient =
                Mathf.Abs(m_PositionOffset.x) > Mathf.Abs(m_PositionOffset.y)
                    ? Mathf.Abs(m_PositionOffset.x / spacing.x)
                    : Mathf.Abs(m_PositionOffset.y / spacing.y);

            var nextPosition = grid.ClampPosition(position + m_CurrentDirection);
            if (nextPosition != position + m_CurrentDirection)
            {
                if (m_DuplicateGemMono == null)
                {
                    m_DuplicateGemMono = Create(gem);
                    m_DuplicateGemMono.StopAllCoroutines();

                    m_DuplicateGemMono.m_CurrentPosition = m_CurrentPosition;
                }

                var dupNextPosition = m_DuplicateGemMono.position + m_CurrentDirection;
                dupNextPosition =
                    new Vector2(dupNextPosition.x * spacing.x, dupNextPosition.y * spacing.y);
                Debug.Log(dupNextPosition);

                m_DuplicateGemMono.m_RectTransform.anchoredPosition =
                    Vector2.Lerp(m_DuplicateGemMono.m_CurrentPosition, dupNextPosition, coefficient);

                m_CurrentPosition = nextPosition - m_CurrentDirection;
                m_CurrentPosition =
                    new Vector2(m_CurrentPosition.x * spacing.x, m_CurrentPosition.y * spacing.y);

                //nextPosition = Vector2.zero;
                //transform.SetAsFirstSibling();
                //m_Image.color =
                //    new Color(
                //        m_Image.color.r,
                //        m_Image.color.g,
                //        m_Image.color.b,
                //        Mathf.Max(
                //            Mathf.Pow(coefficient, 1.1f),
                //            1f - Mathf.Pow(coefficient, 1.1f)));
            }
            else
            {
                if (m_DuplicateGemMono != null)
                {
                    Destroy(m_DuplicateGemMono.gameObject);
                    m_CurrentPosition = CalculatePosition(position);
                }
                //m_Image.color =
                //    new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1f);
            }

            nextPosition = new Vector2(nextPosition.x * spacing.x, nextPosition.y * spacing.y);

            m_RectTransform.anchoredPosition =
                Vector2.Lerp(m_CurrentPosition, nextPosition, coefficient);

            m_UpdatePositionCoroutine = null;
        }

        public static void Init()
        {
            Gem.onCreate.AddListener(gem => Create(gem));
        }

        public static GemMono Create(Gem newGem)
        {
            var newGameObject = new GameObject();
            var newGemMono = newGameObject.AddComponent<GemMono>();

            newGemMono.m_Image = newGameObject.GetComponent<Image>();
            // Hide the gem visually for now
            newGemMono.m_Image.enabled = false;

            newGemMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();

            newGemMono.m_RectTransform.anchorMin = Vector2.zero;
            newGemMono.m_RectTransform.anchorMax = Vector2.zero;
            newGemMono.m_RectTransform.sizeDelta = Vector2.zero;

            newGemMono.m_RectTransform.anchoredPosition = Vector2.zero;

            newGemMono.m_CurrentPosition = newGemMono.m_RectTransform.anchoredPosition;

            newGemMono.gem = newGem;
            newGem.components.Add(newGemMono);

            // Subscribe to the relevant events
            newGemMono.gem.onTypeChange.AddListener(newGemMono.OnTypeChange);
            newGemMono.gem.onPositionChange.AddListener(newGemMono.OnPositionChange);

            var gridMono = newGemMono.gridMono;

            gridMono.grid.onMatch.AddListener(newGemMono.OnMatch);
            gridMono.grid.onGridChange.AddListener(newGemMono.OnGridChange);

            // Parent the object before moving it
            newGameObject.transform.SetParent(gridMono.transform, false);

            newGemMono.OnPositionChange(
                new PositionChangeInformation
                {
                    gem = newGemMono.gem,
                    newPosition = newGemMono.gem.position,
                });
            newGemMono.OnTypeChange(
                new TypeChangeInformation
                {
                    gem = newGemMono.gem,
                    newType = newGemMono.gem.gemType,
                });

            gridMono.onGridResize.AddListener(
                empty => newGemMono.OnPositionChange(
                    new PositionChangeInformation
                    {
                        gem = newGemMono.gem,
                        newPosition = newGemMono.gem.position
                    }));

            newGemMono.m_Image.SetNativeSize();

            newGemMono.m_RectTransform.sizeDelta = newGemMono.m_RectTransform.sizeDelta * 1.5f;

            return newGemMono;
        }
    }
}
