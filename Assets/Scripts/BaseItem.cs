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
    private float m_age; //current durabilities max value.

    public float durability; //how long the item will last.
    public uint modifier; //value that will "modify" player attribute(s).

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

    //no attributes

    public override void UseItem()
    {
        
    }
}

[Serializable]
public class TurnBased : BaseItem
{
    //these items are based on the amount of turns the player makes during play.

    //no attributes

    public void UpdateSelf()
    {
        
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

    //no attributes

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

    //no attributes

    public void UpdateSelf(float value)
    {

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