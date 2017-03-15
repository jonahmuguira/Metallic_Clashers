/*
 Script Info
 Script Name: BaseItem.cs
 Created by: Brock Barlow
 This script is used to handle items.
*/

using System;

[Serializable]
public class BaseItem
{
    private float m_Age; //current durabilities max value.
    public float age
    {
        get { return m_Age; }
        set { m_Age = value; }
    }

    public float durability; //how long the item will last.
    public float modifier; //value that will "modify" player attribute(s).

    public virtual void UseItem()
    {
        age = 0;
        durability = 0;
        modifier = 0;
    }
}

[Serializable]
public class InstantItem : BaseItem 
{
    //instant item idea: heal up item.
    //will "restore" the player's "missing health" value, but not fully.
    //effect will be instant, no lasting/timed effect.

    //TODO. Reconfirm if healing item restores some or all health for player.

    public override void UseItem()
    {
        //TODO. Calculate the player's missing health and calculate how much health should be restored.

        modifier = GameManager.self.playerData.health.value;

        if (GameManager.self.playerData.health.value < modifier)
        {
            GameManager.self.playerData.health.value = modifier;
            if (modifier < GameManager.self.playerData.health.value)
            {
                modifier = GameManager.self.playerData.health.value;
            }
        }

        //modifier = GameManager.self.playerData.health.totalValue - GameManager.self.playerData.health.value;

        //modifier = GameManager.self.playerData.health.value / 75;
        //GameManager.self.playerData.health.value *= modifier;
    }
}

[Serializable]
public class TurnBased : BaseItem
{
    //these items are based on the amount of turns the player makes during play.

    public void UpdateSelf() //increment age.
    {
        age++;
    }
}

[Serializable]
public class TurnItem : TurnBased
{
    //turn item idea: attack increase item. will raise the player's attack value.
    //effect will have a lasting/timed effect (based off turns).

    //turn item idea: defense increase item. will raise the player's defense value.
    //effect will have a lasting/timed effect (based off turns).

    public int itemStatType = 2;

    public override void UseItem()
    {
        var tempAttackValue = GameManager.self.playerData.attack.value;
        var tempDefenseValue = GameManager.self.playerData.defense.value;

        switch (itemStatType)
        {
            case 2: //this case is for the attack stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.attack.value *= modifier;

                if (age >= durability) //Destroy/Remove attack buff.
                {
                    //TODO. Revert attack buff.
                    GameManager.self.playerData.attack.value = tempAttackValue;
                }
                break;

            case 1: //this case is for the defense stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.defense.value *= modifier;

                if (age >= durability) //Destroy/Remove defense buff.
                {
                    //TODO. Revert defense buff.
                    GameManager.self.playerData.defense.value = tempDefenseValue;
                }
                break;

            default:
                break;
        }
    }
}

[Serializable]
public class TimeBased : BaseItem
{
    //these items are based on a timer when activated by the player during play.

    public void UpdateSelf(float value) //modify age with float value.
    {
        age += value;
    }
}

[Serializable]
public class TimeItem : TimeBased
{
    //time item idea: attack increase item. will raise the player's attack value.
    //effect will have a lasting/timed effect (based off time).

    //time item idea: defense increase item. will raise the player's defense value.
    //effect will have a lasting/timed effect (based off time).

    public int itemStatType = 2;

    public override void UseItem()
    {
        var tempAttackValue = GameManager.self.playerData.attack.value;
        var tempDefenseValue = GameManager.self.playerData.defense.value;

        switch (itemStatType)
        {
            case 2: //this case is for the attack stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.attack.value *= modifier;

                if (age >= durability) //Destroy/Remove attack buff.
                {
                    //TODO. Revert attack buff.
                    GameManager.self.playerData.attack.value = tempAttackValue;
                }
                break;

            case 1: //this case is for the defense stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.defense.value *= modifier;

                if (age >= durability) //Destroy/Remove defense buff.
                {
                    //TODO. Revert defense buff.
                    GameManager.self.playerData.defense.value = tempDefenseValue;
                }
                break;

            default:
                break;
        }
    }
}