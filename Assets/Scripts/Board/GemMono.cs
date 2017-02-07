using System.Collections;

namespace Board
{
    using System;

    using UnityEngine;

    using Information;

    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class GemMono : MonoBehaviour
    {
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private Gem m_Gem;

        private Vector3 m_PositionOffset;
        private float m_ReducePositionOffsetTime = 1f;
        private Coroutine m_ReducePositionOffsetCoroutine;

        private Vector3 m_CurrentPosition;
        private float m_MoveToPositionTime = 1f;
        private Coroutine m_MoveToPositionCoroutine;

        private Coroutine m_UpdatePositionCoroutine;

        public Gem gem
        {
            get { return m_Gem; }
            private set { m_Gem = value; }
        }

        public Vector3 positionOffset
        {
            get { return m_PositionOffset; }
            set
            {
                m_PositionOffset = value;

                if (m_ReducePositionOffsetCoroutine != null)
                    StopCoroutine(m_ReducePositionOffsetCoroutine);
                if (m_UpdatePositionCoroutine == null)
                    m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

                m_ReducePositionOffsetCoroutine = StartCoroutine(ReducePositionOffset());
            }
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

            m_MoveToPositionCoroutine =
                StartCoroutine(
                    MoveToPosition(
                        new Vector2(
                            positionChangeInfo.newPosition.x * CombatManager.self.gemOffset.x,
                            positionChangeInfo.newPosition.y * CombatManager.self.gemOffset.y)));
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
            yield return null;

            while (m_PositionOffset != Vector3.zero)
            {
                m_PositionOffset =
                    Vector2.MoveTowards(m_PositionOffset, Vector2.zero, 100f * Time.deltaTime);

                if (m_UpdatePositionCoroutine == null)
                    m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

                yield return null;
            }

            if (m_UpdatePositionCoroutine == null)
                m_UpdatePositionCoroutine = StartCoroutine(UpdatePosition());

            m_ReducePositionOffsetCoroutine = null;
        }

        private IEnumerator UpdatePosition()
        {
            yield return new WaitForEndOfFrame();

            m_RectTransform.anchoredPosition = m_CurrentPosition + m_PositionOffset;

            m_UpdatePositionCoroutine = null;
        }

        public static GemMono Create(Grid grid, GemType gemType, Vector2 position)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.SetParent(FindObjectOfType<Canvas>().transform);

            var newGemMono = newGameObject.AddComponent<GemMono>();
            newGemMono.name = position.ToString();

            grid.onMatch.AddListener(newGemMono.OnMatch);
            grid.onGridChange.AddListener(newGemMono.OnGridChange);

            newGemMono.m_Image = newGameObject.GetComponent<Image>();
            newGemMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();

            newGemMono.m_RectTransform.anchoredPosition =
                new Vector2(
                    position.x * CombatManager.self.gemOffset.x,
                    grid.size.y * CombatManager.self.gemOffset.y);

            newGemMono.gem = new Gem();

            // Subscribe to the relevant events before setting the values
            newGemMono.gem.onTypeChange.AddListener(newGemMono.OnTypeChange);
            newGemMono.gem.onPositionChange.AddListener(newGemMono.OnPositionChange);

            newGemMono.gem.grid = grid;

            newGemMono.gem.gemType = gemType;
            newGemMono.gem.position = position;

            var newBoxCollider = newGameObject.AddComponent<BoxCollider2D>();
            newBoxCollider.isTrigger = true;

            newGemMono.m_Image.SetNativeSize();

            return newGemMono;
        }
    }
}
