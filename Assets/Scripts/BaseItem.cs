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

    public float durability = 0; //how long the item will last.
    public uint modifier; //value that will "modify" player attribute(s).

    public virtual void UseItem() { }
}

//[Serializable]
public class TurnBased : BaseItem
{
    public void UpdateSelf()
    {
        
    }
}

//[Serializable]
public class TimeBased : BaseItem
{
    public void UpdateSelf()
    {

    }
}

//[Serializable]
public class InstantItemTestClass : BaseItem
{
    public override void UseItem()
    {
        //Instantly do effect. No lasting effects.
    }
}

//[Serializable]
public class TurnItemTestClass : TurnBased
{
    public override void UseItem()
    {
        
    }

    public void test()
    {
        var test = durability + 1;
    }
}

//[Serializable]
public class TimeItemTestClass : TimeBased
{
    public override void UseItem()
    {

    }

    public void test()
    {

    }
}