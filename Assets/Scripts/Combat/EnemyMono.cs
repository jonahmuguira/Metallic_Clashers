namespace Combat
{
    using System.Linq;

    using UnityEngine;

    using UI;

    public class EnemyMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Enemy m_Enemy;

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

            newGameObject.transform.SetAsFirstSibling();

            newEnemyHealthBar.enemy = m_Enemy;

            m_Enemy.components.Add(this);
            m_Enemy.components.Add(newEnemyHealthBar);

            m_Enemy.health.onTotalValueChanged.AddListener(OnTotalValueChanged);
            m_Enemy.onDestroy.AddListener(OnDestroy);
        }

        private void OnTotalValueChanged()
        {

        }

        private void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}
