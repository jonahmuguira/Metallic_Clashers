

namespace Combat
{
    using System.Collections;

    using UnityEngine;

    public class TargetingEnemy : MonoBehaviour
    {
        [SerializeField,]
        private GameObject m_MarkerPrefab;

        [SerializeField]
        private Vector3 m_MaxPositionOffset;
        [SerializeField]
        private Vector3 m_MinPositionOffset;
        [SerializeField]
        private AnimationCurve m_AnimationCurve;
        [SerializeField]
        private float m_AnimationTime;

        private bool m_Rising = true;

        private Transform m_Marker;
        private Vector3 m_CurrentPosition;
        private IEnumerator m_MarkerMovementEnumerator;

        private void Start()
        {
            m_Marker = Instantiate(m_MarkerPrefab).transform;

            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
            EnemyManager.self.onCurrentEnemyChange.AddListener(OnCurrentEnemyChange);

            OnCurrentEnemyChange(EnemyManager.self.currentEnemy.enemy);
            m_MarkerMovementEnumerator = MarkerMovementEnumerator();
        }

        private void OnCombatUpdate()
        {
            if (m_MarkerMovementEnumerator != null)
                m_MarkerMovementEnumerator.MoveNext();
        }

        private void OnCurrentEnemyChange(Enemy enemy)
        {
            if (enemy != null)
                SetMarkerToCurrent(enemy.GetComponent<EnemyMono>());
        }

        private void SetMarkerToCurrent(EnemyMono enemyMono)
        {
            var currentEnemyBounds = enemyMono.GetComponent<Collider>().bounds;

            m_Marker.SetParent(enemyMono.transform.root, false);
            m_CurrentPosition = currentEnemyBounds.center +
                new Vector3(0, currentEnemyBounds.extents.y + .5f, 0) + m_MinPositionOffset;
            m_Marker.position = m_CurrentPosition;
        }

        private IEnumerator MarkerMovementEnumerator()
        {
            var deltaTime = 0f;
            while (true)
            {
                // If the enemy is not null or there are no enemies, return
                if (EnemyManager.self.enemies.Count == 0)
                {
                    Destroy(m_Marker.gameObject);
                    Destroy(this);
                    yield break;
                }

                if (EnemyManager.self.currentEnemy != null && m_AnimationTime != 0f)
                {
                    if (m_Rising)
                        m_Marker.position =
                            m_CurrentPosition + m_MaxPositionOffset *
                            m_AnimationCurve.Evaluate(deltaTime / m_AnimationTime);
                    else
                        m_Marker.position =
                            m_CurrentPosition + m_MaxPositionOffset *
                            m_AnimationCurve.Evaluate(1f - deltaTime / m_AnimationTime);

                    if (deltaTime > m_AnimationTime)
                    {
                        m_Rising = !m_Rising;
                        deltaTime = 0f;
                    }

                    deltaTime += Time.deltaTime;
                }

                yield return null;
            }
        }
    }
}