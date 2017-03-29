namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class EnemyHealthBar : Image, IComponent
    {
        [SerializeField]
        private Enemy m_Enemy;

        public Enemy enemy { get { return m_Enemy; } set { m_Enemy = value; } }

        protected override void Start()
        {
            base.Start();

            m_Enemy.health.onTotalValueChanged.AddListener(UpdateFillAmount);

            UpdateFillAmount();
        }

        private void OnGUI()
        {
            rectTransform.position =
                Camera.main.WorldToScreenPoint(
                    enemy.GetComponent<EnemyMono>().transform.position
                    + new Vector3(0f, 1f, 0f));
        }

        private void UpdateFillAmount()
        {
            fillAmount = enemy.health.totalValue / enemy.health.value;
            color = Color.Lerp(Color.red, Color.green, fillAmount);
        }
    }
}
