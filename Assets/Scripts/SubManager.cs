using Input;
using Input.Information;

using UnityEngine;

public abstract class SubManager : MonoBehaviour
{
    protected virtual void OnPress(TouchInformation touchInfo)
    {
        ////TODO: This will be the function that is called when a OnPress Event is called
    }

    protected virtual void OnRelease(TouchInformation touchInfo)
    {
        ////TODO: This will be the function that is called when a OnRelease Event is called
    }

    protected virtual void OnHold(TouchInformation touchInfo)
    {
        ////TODO: This will be the function that is called when a OnHold Event is called
    }

    protected virtual void OnDrag(DragInformation slideInfo)
    {
        ////TODO: This will be the function that is called when a OnSlide Event is called
    }

    private void Awake()
    {
        InputManager.self.onPress.AddListener(OnPress);
        InputManager.self.onRelease.AddListener(OnRelease);
        InputManager.self.onHold.AddListener(OnHold);
        InputManager.self.onDrag.AddListener(OnDrag);
    }
}
