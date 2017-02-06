using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public Attribute health = new Attribute();
    public Attribute attack = new Attribute();
    public Attribute defense = new Attribute();

    //TODO: Set a way for the enemy to listen for the Events that are declared 
        //TODO: in the CombatManager
    //TODO: Set the values for the enemy.
    private void OnCombatBegin()
    {
        
    }

    private void OnCombatUpdate()
    {

    }

    private void OnCombatEnd()
    {

    }

    private void OnPlayerTurn()
    {

    }
}
