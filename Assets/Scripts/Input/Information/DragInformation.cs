namespace Input.Information
{
    using System;

    using UnityEngine;

    [Serializable]
    public class DragInformation
    {
        public Vector2 origin;
        public Vector2 end;
        public float duration;
    }
}