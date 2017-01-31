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
            m_Gem.onPositionChange.AddListener(OnPositionChange);
            m_Gem.onTypeChange.AddListener(OnTypeChange);
        }

        public GemMono()
        { }

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

        }
        private void OnGridChange(GridChangeInformation gridChangeInfo)
        {

        }
    }
}
