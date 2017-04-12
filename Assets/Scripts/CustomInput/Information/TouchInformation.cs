namespace CustomInput.Information
{
    using System;

    using UnityEngine;

    [Serializable]
    public class InputInformation
    {
        public float duration;
    }

    [Serializable]
    public class TouchInformation : InputInformation
    {
        public Vector2 position;
    }

    [Serializable]
    public class DragInformation : InputInformation
    {
        public Vector2 origin;
        public Vector2 end;

        /// <summary>
        /// The delta in mouse position this frame
        /// </summary>
        public Vector2 delta;

        /// <summary>
        /// The total delta in mouse position from the start of the drag
        /// </summary>
        public Vector2 totalDelta;
    }
}