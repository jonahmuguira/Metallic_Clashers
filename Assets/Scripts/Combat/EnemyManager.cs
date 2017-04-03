namespace Combat
{
    using System.Collections.Generic;
    using System.Linq;

    using Board.Information;

    using UnityEngine;

    public class EnemyManager : SubManager<EnemyManager>
    {
        [SerializeField]
        private GameObject m_EnemyHealthBarPrefab;

        private List<EnemyMono> m_Enemies = new List<EnemyMono>();

        private EnemyMono m_CurrentEnemy;

        public float enemyPadding = 1f;
        public bool doCombat;

        public GameObject enemyHealthBarPrefab { get { return m_EnemyHealthBarPrefab; } }

        public EnemyMono currentEnemy
        {
            get { return m_CurrentEnemy; }
            set
            {
                if (value != null)
                    m_CurrentEnemy = value;
            }
        }

        public List<EnemyMono> enemies
        {
            get { return m_Enemies; }
        }

        // Use this for initialization
        protected override void Init()
        {
            if (!doCombat)
                return;

            var managerEnemies = GameManager.self.enemyIndexes;
            var enemyPrefabList = GameManager.self.enemyPrefabList;

            var totalSpace = managerEnemies.Sum(enemy => enemyPrefabList
                [enemy].GetComponent<MeshRenderer>().bounds.size.x);

            totalSpace += enemyPadding * (managerEnemies.Count - 1);

            var pos = -totalSpace / 2f;

            for (var i = 0; i < managerEnemies.Count; i++)
            {
                var enemyPrefab = enemyPrefabList[managerEnemies[i]];
                var enemyMeshBounds = enemyPrefab.GetComponent<MeshRenderer>().bounds;
                pos += enemyMeshBounds.extents.x;

                var enemyObject =
                    Instantiate(
                        enemyPrefab,
                        new Vector3(pos, .5f, 0),
                        enemyPrefab.transform.rotation);


                pos += enemyMeshBounds.extents.x;
                pos += enemyPadding;

                enemyObject.name += i;
                var enemyMono = enemyObject.GetComponent<EnemyMono>();
                m_Enemies.Add(enemyMono);
                CombatManager.self.onCombatBegin.AddListener(enemyMono.enemy.OnCombatBegin);
            }
            GameManager.self.enemyIndexes = new List<int>();
            m_CurrentEnemy = m_Enemies[0];

            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
            CombatManager.self.gridMono.grid.onMatch.AddListener(OnMatch);
        }

        private void OnMatch(MatchInformation matchInfo)
        {
            if (!doCombat)
                return;

            var playerData = GameManager.self.playerData;

            switch (CombatManager.self.combatMode)
            {
            case CombatManager.CombatMode.Attack:
                var dam = playerData.attack.totalValue *
                          (1 + (matchInfo.gems.Count - 3) * .25f);

                m_CurrentEnemy.enemy.TakeDamage(dam, matchInfo.type);
                break;

            case CombatManager.CombatMode.Defense:
                GameManager.self.playerData.defense.modifier += matchInfo.gems.Count
                * (playerData.defense.value * .5F);
                break;
            }

        }

        private void OnCombatUpdate()
        {
            if (!doCombat)
                return;

            // Win
            if (m_Enemies.Count == 0)
            {
                CombatManager.self.onCombatEnd.Invoke();
                return;
            }

            // Lose
            if (GameManager.self.playerData.health.totalValue <= 0)
            {
                CombatManager.self.onCombatEnd.Invoke();
                return;
            }

            var destroyList = m_Enemies.Where(e => e.enemy.health.totalValue <= 0).ToList();

            var finalList = m_Enemies.Where(e => !destroyList.Contains(e)).ToList();

            foreach (var d in destroyList)
            {
                d.GetComponent<EnemyMono>().enemy.onDestroy.Invoke();
            }

            m_Enemies = finalList;

            GameManager.self.playerData.DecayShield();
        }
    }
}