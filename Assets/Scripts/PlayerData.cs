using System;
using System.Collections.Generic;

using StageSelection;

[Serializable]
public class PlayerData
{
    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<StageSelection.Tree> worldData = new List<Tree>();
}
