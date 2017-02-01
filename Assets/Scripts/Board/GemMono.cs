namespace Board
{
    using UnityEngine;

    using Information;

    [RequireComponent(typeof(SpriteRenderer))]
    public class GemMono : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer;

        [SerializeField]
        private Gem m_Gem;

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

            m_SpriteRenderer.sprite = CombatManager.self.gemSprites[spriteIndex];
        }
        private void OnPositionChange(PositionChangeInformation positionChangeInfo)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_Gem.position, 0.5f);
        }

        private void OnMatch(MatchInformation matchInfo)
        {
            //TODO: Check if this gem matched and play an animation if so. Delete afterwards
        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {
            //TODO: Check to see if this gem was changed in the grid
        }

        public static GemMono Create(Grid grid, GemType gemType, Vector2 position)
        {
            var newGemMono = new GameObject().AddComponent<GemMono>();

            grid.onMatch.AddListener(newGemMono.OnMatch);
            grid.onGridChange.AddListener(newGemMono.OnGridChange);

            newGemMono.m_SpriteRenderer = newGemMono.GetComponent<SpriteRenderer>();

            newGemMono.gem = new Gem();

            // Subscribe to the relevant events before setting the values
            newGemMono.gem.onTypeChange.AddListener(newGemMono.OnTypeChange);
            newGemMono.gem.onPositionChange.AddListener(newGemMono.OnPositionChange);

            newGemMono.gem.grid = grid;

            newGemMono.gem.gemType = gemType;
            newGemMono.gem.position = position;

            return newGemMono;
        }
    }
}
