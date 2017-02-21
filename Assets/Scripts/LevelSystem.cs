using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LevelSystem
{
    [SerializeField]
    public struct LevelInfo
    {
        public uint level;
        public uint currentExperience;
        public uint experienceRequired;
        public uint experienceNeeded; //experienceRequired - currentExperience
    }

    [SerializeField]
    public LevelInfo playerLevelInfo;

    private LevelInfo CalculateLevel(uint exp)
    {
        //base_xp * (level_to_get ^ factor);

        //base_xp = constant, how much xp needed to levelup.
        //level_to_get = level aiming for.
        //factor = constant, how much of an increase of xp needed for levelup.

        var tempExperience = exp;

        const uint c_baseExperience = 100;
        uint tempLevel = 1;
        const uint c_factor = 2;

        var mathFormula = c_baseExperience * ((tempLevel + 1) ^ c_factor);

        for (tempLevel = 1; tempExperience > mathFormula; tempLevel++)
        {
            tempExperience -= mathFormula;
        }

        var levelInfo = new LevelInfo
        {
            level = tempLevel,
            currentExperience = tempExperience,
            experienceRequired = mathFormula,
            experienceNeeded = mathFormula - tempExperience
        };

        return levelInfo;
    }

    public void IsLeveledUp(uint currentExperience, uint modifier)
    {
        
    }
}