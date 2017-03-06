/*
 Script Info
 Script Name: LevelSystem.cs
 Created By: Brock Barlow
 This script is used to level up the player.
*/

using System.Reflection;

using UnityEngine;

public class LevelSystem
{
    [SerializeField]
    public struct LevelInfo
    {
        public uint level; //player's level
        public uint currentExperience; //player's current exp amount
        public uint experienceRequired; //the exp amount the player needs to level up
        public uint experienceNeeded; //experienceRequired - currentExperience
    }

    [SerializeField]
    public LevelInfo playerLevelInfo;

    private static LevelInfo CalculateLevel(uint exp)
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
                                                     //fight exp
    public void IsLeveledUp(uint currentExperience, uint modifier)
    {
        var tempCurrentExperience = CalculateLevel(currentExperience);
        var finalTotal = CalculateLevel(currentExperience + modifier);

        if (finalTotal.level != tempCurrentExperience.level)
        {
            //TODO: Define what stats get changed and by how much.
            //Need a lot of feedback in this area.

            var playerData = new PlayerData();

            const int c_percentageValue = 10 / 100;

            //health stat change
            var healthPercentageResult = playerData.health.totalValue * c_percentageValue;
            var statChangeHealth = playerData.health.totalValue + healthPercentageResult;

            //attack stat change
            var attackPercentageResult = playerData.attack.totalValue * c_percentageValue;
            var statChangeAttack = playerData.attack.totalValue + attackPercentageResult;

            //defense stat change
            var defensePercentageResult = playerData.defense.totalValue * c_percentageValue;
            var statChangeDefense = playerData.defense.totalValue + defensePercentageResult;

            //stamina stat change
            //TODO:
        }

        playerLevelInfo = finalTotal;
    }
}