using System.Collections;

namespace Board
{
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

        //private Vector2 m_

        private float m_MoveToPositionTime = 1f;
        private Coroutine m_MoveToPositionCoroutine;

        public Gem gem
        {
            get { return m_Gem; }
            private set { m_Gem = value; }
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
                StartCoroutine(MoveToPosition(positionChangeInfo.newPosition * 50f));
        }

        private void OnMatch(MatchInformation matchInfo)
        {
            //TODO: Check if this gem matched and play an animation if so. Delete afterwards
        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {
            //TODO: Check to see if this gem was changed in the grid
        }

        private IEnumerator MoveToPosition(Vector3 newPosition)
        {
            var deltaTime = 0f;
            while (deltaTime < m_MoveToPositionTime)
            {
                m_RectTransform.anchoredPosition =
                    Vector3.Lerp(
                        m_RectTransform.anchoredPosition,
                        newPosition,
                        deltaTime / m_MoveToPositionTime);

                deltaTime += Time.deltaTime;

                yield return null;
            }

            m_MoveToPositionCoroutine = null;
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
