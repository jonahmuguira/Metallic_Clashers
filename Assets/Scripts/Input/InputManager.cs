namespace Input
{
    using UnityEngine;
    using Information;
    using UnityEngine.Events;

    public class OnPress : UnityEvent<TouchInformation> { }
    public class OnRelease : UnityEvent<TouchInformation> { }
    public class OnHold : UnityEvent<TouchInformation> { }
    public class OnSlide : UnityEvent<SlideInformation> { }

    public class InputManager : MonoBehaviour
    {
        public float slideDistance;
        public float slideDuration;

        public float holdDuration;

        public OnPress onPress;
        public OnRelease onRelease;
        public OnHold onHold;
        public OnSlide onSlide;

        private void Awake()
        {
            
        }

        private void Update()
        {
            
        }
    }
}