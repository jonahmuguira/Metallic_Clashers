using System;
using System.Collections.Generic;
using System.IO;

using CustomInput;
using CustomInput.Information;

using Library;

using UnityEngine;

using Random = UnityEngine.Random;

public enum InputType { Start, While, End, }
[Serializable]
public class InputAction
{
    public float time;
    public InputType inputType;

    public Random.State randomState;
}
[Serializable]
public class TouchAction : InputAction
{
    public TouchInformation touchInformation;
}
[Serializable]
public class DragAction : InputAction
{
    public DragInformation dragInformation;
}

[Serializable]
public class InputData
{
    public List<TouchAction> touchActions = new List<TouchAction>();
    public List<DragAction> dragActions = new List<DragAction>();

    public int randomSeed;
    public Random.State randomState;
}

public class InputRecorder : MonoSingleton<InputRecorder>
{
    [SerializeField]
    private bool m_Record;

    private InputData m_InputData = new InputData();

    protected override void OnAwake()
    {
        m_InputData.randomSeed = GameManager.self.randomSeed;
        m_InputData.randomState = Random.state;

        InputManager.self.onPress.AddListener(OnPress);
        InputManager.self.onHold.AddListener(OnHold);
        InputManager.self.onRelease.AddListener(OnRelease);

        InputManager.self.onBeginDrag.AddListener(OnBeginDrag);
        InputManager.self.onDrag.AddListener(OnDrag);
        InputManager.self.onEndDrag.AddListener(OnEndDrag);

        DontDestroyOnLoad(this);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        if (!m_Record)
            return;

        var jsonData = JsonUtility.ToJson(m_InputData);
        File.WriteAllText(Application.persistentDataPath + "/InputData.json", jsonData);
    }

    private void OnPress(TouchInformation touchInformation)
    {
        if (!m_Record)
            return;

        m_InputData.touchActions.Add(
            new TouchAction
            {
                time = Time.unscaledTime,
                inputType = InputType.Start,

                randomState = Random.state,

                touchInformation =
                    new TouchInformation
                    {
                        duration = touchInformation.duration,

                        position = ScalePosition(touchInformation.position),
                    },
            });
    }
    private void OnHold(TouchInformation touchInformation)
    {
        if (!m_Record)
            return;

        m_InputData.touchActions.Add(
            new TouchAction
            {
                time = Time.unscaledTime,
                inputType = InputType.While,

                randomState = Random.state,

                touchInformation =
                    new TouchInformation
                    {
                        duration = touchInformation.duration,

                        position = ScalePosition(touchInformation.position),
                    },
            });
    }
    private void OnRelease(TouchInformation touchInformation)
    {
        if (!m_Record)
            return;

        m_InputData.touchActions.Add(
            new TouchAction
            {
                time = Time.unscaledTime,
                inputType = InputType.End,

                randomState = Random.state,

                touchInformation =
                    new TouchInformation
                    {
                        duration = touchInformation.duration,

                        position = ScalePosition(touchInformation.position),
                    },
            });
    }

    private void OnBeginDrag(DragInformation dragInformation)
    {
        if (!m_Record)
            return;

        m_InputData.dragActions.Add(
            new DragAction
            {
                time = Time.unscaledTime,
                inputType = InputType.Start,

                randomState = Random.state,

                dragInformation =
                    new DragInformation
                    {
                        duration = dragInformation.duration,

                        origin = ScalePosition(dragInformation.origin),
                        end = ScalePosition(dragInformation.end),

                        delta = ScalePosition(dragInformation.delta),
                        totalDelta = ScalePosition(dragInformation.totalDelta),
                    },
            });
    }
    private void OnDrag(DragInformation dragInformation)
    {
        if (!m_Record)
            return;

        m_InputData.dragActions.Add(
            new DragAction
            {
                time = Time.unscaledTime,
                inputType = InputType.While,

                randomState = Random.state,

                dragInformation =
                    new DragInformation
                    {
                        duration = dragInformation.duration,

                        origin = ScalePosition(dragInformation.origin),
                        end = ScalePosition(dragInformation.end),

                        delta = ScalePosition(dragInformation.delta),
                        totalDelta = ScalePosition(dragInformation.totalDelta),
                    },
            });
    }
    private void OnEndDrag(DragInformation dragInformation)
    {
        if (!m_Record)
            return;

        m_InputData.dragActions.Add(
            new DragAction
            {
                time = Time.unscaledTime,
                inputType = InputType.End,

                randomState = Random.state,

                dragInformation =
                    new DragInformation
                    {
                        duration = dragInformation.duration,

                        origin = ScalePosition(dragInformation.origin),
                        end = ScalePosition(dragInformation.end),

                        delta = ScalePosition(dragInformation.delta),
                        totalDelta = ScalePosition(dragInformation.totalDelta),
                    },
            });
    }

    private Vector2 ScalePosition(Vector2 position)
    {
        position =
            new Vector2(
                position.x / Screen.width,
                position.y / Screen.height);

        return position;
    }

    [ContextMenu("Delete Data")]
    private void DeleteData()
    {
        File.Delete(Application.persistentDataPath + "/InputData.txt");
    }
}
