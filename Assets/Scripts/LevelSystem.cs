/* Script Info - Script Name: LevelSystem.cs, Created By: Brock Barlow, This script is used to level up the player. */

using System;

[Serializable]
public class LevelSystem
{
    [Serializable]
    public struct LevelInfo
    {
        public uint level; //player's level
        public uint currentExperience; //player's current exp amount
        public uint experienceRequired; //the exp amount the player needs to level up
        public uint experienceNeeded { get { return experienceRequired - currentExperience; } } //experienceRequired - currentExperience 
    }

    public LevelInfo playerLevelInfo;

    private LevelInfo CalculateLevel(uint exp)
    {
        //base_xp * (level_to_get ^ factor);

        //base_xp = constant, how much xp needed to levelup.
        //level_to_get = level aiming for.
        //factor = constant, how much of an increase of xp needed for levelup.

        var tempExperience = exp;
        uint tempExpeienceRequired = 0;

        const uint c_baseExperience = 200; //base_xp //testing value from research results
        uint tempLevel = 1; //level_to_get //player needs to start at level one
        const uint c_factor = 2; //factor //testing value from research results

        var experienceRequiredFormula = c_baseExperience * ((tempLevel + 1) ^ c_factor);

        for (tempLevel = 1; tempExperience > experienceRequiredFormula; tempLevel++)
        {
            tempExperience -= experienceRequiredFormula;
            tempExpeienceRequired += experienceRequiredFormula;
            experienceRequiredFormula = c_baseExperience * ((tempLevel + 2) ^ c_factor); //recalculate formula value
        }

        tempExpeienceRequired += experienceRequiredFormula;

        var levelInfo = new LevelInfo
        {
            level = tempLevel, //player's level
            currentExperience = exp, //player's current exp amout
            experienceRequired = tempExpeienceRequired, //the exp amount the player needs to level up
        };

        return levelInfo;
    }
                             //fight exp
    public void IsLeveledUp(uint modifier)
    {
        var tempCurrentExperience = playerLevelInfo;
        var finalTotal = CalculateLevel(playerLevelInfo.currentExperience + modifier);

        // TODO: This was messing with the values when Combat started.
        //if (finalTotal.level != tempCurrentExperience.level)
        //{
        //    uint differenceInLevel;

        //    for (differenceInLevel = finalTotal.level - tempCurrentExperience.level; differenceInLevel > 0; differenceInLevel--)
        //    {
        //        const int c_percentageValue = 10 / 100; //10%

        //        GameManager.self.playerData.health.value *= c_percentageValue; //health stat change
        //        GameManager.self.playerData.attack.value *= c_percentageValue; //attack stat change
        //        GameManager.self.playerData.defense.value *= c_percentageValue; //defense stat change
        //        StaminaManager.self.maxValue *= c_percentageValue; //stamina stat change
        //    }
        //}

        playerLevelInfo = finalTotal;
    }
}