namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyTimerUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_TimerMidground;
        [SerializeField]
        private Text m_MovesText;

        private EnemyHealthBar m_EnemyHealthBar;

        // Use this for initialization
        private void Start()
        {
            m_EnemyHealthBar = GetComponent<EnemyHealthBar>();

            CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);

            OnPlayerTurn();
        }

        // Update is called once per frame
        private void OnGUI()
        {
            m_TimerMidground.fillAmount =
                m_EnemyHealthBar.enemy.timeUntilNextAttack / m_EnemyHealthBar.enemy.attackSpeed;
        }

        private void OnPlayerTurn()
        {
            m_MovesText.text = m_EnemyHealthBar.enemy.movesUntilNextAttack.ToString();
        }
    }
}
