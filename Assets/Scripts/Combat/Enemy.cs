using System;
using System.Collections.Generic;
using Combat.Board;
using UnityEngine;

namespace Combat
{
    using UnityEngine.Events;

    [Serializable]
    public class UnityEnemyEvent : UnityEvent<Enemy> { }

    [Serializable]
    public class Enemy : IAttachable
    {
        [SerializeField]
        private Attribute m_Health = new Attribute { value = 10f };
        [SerializeField]
        private Attribute m_Attack = new Attribute { value = 10f };
        [SerializeField]
        private Attribute m_Defense = new Attribute { value = 10f };

        private UnityEnemyEvent m_OnTakeDamage = new UnityEnemyEvent();
        private UnityEvent m_OnDestroy = new UnityEvent();

        public Attribute health { get { return m_Health; } }
        public Attribute attack { get { return m_Attack; } }
        public Attribute defense { get { return m_Defense; } }
        public UnityEvent onDestroy { get { return m_OnDestroy; } }

        public float attackSpeed;
        public int movesUntilAttack;
        public GemType damageType;

        public List<GemType> resistances;
        public List<GemType> weaknesses;

        private int movesCounter = 0;
        private float attackCountdown;

        private readonly List<IComponent> m_Components = new List<IComponent>();

        public UnityEnemyEvent onTakeDamage { get { return m_OnTakeDamage; } }

        public List<IComponent> components { get { return m_Components; } }

        public Enemy()
        {
            health.value = 10;
            attack.value = 10;
            defense.value = 10;

            attackSpeed = 5;
            attackCountdown = attackSpeed;

            movesUntilAttack = 3;
        }

        public Enemy(
            float newHealth,
            float newAttack,
            float newDefense,
            float newAttackSpeed,
            int newMovesUntilAttack)
        {
            health.value = newHealth;
            attack.value = newAttack;
            defense.value = newDefense;

            attackSpeed = newAttackSpeed;
            attackCountdown = attackSpeed;

            movesUntilAttack = newMovesUntilAttack;
        }

        public void OnCombatBegin()
        {
            CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatUpdate()
        {
            if (health.totalValue <= 0)
                m_OnDestroy.Invoke();

            attackCountdown -= Time.deltaTime;

            if (attackCountdown > 0) return;
            Attack();
            attackCountdown = attackSpeed;
        }

        private void OnPlayerTurn()
        {
            movesCounter++;
            if (movesCounter != 0 && movesCounter % movesUntilAttack == 0)
            {
                Attack();
            }
        }

        private void Attack()
        {
            GameManager.self.playerData.TakeDamage(attack.totalValue, damageType);
        }

        public void TakeDamage(float damage, GemType gemType)
        {
            var percentage = damage / defense.totalValue;
            var finalDamage = damage * Mathf.Clamp(percentage, 0f, 1f);

            if (resistances != null && resistances.Contains(gemType))
            {
                finalDamage *= .75f;
            }

            else if (weaknesses != null && weaknesses.Contains(gemType))
            {
                finalDamage *= 1.25f;
            }

            health.modifier -= finalDamage;

            m_OnTakeDamage.Invoke(this);
        }
    }
}
