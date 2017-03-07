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

        private Vector2 m_CurrentPosition;
        private float m_MoveToPositionTime = 1f;
        private IEnumerator m_MoveToPositionCoroutine;

        private GameObject m_DuplicateImage;
        private IEnumerator m_UpdatePositionCoroutine;

        public RectTransform rectTransform { get { return m_RectTransform; } }

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
            get { return row.GetComponent<GridCollectionMono>(); }
        }

        public Column column { get { return m_Gem.column; } }
        public GridCollectionMono columnMono
        {
            get { return column.GetComponent<GridCollectionMono>(); }
        }

        public Vector2 currentPosition
        {
            get { return m_CurrentPosition; }
            set { m_CurrentPosition = value; }
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

        private void OnCombatUpdate()
        {
            if (m_MoveToPositionCoroutine != null && m_MoveToPositionCoroutine.MoveNext()) ;
            else { m_MoveToPositionCoroutine = null; }

            if (m_UpdatePositionCoroutine != null && m_UpdatePositionCoroutine.MoveNext()) ;
            else { m_UpdatePositionCoroutine = null; }
        }

        public void UpdatePosition()
        {
            StartCoroutine(OnUpdatePosition());
        }

        public Vector2 CalculatePosition(Vector2 newPosition)
        {
            newPosition = grid.ClampPosition(newPosition);

            var spacing = gridMono.CalculateSpacing();
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

            var spacing = gridMono.CalculateSpacing();
            var moveToPosition =
                new Vector2(
                    positionChangeInfo.newPosition.x * spacing.x,
                    positionChangeInfo.newPosition.y * spacing.y);

            m_MoveToPositionCoroutine = MoveToPosition(moveToPosition);
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
                    m_UpdatePositionCoroutine = OnUpdatePosition();

                yield return null;
            }

            if (m_UpdatePositionCoroutine == null)
                m_UpdatePositionCoroutine = OnUpdatePosition();

            m_MoveToPositionCoroutine = null;
        }

        private IEnumerator OnUpdatePosition()
        {
            yield return null;

            var spacing = gridMono.CalculateSpacing();

            var nextPosition =
                grid.ClampPosition(
                    position + rowMono.currentDirection + columnMono.currentDirection);

            if (nextPosition != position + rowMono.currentDirection + columnMono.currentDirection)
            {
                if (m_DuplicateImage == null)
                {
                    m_DuplicateImage = new GameObject();
                    m_DuplicateImage.transform.SetParent(transform.parent, false);

                    var duplicateSpriteRenderer = m_DuplicateImage.AddComponent<Image>();
                    var duplicateRectTransform = m_DuplicateImage.GetComponent<RectTransform>();

                    duplicateSpriteRenderer.sprite = m_Image.sprite;

                    duplicateRectTransform.anchorMin = Vector2.zero;
                    duplicateRectTransform.anchorMax = Vector2.zero;
                    duplicateRectTransform.sizeDelta = Vector2.zero;

                    duplicateRectTransform.anchoredPosition = m_CurrentPosition;

                    duplicateSpriteRenderer.SetNativeSize();
                    duplicateRectTransform.sizeDelta = duplicateRectTransform.sizeDelta * 1.5f;
                }

                m_DuplicateImage.GetComponent<RectTransform>().anchoredPosition =
                    CalculatePosition(position) + rowMono.positionOffset + columnMono.positionOffset;

                m_CurrentPosition = nextPosition - rowMono.currentDirection - columnMono.currentDirection;
                m_CurrentPosition =
                    new Vector2(m_CurrentPosition.x * spacing.x, m_CurrentPosition.y * spacing.y);
            }
            else if (m_DuplicateImage != null)
            {
                Destroy(m_DuplicateImage.gameObject);
                m_CurrentPosition = CalculatePosition(position);
            }

            m_RectTransform.anchoredPosition =
                m_CurrentPosition + columnMono.positionOffset + rowMono.positionOffset;

            m_UpdatePositionCoroutine = null;
        }

        public static void Init()
        {
            Gem.onCreate.AddListener(OnCreateGem);
        }

        public static void OnCreateGem(Gem newGem)
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

            CombatManager.self.onCombatUpdate.AddListener(newGemMono.OnCombatUpdate);
        }
    }
}
