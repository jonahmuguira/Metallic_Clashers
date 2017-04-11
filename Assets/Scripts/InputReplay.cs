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

using Random = UnityEngine.Random;

public class InputReplay : MonoSingleton<InputReplay>
{
    [SerializeField]
    private bool m_Replay;
    [SerializeField]
    private bool m_DrawData;

    private InputData m_InputData;

    protected override void OnAwake()
    {
        var jsonData = File.ReadAllText(Application.persistentDataPath + "/InputData.json");
        m_InputData = JsonUtility.FromJson<InputData>(jsonData);

        if (!m_Replay)
            return;

        Random.InitState(m_InputData.randomSeed);
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

    private void OnGUI()
    {
        if (m_DrawData)
            DrawData();
    }

    private void DrawData()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        {
            GL.MultMatrix(transform.localToWorldMatrix);

            GL.Begin(GL.QUADS);
            {
                foreach (var dragAction in m_InputData.dragActions.Where(action => action.inputType == InputType.End))
                {
                    var origin = dragAction.dragInformation.origin;
                    origin.y = Screen.height - origin.y;

                    var end = dragAction.dragInformation.end;
                    end.y = Screen.height - end.y;

                    var direction = new Vector2(end.x - origin.x, end.y - origin.y).normalized;

                    GL.Color(Color.green);
                    GL.Vertex3(origin.x, origin.y, 0f);
                    GL.Vertex3(origin.x + 15f * direction.y, origin.y + 15f * -direction.x, 0f);

                    GL.Color(Color.red);
                    GL.Vertex3(end.x + 15f * direction.y, end.y + 15f * -direction.x, 0f);
                    GL.Vertex3(end.x, end.y, 0f);
                    
                }
            }
            GL.End();
        }
        GL.PopMatrix();
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

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}
