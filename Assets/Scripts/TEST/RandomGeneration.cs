namespace TEST
{
    using System.Globalization;

    using UnityEngine;
    using UnityEngine.UI;

    public class RandomGeneration : MonoBehaviour
    {
        [SerializeField]
        private Button m_SeedButton;
        [SerializeField]
        private Button m_GenerateButton;

        [SerializeField]
        private Text m_SeedText;
        [SerializeField]
        private Text m_GenerateText;

        // Use this for initialization
        private void Awake()
        {
            m_SeedButton.onClick.AddListener(OnSeedClick);
            m_GenerateButton.onClick.AddListener(OnGenerateClick);
        }

        private void OnSeedClick()
        {
            int seed;
            if (!int.TryParse(m_SeedText.text, out seed))
                return;

            Random.InitState(seed);
        }
        private void OnGenerateClick()
        {
            m_GenerateText.text = Random.value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
