using System.Collections;

namespace Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Information;

    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class GemMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Image m_BackgroundImage;
        [SerializeField]
        private Image m_MidgroundImage;
        [SerializeField]
        private Image m_ForegroundImage;

        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private Gem m_Gem;

        private Vector2 m_CurrentPosition;
        private float m_MoveToPositionTime = 1f;
        private IEnumerator m_MoveToPositionCoroutine;

        private bool m_PositionIsDirty;

        private GameObject m_DuplicateImage;

        private static List<GemMono> s_GemsCreatedThisFrame = new List<GemMono>();

        public RectTransform rectTransform { get { return m_RectTransform; } }

        public Gem gem { get { return m_Gem; } private set { m_Gem = value; } }

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
        public GridCollectionMono columnMono { get { return column.GetComponent<GridCollectionMono>(); } }

        public Vector2 currentPosition
        {
            get { return m_CurrentPosition; }
            set { m_CurrentPosition = value; }
        }

        public IEnumerator moveToPositionCoroutine { get { return m_MoveToPositionCoroutine; } }

        public bool positionIsDirty { get { return m_PositionIsDirty; } set { m_PositionIsDirty = value; } }

        private void Awake()
        {
            s_GemsCreatedThisFrame.Add(this);
        }

        private void Start()
        {
            var stackPosition = 0f;
            foreach (var gemMono in s_GemsCreatedThisFrame)
                if (position.x == gemMono.position.x)
                    if (position.y > gemMono.position.y)
                        stackPosition++;

            m_CurrentPosition =
                new Vector2(
                    m_Gem.position.x
                    * (gridMono.rectTransform.rect.width / (grid.size.x - 1)),

                    CombatManager.self.gridParentRectTransform.rect.height
                    + stackPosition
                    * (gridMono.rectTransform.rect.height / (grid.size.y - 1)));

            m_RectTransform.anchoredPosition = m_CurrentPosition;

            OnPositionChange(new PositionChangeInformation { gem = gem, newPosition = gem.position });

            // Now that everything should be initialized, we can show the gem visually
            m_BackgroundImage.enabled = true;
            m_ForegroundImage.enabled = true;
        }

        private void OnCombatUpdate()
        {
            s_GemsCreatedThisFrame.Remove(this);

            if (m_MoveToPositionCoroutine != null)
                if (!m_MoveToPositionCoroutine.MoveNext())
                    m_MoveToPositionCoroutine = null;
        }

        private void OnCombatLateUpdate()
        {
            if (m_PositionIsDirty)
                UpdatePosition();
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

            m_MidgroundImage.color = CombatManager.self.gemMonoInformation.colors[spriteIndex];
            m_ForegroundImage.color = CombatManager.self.gemMonoInformation.colors[spriteIndex];
        }
        private void OnPositionChange(PositionChangeInformation positionChangeInfo)
        {
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
            if (matchInfo.gems.Any(matchInfoGem => matchInfoGem == m_Gem))
                gridMono.gemMatchAnimations.Add(MatchAnimation());
        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {
            //TODO: Check to see if this gem was changed in the grid
        }

        private IEnumerator MoveToPosition(Vector2 newPosition)
        {
            var deltaTime = 0f;
            while (m_CurrentPosition != newPosition)
            {
                m_CurrentPosition =
                    Vector2.MoveTowards(m_CurrentPosition, newPosition, 40f * deltaTime);

                deltaTime += Time.deltaTime;

                m_PositionIsDirty = true;

                yield return null;
            }

            m_PositionIsDirty = true;

            m_MoveToPositionCoroutine = null;
        }

        private void UpdatePosition()
        {
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

                    var duplicateRectTransform = m_DuplicateImage.AddComponent<RectTransform>();

                    duplicateRectTransform.anchorMin = Vector2.zero;
                    duplicateRectTransform.anchorMax = Vector2.zero;
                    duplicateRectTransform.sizeDelta = Vector2.zero;

                    duplicateRectTransform.anchoredPosition = m_CurrentPosition;

                    var duplicateSpriteRenderer = m_DuplicateImage.AddComponent<Image>();
                    duplicateSpriteRenderer.sprite = m_BackgroundImage.sprite;
                    duplicateSpriteRenderer.color = m_BackgroundImage.color;
                    duplicateSpriteRenderer.SetNativeSize();

                    duplicateRectTransform.sizeDelta = m_RectTransform.sizeDelta;

                    var duplicateMidgroundGameObject = new GameObject();
                    var duplicateMidgroundRectTransform =
                        duplicateMidgroundGameObject.AddComponent<RectTransform>();
                    var duplicateMidgroundImage = duplicateMidgroundGameObject.AddComponent<Image>();

                    duplicateMidgroundImage.sprite = m_MidgroundImage.sprite;
                    duplicateMidgroundImage.color = m_MidgroundImage.color;
                    duplicateMidgroundImage.SetNativeSize();

                    duplicateMidgroundGameObject.transform.SetParent(m_DuplicateImage.transform, false);

                    duplicateMidgroundRectTransform.sizeDelta = m_RectTransform.sizeDelta;

                    var duplicateForegroundGameObject = new GameObject();
                    var duplicateForegroundRectTransform =
                        duplicateForegroundGameObject.AddComponent<RectTransform>();
                    var duplicateForegroundImage = duplicateForegroundGameObject.AddComponent<Image>();

                    duplicateForegroundImage.sprite = m_ForegroundImage.sprite;
                    duplicateForegroundImage.color = m_ForegroundImage.color;
                    duplicateForegroundImage.SetNativeSize();

                    duplicateForegroundGameObject.transform.SetParent(m_DuplicateImage.transform, false);

                    duplicateForegroundRectTransform.sizeDelta = m_RectTransform.sizeDelta;
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
        }

        private IEnumerator MatchAnimation()
        {
            var deltaTime = 0f;
            while (deltaTime < 1f)
            {
                m_BackgroundImage.color =
                    new Color(
                        m_BackgroundImage.color.r,
                        m_BackgroundImage.color.g,
                        m_BackgroundImage.color.b,
                        1 - deltaTime / 1f);

                m_MidgroundImage.color =
                    new Color(
                        m_MidgroundImage.color.r,
                        m_MidgroundImage.color.g,
                        m_MidgroundImage.color.b,
                        1 - deltaTime / 1f);

                m_ForegroundImage.color =
                    new Color(
                        m_ForegroundImage.color.r,
                        m_ForegroundImage.color.g,
                        m_ForegroundImage.color.b,
                        1 - deltaTime / 1f);

                deltaTime += Time.deltaTime;

                yield return null;
            }

            if (m_DuplicateImage != null)
                Destroy(m_DuplicateImage);

            Destroy(gameObject);
        }

        public static void Init()
        {
            Gem.onCreate.AddListener(OnCreateGem);
        }

        public static void OnCreateGem(Gem newGem)
        {
            var newGameObject = new GameObject();
            var newGemMono = newGameObject.AddComponent<GemMono>();

            newGemMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();

            newGemMono.m_RectTransform.anchorMin = Vector2.zero;
            newGemMono.m_RectTransform.anchorMax = Vector2.zero;
            newGemMono.m_RectTransform.sizeDelta = Vector2.zero;

            newGemMono.m_BackgroundImage = newGameObject.GetComponent<Image>();
            newGemMono.m_BackgroundImage.sprite = CombatManager.self.gemMonoInformation.backgroundImage;
            newGemMono.m_BackgroundImage.SetNativeSize();

            newGemMono.m_RectTransform.sizeDelta = newGemMono.m_RectTransform.sizeDelta / 8f;

            var midgroundGameObject = new GameObject();
            var midgroundRectTransform = midgroundGameObject.AddComponent<RectTransform>();

            newGemMono.m_MidgroundImage = midgroundGameObject.AddComponent<Image>();
            newGemMono.m_MidgroundImage.sprite = CombatManager.self.gemMonoInformation.midgroundImage;
            newGemMono.m_MidgroundImage.SetNativeSize();

            midgroundGameObject.transform.SetParent(newGemMono.transform, false);

            midgroundRectTransform.sizeDelta = newGemMono.m_RectTransform.sizeDelta;

            var foregroundGameObject = new GameObject();
            var foregroundRectTransform = foregroundGameObject.AddComponent<RectTransform>();

            newGemMono.m_ForegroundImage = foregroundGameObject.AddComponent<Image>();
            newGemMono.m_ForegroundImage.sprite = CombatManager.self.gemMonoInformation.foregroundImage;
            newGemMono.m_ForegroundImage.SetNativeSize();

            foregroundGameObject.transform.SetParent(newGemMono.transform, false);

            foregroundRectTransform.sizeDelta = newGemMono.m_RectTransform.sizeDelta;

            // Hide the gem visually for now
            newGemMono.m_BackgroundImage.enabled = false;
            newGemMono.m_ForegroundImage.enabled = false;

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

            CombatManager.self.onCombatUpdate.AddListener(newGemMono.OnCombatUpdate);
            CombatManager.self.onCombatLateUpdate.AddListener(newGemMono.OnCombatLateUpdate);
        }
    }
}
