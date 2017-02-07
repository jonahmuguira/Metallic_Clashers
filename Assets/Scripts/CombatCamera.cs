using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class CombatCamera
{
    [SerializeField]
    private List<TransformAnimation> m_Animations = new List<TransformAnimation>();

    public List<TransformAnimation> animations { get { return m_Animations;} }

    private void OnCombatUpdate()
    {
        //TODO.
    }
}