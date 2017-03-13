using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

using Tree = StageSelection.Tree;

[Serializable]
public class PlayerData
{
    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public List<GemType> resistances;
    public List<GemType> weaknesses;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<Tree> worldData = new List<Tree>();
    public StaminaInformation staminaInformation;

    public LevelSystem playerLevelSystem;

    public void TakeDamage(float damage, GemType gemType)
    {
        var percentage = damage / defense.totalValue;
        var finalDamage =  damage * Mathf.Clamp(percentage, 0f, 1f);

        if (resistances.Contains(gemType))
        {
            finalDamage *= .75f;
        }

        else if (weaknesses.Contains(gemType))
        {
            finalDamage *= 1.25f;
        }

        health.modifier -= finalDamage;
    }
}
