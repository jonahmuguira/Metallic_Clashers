using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Combat.Board;
using Items;
using UnityEngine;
using UnityEngine.Events;

using Tree = StageSelection.Tree;

[Serializable]
public class PlayerData
{
    public PlayerData()
    {
        health = new Attribute();
        attack = new Attribute();
        defense = new Attribute();

        health.value = 200;
        attack.value = 10;
        defense.value = 25;

        health.coefficient = 1;
        defense.coefficient = 1;
        attack.coefficient = 1;

        resistances = new List<GemType>();
        weaknesses = new List<GemType>();
    }

    public PlayerData(float hel, float att, float def)
    {
        health = new Attribute();
        attack = new Attribute();
        defense = new Attribute();

        health.value = hel;
        attack.value = att;
        defense.value = def;

        health.coefficient = 1;
        defense.coefficient = 1;
        attack.coefficient = 1;

        resistances = new List<GemType>();
        weaknesses = new List<GemType>();
    }

    private UnityEvent m_OnTakeDamage = new UnityEvent();

    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public float decayRate = 1f;

    public List<GemType> resistances;
    public List<GemType> weaknesses;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<Tree> worldData = new List<Tree>();
    public StaminaInformation staminaInformation = new StaminaInformation();

    public LevelSystem playerLevelSystem = new LevelSystem();

    public UnityEvent onTakeDamage { get { return m_OnTakeDamage; } }

    [XmlIgnore]
    public ItemManager itemManager = new ItemManager();

    public void TakeDamage(float damage, GemType gemType)
    {
        var finalDamage = damage - defense.totalValue;
        defense.modifier -= damage;

        if (defense.modifier < -defense.value) { defense.modifier = -defense.value; }

        if (finalDamage <= 0) return;

        if (resistances.Contains(gemType)) { finalDamage *= .75f; }
        else if (weaknesses.Contains(gemType)) { finalDamage *= 1.25f; }

        health.modifier -= finalDamage;

        m_OnTakeDamage.Invoke();
    }

    public void DecayShield()
    {
        if (defense.modifier > 0) { defense.modifier -= decayRate * Time.deltaTime; if (defense.modifier < 0) { defense.modifier = 0; } }

        if (defense.modifier > defense.value * 20 - defense.value) { defense.modifier = defense.value * 20 - defense.value; }
    }
}