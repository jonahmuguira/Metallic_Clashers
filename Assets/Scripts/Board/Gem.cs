namespace Board
{
    using System;

    using UnityEngine;
    using UnityEngine.Events;

    using Information;

    [Serializable]
    public class OnPositionChange : UnityEvent<PositionChangeInformation> { }
    [Serializable]
    public class OnTypeChange : UnityEvent<TypeChangeInformation> { }

    public enum GemType
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
    }

    [Serializable]
    public class Gem
    {
        [SerializeField]
        private GemType m_GemType;

        [SerializeField]
        private Vector2 m_Position;

        [SerializeField]
        private readonly OnTypeChange m_OnTypeChange = new OnTypeChange();
        [SerializeField]
        private readonly OnPositionChange m_OnPositionChange = new OnPositionChange();

        public Grid grid { get; set; }

        public GemType gemType
        {
            get { return m_GemType; }
            set
            {
                if (m_GemType == value)
                    return;

                m_GemType = value;

                m_OnTypeChange.Invoke(new TypeChangeInformation { gem = this, newType = value });
            }
        }

        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                if (m_Position == value)
                    return;

                m_Position = value;

                m_OnPositionChange.Invoke(new PositionChangeInformation { gem = this, newPosition = value });
            }
        }

        public OnTypeChange onTypeChange { get { return m_OnTypeChange; } }
        public OnPositionChange onPositionChange { get { return m_OnPositionChange; } }
    }
}
