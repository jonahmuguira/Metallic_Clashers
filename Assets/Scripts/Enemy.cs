using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    [SerializeField]
    private Attribute m_health = new Attribute();
    [SerializeField]
    private Attribute m_attack = new Attribute();
    [SerializeField]
    private Attribute m_defense = new Attribute();

    public Attribute health { get {return m_health;} }
    public Attribute attack { get { return m_attack; } }
    public Attribute defense { get { return m_defense; } }

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
