using System;
using System.Collections.Generic;

using UnityEngine;

using Tree = StageSelection.Tree;

[Serializable]
public class PlayerData
{
    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<Tree> worldData = new List<Tree>();

    public void TakeDamage(float damage)
    {
        var percentage = damage / defense.totalValue;
        health.modifier -=  damage * Mathf.Clamp(percentage, 0f, 1f);
    }
}
