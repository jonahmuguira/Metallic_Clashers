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
    private int m_Inventory; //item container.

    private int m_Turns; //will determine how long effect will last based on turns.
    private bool m_IsTurnActive = false; //starts false, becomes true when active.

    private float m_Timer; //will determine how long effect will last based on time.
    private bool m_IsTimeActive = false; //starts false, becomes true when active.

    public uint modifier; //value that will "modifier" player attribute(s).
}

[Serializable]
public class InstantBased : BaseItem
{
    
}

[Serializable]
public class TurnBased : BaseItem
{
    
}

[Serializable]
public class TimeBased : BaseItem
{

}