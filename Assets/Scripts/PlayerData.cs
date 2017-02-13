using StageSelection;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<Tree> worldData = new List<Tree>();

    public void TakeDamage(float dam)
    {
        var def = defense.value * defense.coefficient + defense.modifier;
        var damagePercentage = (dam - def) / def;

        health.modifier -= (float)Math.Log(dam, def * .8f);

        //if (damagePercentage <= 0.2f)
        //{
        //    health.modifier += dam * 0.2f;
        //    return;
        //}

        //health.modifier += dam - def;
    }
}
