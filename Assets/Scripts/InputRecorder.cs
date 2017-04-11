using System;
using System.Collections.Generic;
using System.IO;

using CustomInput;
using CustomInput.Information;

using Library;

using UnityEngine;

public enum InputType { Start, While, End, }
[Serializable]
public class InputAction
{
    public float time;
    public InputType inputType;
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
}

public class InputRecorder : MonoSingleton<InputRecorder>
{
    [SerializeField]
    private bool m_Record;

    private InputData m_InputData = new InputData();

    protected override void OnAwake()
    {
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
                touchInformation = touchInformation,
                inputType = InputType.Start,
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
                touchInformation = touchInformation,
                inputType = InputType.While,
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
                touchInformation = touchInformation,
                inputType = InputType.End,
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
                dragInformation = dragInformation,
                inputType = InputType.Start,
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
                dragInformation = dragInformation,
                inputType = InputType.While,
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
                dragInformation = dragInformation,
                inputType = InputType.End,
            });
    }

    [ContextMenu("Delete Data")]
    private void DeleteData()
    {
        File.Delete(Application.persistentDataPath + "/InputData.txt");
    }
}
