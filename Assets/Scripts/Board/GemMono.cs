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
        // Use this for initialization
        private void Awake()
        {

        }

        private void OnTypeChange(TypeChangeInformation typeChangeInfo)
        {
            //TODO: Update sprite based on new type
        }
        private void OnPositionChange(PositionChangeInformation positionChangeInfo)
        {
            //TODO: Animate to new position
        }


        private void OnMatch(MatchInformation matchInfo)
        {
            //TODO: Check if this gem matched and play an animation if so. Delete afterwards
        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {
            //TODO: Check to see if this gem was changed in the grid
        }

        public static GemMono Create(GemType gemType, Vector2 position)
        {
            var newGemMono = new GameObject().AddComponent<GemMono>();
            newGemMono.gem = new Gem { gemType = gemType, position = position };

            return newGemMono;
        }
    }
}
