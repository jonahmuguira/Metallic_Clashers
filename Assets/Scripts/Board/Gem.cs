namespace Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.Events;

    using Information;

    public interface IComponent { }

    [Serializable]
    public class PositionChangeEvent : UnityEvent<PositionChangeInformation> { }
    [Serializable]
    public class TypeChangeEvent : UnityEvent<TypeChangeInformation> { }
    [Serializable]
    public class CreateGemEvent : UnityEvent<Gem> { }

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
        #region SERIALIZED_FIELDS

        [SerializeField]
        private GemType m_GemType;

        [SerializeField]
        private Vector2 m_Position;

        [SerializeField]
        private TypeChangeEvent m_OnTypeChange = new TypeChangeEvent();
        [SerializeField]
        private PositionChangeEvent m_OnPositionChange = new PositionChangeEvent();

        #endregion

        private readonly List<IComponent> m_Components = new List<IComponent>();

        public static readonly CreateGemEvent onCreate = new CreateGemEvent();

        #region PUBLIC_PROPERTIES

        public Grid grid { get; private set; }

        public GemType gemType
        {
            get { return m_GemType; }
            set
            {
                m_GemType = value;

                m_OnTypeChange.Invoke(new TypeChangeInformation { gem = this, newType = value });
            }
        }

        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;

                m_OnPositionChange.Invoke(new PositionChangeInformation { gem = this, newPosition = value });
            }
        }

        public TypeChangeEvent onTypeChange { get { return m_OnTypeChange; } }
        public PositionChangeEvent onPositionChange { get { return m_OnPositionChange; } }

        public List<IComponent> components { get { return m_Components; } }

        public Row row { get { return grid.rows[(int)position.y]; } }
        public Column column { get { return grid.columns[(int)position.x]; } }

        #endregion

        private Gem() { }

        public Gem(Grid newGrid, Vector2 newPosition, GemType newGemType) : this()
        {
            grid = newGrid;

            m_Position = newPosition;
            m_GemType = newGemType;

            onCreate.Invoke(this);
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)components.First(component => component is T);
        }
    }
}
