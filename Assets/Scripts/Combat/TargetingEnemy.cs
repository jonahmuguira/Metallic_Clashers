using UnityEngine;

namespace Combat
{
    using System.Linq;

    using CustomInput;
    using CustomInput.Information;

    using UnityEngine.Serialization;

    public class TargetingEnemy : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("markerPrefab")]
        private GameObject m_MarkerPrefab;

        private bool m_Rising = true;
        private float m_MaxY;
        private float m_MinY;

        private Transform m_Marker;

        private void Start()
        {
            m_Marker = Instantiate(m_MarkerPrefab).transform;

            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
            EnemyManager.self.onCurrentEnemyChange.AddListener(OnCurrentEnemyChange);

            OnCurrentEnemyChange(EnemyManager.self.currentEnemy.enemy);
        }

        private void OnCombatUpdate()
        {
            // If the enemy is not null or there are no enemies, return
            if (EnemyManager.self.currentEnemy == null)
                return;
            if (EnemyManager.self.enemies.Count == 0)
            {
                Destroy(gameObject);
                return;
            }

            if (m_Rising)
                m_Marker.position += new Vector3(0, Time.deltaTime, 0);
            else
                m_Marker.position -= new Vector3(0, Time.deltaTime, 0);

            if (m_Marker.position.y < m_MinY)
                m_Rising = true;

            if (m_Marker.position.y > m_MaxY)
                m_Rising = false;
        }

        private void OnCurrentEnemyChange(Enemy enemy)
        {
            SetMarkerToCurrent(enemy.GetComponent<EnemyMono>());
        }

        private void SetMarkerToCurrent(EnemyMono enemyMono)
        {
            var currenyEnemyBounds = enemyMono.GetComponent<MeshRenderer>().bounds;
            m_MinY = currenyEnemyBounds.center.y + currenyEnemyBounds.extents.y;
            m_MaxY = m_MinY + 1;

            m_Marker.position = currenyEnemyBounds.center +
                new Vector3(0, currenyEnemyBounds.extents.y + .5f, 0);
        }
    }
}