using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public Attribute health = new Attribute();
    public Attribute attack = new Attribute();
    public Attribute defense = new Attribute();

    public float attackSpeed;
    public int movesUntilAttack;

    private int movesCounter;
    private float attackCountdown;
    private PlayerData playerData;

    public Enemy()
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
    }

    public Enemy(float pHealth, float pAttack, float pDefense, 
        float pattackSpeed, int pmovesUntilAttack)
    {
        CombatManager.self.onCombatBegin.AddListener(OnCombatBegin);
        CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
        CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);

        health.value = pHealth;
        attack.value = pAttack;
        defense.value = pDefense;

        attackSpeed = pattackSpeed;
        attackCountdown = attackSpeed;

        movesUntilAttack = pmovesUntilAttack;
    }

    private void OnCombatBegin()
    {
        playerData = GameManager.self.playerData;
    }

    private void OnCombatUpdate()
    {
        attackCountdown -= Time.deltaTime;

        if (attackCountdown > 0) return;
        Attack();
        attackCountdown = attackSpeed;
    }

    private void OnCombatEnd()
    {

    }

    private void OnPlayerTurn()
    {
        movesCounter++;
        if (movesCounter != 0 && movesCounter%movesUntilAttack == 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        
    }
}
