using UnityEngine;

namespace Combat
{
    using System.Collections;

    using UnityEngine.UI;

    public class ResultsScreen : MonoBehaviour
    {
        [SerializeField]
        private Image m_EXPForgroundImage;
        [SerializeField]
        private Image m_EXPMidgroundImage;
        private RectTransform m_RectTransform;
        private Button m_EndSceneButton;

        public float animationTime;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_RectTransform.anchoredPosition -= new Vector2(0, 1000);
            m_EXPForgroundImage.fillAmount = .5f;
            SetMidground(0);
            m_EndSceneButton = GetComponentInChildren<Button>();
            m_EndSceneButton.onClick.AddListener(() => { CombatManager.self.onCombatEnd.Invoke();});
        }

        [ContextMenu("Start animation")]
        private void ResultsScreenBegin()
        {
            StartCoroutine(ResultsScreenEnumerator());
        }

        private void SetMidground(float fillAmount)
        {
            m_EXPMidgroundImage.rectTransform.anchorMin = new Vector2(
                m_EXPForgroundImage.fillAmount, m_EXPMidgroundImage.rectTransform.anchorMin.y);
            m_EXPMidgroundImage.rectTransform.anchorMax = new Vector2(
                m_EXPMidgroundImage.rectTransform.anchorMin.x + 1, 
                m_EXPMidgroundImage.rectTransform.anchorMax.y);
            m_EXPMidgroundImage.fillAmount = fillAmount;
        }

        private IEnumerator ResultsScreenEnumerator()
        {
            var animationFraction = 1000/animationTime;

            while (m_RectTransform.anchoredPosition.y < 0)
            {
                m_RectTransform.anchoredPosition += new Vector2(0, Time.deltaTime * animationFraction);
                yield return null;
            }

            m_RectTransform.anchoredPosition = Vector2.zero;
        }

    }
}
