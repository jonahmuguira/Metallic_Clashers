/*
 Script Info
 Script Name: BaseItem.cs
 Created by: Brock Barlow
 This script is used to handle items.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }
}

[Serializable]
public class InstantItem : BaseItem 
{
    //instant item idea: heal up item.
    //will "restore" the player's missing health value, but not fully.
    //effect will be instant, no lasting/timed effect.

    //instant item idea: instant attack item.
    //will attack the enemy for a set amount of damage.
    //effect will be instant, no lasting/timed effect.

    //no "own" attributes, no age, no durability.
    //just modifier.

    public override void UseItem()
    {
        modifier = GameManager.self.playerData.health.value / 100;

        GameManager.self.playerData.health.value += modifier;
    }
}

[Serializable]
public class TurnBased : BaseItem
{
    //these items are based on the amount of turns the player makes during play.

    //no "own" attributes, no modifier, no durability.
    //just age.

    public void UpdateSelf() //increment age.
    {
        age++;
    }
}

[Serializable]
public class TurnItem : TurnBased
{
    //turn item idea: attack increase item.
    //will raise the player's attack value.
    //effect will have a lasting/timed effect (based off turns).

    //turn item idea: defense increase item.
    //will raise the player's defense value.
    //effect will have a lasting/timed effect (based off turns).

    //no "own" attributes
    //will need age, durability and modifier

    public override void UseItem()
    {

    }

    //public void test()
    //{
    //    var test = durability + 1;
    //}
}

[Serializable]
public class TimeBased : BaseItem
{
    //these items are based on a timer when activated by player during play.

    //no "own" attributes, no modifier, no durability.
    //just age.

    public void UpdateSelf(float value)
    {
        //modify age with float value.
        age += value;
    }
}

[Serializable]
public class TimeItem : TimeBased
{
    //time item idea: attack increase item.
    //will raise the player's attack value.
    //effect will have a lasting/timed effect (based off time).

    //time item idea: defense increase item.
    //will raise the player's defense value.
    //effect will have a lasting/timed effect (based off time).

    //no attributes

    public override void UseItem()
    {

    }
}