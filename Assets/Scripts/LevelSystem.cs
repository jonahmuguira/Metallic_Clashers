using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LevelSystem
{
    public struct LevelInfo
    {
        public uint level;
        public uint currentExperience;
        public uint experienceRequired;
        public uint experienceNeeded; //experienceRequired - currentExperience
    }

    public LevelInfo playLevelInfo;

    //private LevelInfo CalculateLevel(uint exp)
    //{
    //    //base_xp * (level_to_get ^ factor);

    //    //base_xp = constant, how much xp needed to levelup.
    //    //level_to_get = level aiming for.
    //    //factor = constant, how much of an increase of xp needed for levelup.

    //    var tempExperience = exp;

    //    var level = 0;
    //    var baseExperience = 100;
    //    var factor = 2;
    //    var math = baseExperience * (level + 1 ^ factor);

    //    for (level = 1; tempExperience > math; level++)
    //    {
    //        tempExperience -= math;
    //    }
    //}

    public void IsLeveledUp(uint currentExperience, uint modifier)
    {
        
    }
}