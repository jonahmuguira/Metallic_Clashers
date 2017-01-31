namespace Board
{
    using System;

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

            //TODO: Update sprite based on new type
            switch (typeChangeInfo.newType)
            {
            case GemType.Red:
                break;

            case GemType.Blue:
                break;

            case GemType.Green:
                break;

            case GemType.Yellow:
                break;

            case GemType.Purple:
                break;

            default:
                throw new ArgumentOutOfRangeException();
            }
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

        public static GemMono Create(Grid grid, GemType gemType, Vector2 position)
        {
            var newGemMono = new GameObject().AddComponent<GemMono>();

            grid.onMatch.AddListener(newGemMono.OnMatch);
            grid.onGridChange.AddListener(newGemMono.OnGridChange);

            newGemMono.gem = new Gem { gemType = gemType, position = position };

            newGemMono.gem.onTypeChange.AddListener(newGemMono.OnTypeChange);
            newGemMono.gem.onPositionChange.AddListener(newGemMono.OnPositionChange);

            return newGemMono;
        }
    }
}
