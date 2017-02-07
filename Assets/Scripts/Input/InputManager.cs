namespace Input
{
    using System;

    using UnityEngine;
    using UnityEngine.Events;

    using Library;
    using Information;

    [Serializable]
    public class TouchEvent : UnityEvent<TouchInformation> { }

    [Serializable]
    public class DragEvent : UnityEvent<DragInformation> { }

    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField]
        private float m_DragDistance;
        [SerializeField]
        private float m_DragDuration;

        [SerializeField]
        private float m_HoldDuration;

        [SerializeField, Space]
        private TouchEvent m_OnPress = new TouchEvent();
        [SerializeField]
        private TouchEvent m_OnRelease = new TouchEvent();
        [SerializeField]
        private TouchEvent m_OnHold = new TouchEvent();

        [SerializeField]
        private DragEvent m_OnBeginDrag = new DragEvent();
        [SerializeField]
        private DragEvent m_OnDrag = new DragEvent();
        [SerializeField]
        private DragEvent m_OnEndDrag = new DragEvent();

        private bool m_MouseWasDown;
        private float m_CurrentHoldDuration;
        private Vector3 m_PreviousPosition;

        public float dragDistance { get { return m_DragDistance; } }
        public float dragDuration { get { return m_DragDuration; } }
        public float holdDuration { get { return m_HoldDuration; } }

        public TouchEvent onPress { get { return m_OnPress; } }
        public TouchEvent onRelease { get { return m_OnRelease; } }
        public TouchEvent onHold { get { return m_OnHold; } }

        public DragEvent onBeginDrag { get { return m_OnBeginDrag; } }
        public DragEvent onDrag { get { return m_OnDrag; } }
        public DragEvent onEndDrag { get { return m_OnEndDrag; } }

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_OnPress.Invoke(
                    new TouchInformation { duration = 0f, position = Input.mousePosition });

                m_PreviousPosition = Input.mousePosition;
                m_CurrentHoldDuration = 0f;
                m_MouseWasDown = true;
            }

            if (Input.GetMouseButton(0) && m_MouseWasDown)
            {
                m_OnHold.Invoke(
                    new TouchInformation
                    {
                        duration = m_CurrentHoldDuration,
                        position = Input.mousePosition
                    });

                m_CurrentHoldDuration += Time.deltaTime;
                m_MouseWasDown = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_CurrentHoldDuration < m_DragDuration &&
                    Vector3.Distance(m_PreviousPosition, Input.mousePosition) < m_DragDistance)
                {
                    m_OnRelease.Invoke(
                        new TouchInformation
                        {
                            duration = m_CurrentHoldDuration,
                            position = m_PreviousPosition
                        });
                }
                else if (Vector3.Distance(m_PreviousPosition, Input.mousePosition) >= m_DragDistance)
                {
                    m_OnDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PreviousPosition,
                            end = Input.mousePosition,
                            duration = m_CurrentHoldDuration
                        });
                }

                m_CurrentHoldDuration = 0f;
                m_MouseWasDown = false;
            }
        }
    }
}