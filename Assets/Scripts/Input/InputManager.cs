namespace Input
{
    using UnityEngine;
    using Information;
    using UnityEngine.Events;

    public class OnPress : UnityEvent<TouchInformation> { }
    public class OnRelease : UnityEvent<TouchInformation> { }
    public class OnHold : UnityEvent<TouchInformation> { }
<<<<<<< HEAD
    public class OnSlide : UnityEvent<DragInformation> { }
=======
    public class OnDrag : UnityEvent<SlideInformation> { }
>>>>>>> 8ff3f4ee2891b9eb04c75a7d87f04720f95efdad

    public class InputManager : MonoBehaviour
    {
        public float dragDistance;
        public float dragDuration;
        public float holdDuration;

        private readonly OnPress m_OnPress = new OnPress();
        private readonly OnRelease m_OnRelease = new OnRelease();
        private readonly OnHold m_OnHold = new OnHold();
        private readonly OnDrag m_OnDrag = new OnDrag();

        private void Awake()
        {
            
        }

        private void Update()
        {
            
        }

        public OnPress onPress { get { return m_OnPress;} }
        public OnRelease onRelease { get { return m_OnRelease;} }
        public OnHold onHold { get { return m_OnHold;} }
        public OnDrag onDrag { get { return m_OnDrag;} }

    }
}