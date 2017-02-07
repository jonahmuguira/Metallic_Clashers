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
        private float m_DragDeadzone;

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

        private float m_CurrentHoldDuration;
        private Vector2 m_PreviousPosition;

        private bool m_Dragging;

        public float dragDeadzone { get { return m_DragDeadzone; } }

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
                //Debug.Log("Press");

                m_OnPress.Invoke(
                    new TouchInformation { duration = 0f, position = Input.mousePosition });

                m_PreviousPosition = Input.mousePosition;
                m_CurrentHoldDuration = 0f;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!m_Dragging &&
                    Vector2.Distance(m_PreviousPosition, Input.mousePosition) >= m_DragDeadzone)
                {
                    //Debug.Log("Begin Drag");

                    m_OnBeginDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PreviousPosition,
                            end = Input.mousePosition,
                            duration = 0f,
                        });

                    m_Dragging = true;
                }
                else if (m_Dragging &&
                         Vector2.Distance(m_PreviousPosition, Input.mousePosition) >= m_DragDeadzone)
                {
                    //Debug.Log("Drag");

                    m_OnDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PreviousPosition,
                            end = Input.mousePosition,
                            duration = m_CurrentHoldDuration
                        });
                }
                else
                {
                    //Debug.Log("Hold");

                    m_OnHold.Invoke(
                        new TouchInformation
                        {
                            duration = m_CurrentHoldDuration,
                            position = Input.mousePosition
                        });
                }

                m_CurrentHoldDuration += Time.deltaTime;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!m_Dragging)
                {
                    //Debug.Log("Release");

                    m_OnRelease.Invoke(
                        new TouchInformation
                        {
                            duration = m_CurrentHoldDuration,
                            position = m_PreviousPosition
                        });
                }
                else
                {
                    //Debug.Log("End Drag");

                    m_OnEndDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PreviousPosition,
                            end = Input.mousePosition,
                            duration = m_CurrentHoldDuration
                        });
                }

                m_CurrentHoldDuration = 0f;
                m_Dragging = false;
            }
        }
    }
}