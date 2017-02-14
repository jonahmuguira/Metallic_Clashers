using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

public enum AnimationType
{
    Rotate,
    Zoom,
    Pan
}

public enum TargetType
{
    Player,
    Enemy,
    Enemies,
    All
}

[Serializable]
public class TransformAnimation
{
    [SerializeField]
    private AnimationType m_AnimationType;
    [SerializeField]
    private TargetType m_TargetType;

    [SerializeField]
    private Vector3 m_PositionOffset;
    [SerializeField]
    private Vector3 m_RotationOffset;
    [SerializeField]
    private float m_ZoomOffset;

    [SerializeField]
    private AnimationCurve m_AnimationCurve;

    [SerializeField]
    private float m_Duration;

    [SerializeField]
    private float m_StartDelay;
    [SerializeField]
    private float m_EndDelay;

    [SerializeField]
    private Vector3 m_Magnitude;

    public AnimationType animationType
    {
        get { return m_AnimationType; }
    }
    public TargetType targetType
    {
        get { return m_TargetType; }
    }

    public Vector3 positionOffset
    {
        get { return m_PositionOffset; }
    }
    public Vector3 rotationOffset
    {
        get { return m_RotationOffset; }
    }
    public float zoomOffset
    {
        get { return m_ZoomOffset; }
    }

    public AnimationCurve animationCurve
    {
        get { return m_AnimationCurve; }
    }

    public float duration
    {
        get { return m_Duration; }
    }

    public float startDelay
    {
        get { return m_StartDelay; }
    }
    public float endDelay
    {
        get { return m_EndDelay; }
    }

    public Vector3 magnitude
    {
        get { return m_Magnitude; }
    }

    public IEnumerator Animate(Transform transform, List<Transform> targets)
    {
        var childTransform = transform.GetChild(0);

        transform.localPosition += m_PositionOffset;
        transform.eulerAngles += m_RotationOffset;
        childTransform.localPosition =
            new Vector3(
                childTransform.localPosition.x,
                childTransform.localPosition.y,
                childTransform.localPosition.z + m_ZoomOffset);

        //TODO: Do something based on TargetType
        PropertyInfo propertyInfo;
        switch (animationType)
        {
            case AnimationType.Rotate:
                propertyInfo = typeof(Transform).GetProperty("localEulerAngles");
                break;
            case AnimationType.Zoom:
                propertyInfo = typeof(Transform).GetProperty("localPosition");
                transform = childTransform;
                break;
            case AnimationType.Pan:
                propertyInfo = typeof(Transform).GetProperty("localPosition");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var originalVector = (Vector3)propertyInfo.GetValue(transform, null);

        if (m_StartDelay > 0f)
        {
            var startDelayTime = 0f;
            while (startDelayTime < m_StartDelay)
            {
                startDelayTime += Time.deltaTime;
                yield return null;
            }
        }

        var deltaTime = 0f;
        while (deltaTime < m_Duration)
        {
            propertyInfo.SetValue(
                transform,
                originalVector +
                    new Vector3(
                        m_Magnitude.x * m_AnimationCurve.Evaluate(deltaTime / m_Duration),
                        m_Magnitude.y * m_AnimationCurve.Evaluate(deltaTime / m_Duration),
                        m_Magnitude.z * m_AnimationCurve.Evaluate(deltaTime / m_Duration)), null);

            deltaTime += Time.deltaTime;
            yield return null;
        }

        propertyInfo.SetValue(transform, originalVector + magnitude, null);

        if (m_EndDelay > 0f)
        {
            var endDelayTime = 0f;
            while (endDelayTime < m_EndDelay)
            {
                endDelayTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}