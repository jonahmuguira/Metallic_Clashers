using Input.Information;

public abstract class SubManager
{
    private void Awake()
    {
        ////TODO: This is where the listeners for the Different events will be.
    }

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

    protected virtual void OnSlide(SlideInformation slideInfo)
    {
        ////TODO: This will be the function that is called when a OnSlide Event is called
    }
}
