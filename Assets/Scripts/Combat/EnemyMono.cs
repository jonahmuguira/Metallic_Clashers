namespace Combat
{
    using System.Linq;

    using UnityEngine;

    using UI;

    public class EnemyMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Enemy m_Enemy;
        [SerializeField]
        private Animator m_Animator;

        public Enemy enemy { get { return m_Enemy; } }

        private void Awake()
        {
            var newGameObject = Instantiate(EnemyManager.self.enemyHealthBarPrefab);
            var newEnemyHealthBar = newGameObject.GetComponent<EnemyHealthBar>();

            newGameObject.transform.SetParent(
                FindObjectsOfType<Canvas>().
                    FirstOrDefault(
                        canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay).transform,
                false);

            m_Animator = GetComponent<Animator>();

            newGameObject.transform.SetAsFirstSibling();

            newEnemyHealthBar.enemy = m_Enemy;

            m_Enemy.components.Add(this);
            m_Enemy.components.Add(newEnemyHealthBar);

            m_Enemy.onTakeDamage.AddListener(OnEnemyTakeDamage);
            m_Enemy.onAttack.AddListener(OnAttack);

            m_Enemy.onDestroy.AddListener(OnEnemyDestroy);
        }

        private void OnEnemyDestroy()
        {
            m_Animator.SetTrigger("Dead");
        }

        private void OnEnemyTakeDamage(Enemy hitEnemy)
        {
            if (enemy != hitEnemy)
                return;

            m_Animator.SetTrigger("Take Damage");
        }

        private void OnAttack()
        {
            m_Animator.SetTrigger("Attack");
        }

        private void OnDeadStateExit()
        {
            Destroy(transform.root.gameObject);
        }
    }
}
