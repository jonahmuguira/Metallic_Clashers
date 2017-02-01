namespace Input
{
    using System;

    using UnityEngine;
    using UnityEngine.Events;
    
    using Library;
    using Information;

    [Serializable]
    public class OnPress : UnityEvent<TouchInformation> { }
    [Serializable]
    public class OnRelease : UnityEvent<TouchInformation> { }
    [Serializable]
    public class OnHold : UnityEvent<TouchInformation> { }
    [Serializable]
    public class OnDrag : UnityEvent<DragInformation> { }

    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField]
        private float m_DragDistance;
        [SerializeField]
        private float m_DragDuration;
        [SerializeField]
        private float m_HoldDuration;

        [SerializeField]
        private OnPress m_OnPress = new OnPress();
        [SerializeField]
        private OnRelease m_OnRelease = new OnRelease();
        [SerializeField]
        private OnHold m_OnHold = new OnHold();
        [SerializeField]
        private OnDrag m_OnDrag = new OnDrag();

        public float dragDistance { get { return m_DragDistance; } }
        public float dragDuration { get { return m_DragDuration; } }
        public float holdDuration { get { return m_HoldDuration; } }

        public OnPress onPress { get { return m_OnPress; } }
        public OnRelease onRelease { get { return m_OnRelease; } }
        public OnHold onHold { get { return m_OnHold; } }
        public OnDrag onDrag { get { return m_OnDrag; } }

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {

        }
    }
}