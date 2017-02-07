using System;
using System.Collections;
using System.Collections.Generic;

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
        get { return m_StartDelay;}
    }
    public float endDelay
    {
        get { return m_EndDelay; }
    }

    public Vector3 magnitude
    {
        get { return m_Magnitude;}
    }

    public IEnumerator Animate(Transform transform, List<Transform> targets)
    {
        return null; //placed here to avoid error(s)
    }
}