namespace Combat
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Board.Information;

    using CustomInput.Information;

    using UnityEngine;

    public class EnemyManager : SubManager<EnemyManager>
    {
        [SerializeField]
        private GameObject m_EnemyHealthBarPrefab;

        private List<EnemyMono> m_Enemies = new List<EnemyMono>();

        private EnemyMono m_CurrentEnemy;

        [SerializeField]
        private UnityEnemyEvent m_OnCurrentEnemyChange = new UnityEnemyEvent();

        [SerializeField]
        private float m_PauseCameraTime;
        private float m_CurrentPauseCameraTime;
        private IEnumerator m_PauseCameraEnumerator;

        private List<IEnumerator> m_AnimateEnemies = new List<IEnumerator>();

        private uint m_ExperianceTotal = 0;

        public float enemyPadding = 1f;
        public bool doCombat;

        public GameObject enemyHealthBarPrefab { get { return m_EnemyHealthBarPrefab; } }
        public uint experianceTotal { get { return m_ExperianceTotal;} }

        public EnemyMono currentEnemy
        {
            get { return m_CurrentEnemy; }
            private set
            {
                if (m_CurrentEnemy == value)
                    return;

                m_CurrentEnemy = value;

                m_OnCurrentEnemyChange.Invoke(
                    m_CurrentEnemy == null ? null : value.enemy);
            }
        }

        public UnityEnemyEvent onCurrentEnemyChange { get { return m_OnCurrentEnemyChange; } }

        public List<EnemyMono> enemies { get { return m_Enemies; } }

        // Use this for initialization
        protected override void Init()
        {
            if (!doCombat)
                return;

            var managerEnemies = GameManager.self.enemyIndexes;
            var enemyPrefabList = GameManager.self.enemyPrefabList;

            var totalSpace =
                enemyPrefabList.Sum(
                    enemy => enemy.transform.root.GetComponentInChildren<Collider>().bounds.size.x);

            totalSpace += enemyPadding * (managerEnemies.Count - 1);

            var pos = -totalSpace / 2f;

            for (var i = 0; i < managerEnemies.Count; i++)
            {
                var enemyPrefab = enemyPrefabList[managerEnemies[i]];
                var enemyMeshBounds =
                    enemyPrefab.transform.root.GetComponentInChildren<Collider>().bounds;

                pos += enemyMeshBounds.extents.x;

                var enemyObject =
                    Instantiate(
                        enemyPrefab,
                        new Vector3(pos, .5f, 0),
                        enemyPrefab.transform.rotation);

                var animator = enemyObject.transform.root.GetComponentInChildren<Animator>();
                animator.speed = 0f;

                var randomTime = Random.Range(0f, 1f);
                m_AnimateEnemies.Add(AnimateEnemy(randomTime, animator));

                pos += enemyMeshBounds.extents.x;
                pos += enemyPadding;

                enemyObject.name += i;
                var enemyMono = enemyObject.transform.root.GetComponentInChildren<EnemyMono>();
                var enemy = enemyMono.enemy;
                enemy.attackCountdown = enemy.attackSpeed - Random.Range(0f, enemy.attackSpeed);
                enemy.movesCounter = Random.Range(0, enemy.movesUntilAttack);

                m_Enemies.Add(enemyMono);
                CombatManager.self.onCombatBegin.AddListener(enemyMono.enemy.OnCombatBegin);

                m_ExperianceTotal += enemy.experianceValue;
            }
            GameManager.self.enemyIndexes = new List<int>();
            currentEnemy = m_Enemies[0];

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

                currentEnemy.enemy.TakeDamage(dam, matchInfo.type);

                if (currentEnemy.enemy.health.totalValue <= 0f)
                {
                    m_Enemies.Remove(currentEnemy);
                    currentEnemy = m_Enemies.FirstOrDefault();
                }
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

            GameManager.self.playerData.DecayShield();

            if (m_PauseCameraEnumerator != null)
                m_PauseCameraEnumerator.MoveNext();

            foreach (var animateEnemy in m_AnimateEnemies.ToList())
            {
                if (!animateEnemy.MoveNext())
                    m_AnimateEnemies.Remove(animateEnemy);
            }
        }


        protected override void OnPress(TouchInformation touchInfo)
        {
            // Else shoot ray from touch
            var ray = Camera.main.ScreenPointToRay(touchInfo.position);
            RaycastHit raycastHit;

            if (!Physics.Raycast(ray.origin, ray.direction, out raycastHit))   // Did the ray hit something
                return;

            var hitEnemyMono = raycastHit.transform.root.GetComponentInChildren<EnemyMono>();

            if (hitEnemyMono == null)
                return;

            currentEnemy = hitEnemyMono;

            m_CurrentPauseCameraTime = 0f;
            if (m_PauseCameraEnumerator == null)
                m_PauseCameraEnumerator = PauseCameraEnumerator();
        }

        private IEnumerator AnimateEnemy(float waitTime, Animator enemyAnimator)
        {
            var deltaTime = 0f;
            while (deltaTime < waitTime)
            {
                deltaTime += Time.deltaTime;

                yield return null;
            }

            enemyAnimator.speed = 1f;
        }

        private IEnumerator PauseCameraEnumerator()
        {
            var combatCamera = FindObjectOfType<CombatCamera>();
            if (combatCamera == null)
                yield break;

            combatCamera.isAnimating = false;

            // Wait until 'm_PauseCameraTime' time has passed
            while (m_CurrentPauseCameraTime < m_PauseCameraTime)
            {
                m_CurrentPauseCameraTime += Time.deltaTime;

                yield return null;
            }

            combatCamera.isAnimating = true;

            m_PauseCameraEnumerator = null;
        }
    }
}