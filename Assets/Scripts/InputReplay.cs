using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CustomInput;
using CustomInput.Information;

using Library;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputReplay : MonoSingleton<InputReplay>
{
    [SerializeField]
    private bool m_Replay;

    private InputData m_InputData;

    protected override void OnAwake()
    {
        var jsonData = File.ReadAllText(Application.persistentDataPath + "/InputData.json");
        m_InputData = JsonUtility.FromJson<InputData>(jsonData);
    }

    private void Update()
    {
        if (!m_Replay)
            return;

        if (!m_InputData.touchActions.Any() && !m_InputData.dragActions.Any())
            return;

        var currentTouchAction = m_InputData.touchActions.FirstOrDefault();
        var currentDragAction = m_InputData.dragActions.FirstOrDefault();

        if (currentTouchAction != null && Time.unscaledTime >= currentTouchAction.time)
        {
            ProcessTouch(currentTouchAction);
            m_InputData.touchActions.Remove(currentTouchAction);
        }

        if (currentDragAction != null && Time.unscaledTime >= currentDragAction.time)
        {
            ProcessDrag(currentDragAction);
            m_InputData.dragActions.Remove(currentDragAction);
        }
    }

    private static void ProcessTouch(TouchAction touchAction)
    {
        switch (touchAction.inputType)
        {
            case InputType.Start:
                InputManager.self.onPress.Invoke(touchAction.touchInformation);
                break;
            case InputType.While:
                InputManager.self.onHold.Invoke(touchAction.touchInformation);
                break;
            case InputType.End:
                InputManager.self.onRelease.Invoke(touchAction.touchInformation);
                PushButtons(touchAction.touchInformation.position);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private static void ProcessDrag(DragAction dragAction)
    {
        switch (dragAction.inputType)
        {
            case InputType.Start:
                InputManager.self.onBeginDrag.Invoke(dragAction.dragInformation);
                break;
            case InputType.While:
                InputManager.self.onDrag.Invoke(dragAction.dragInformation);
                break;
            case InputType.End:
                InputManager.self.onEndDrag.Invoke(dragAction.dragInformation);
                PushButtons(dragAction.dragInformation.end);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void PushButtons(Vector2 position)
    {
        var pointerEventData =
            new PointerEventData(EventSystem.current) { position = position };

        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, hits);

        if (!hits.Any())
            return;

        var buttons = hits.Select(hit => hit.gameObject.GetComponent<Button>());
        foreach (var button in buttons)
        {
            if (button != null)
                button.onClick.Invoke();
        }
    }
}
