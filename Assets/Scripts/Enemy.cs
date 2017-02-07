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

    public Enemy()
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
    }

    public Enemy(float p_Health, float p_Attack, float p_Defense)
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        health.value = p_Health;
        attack.value = p_Attack;
        defense.value = p_Defense;
    }

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
