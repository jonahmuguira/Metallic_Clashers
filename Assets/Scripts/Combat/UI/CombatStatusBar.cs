﻿namespace Combat.UI
{
    using System;
    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;

    public class CombatStatusBar : MonoBehaviour
    {
        [SerializeField]
        private Image m_HealthImage;
        [SerializeField]
        private Image m_HealthDelayedImage;
        [SerializeField]
        private Image m_HealthBackgroundImage;
        [SerializeField]
        private Text m_HealthText;

        [SerializeField]
        private Image m_ShieldImage;
        [SerializeField]
        private Text m_ShieldText;

        [SerializeField]
        private Image m_ShieldIcon;

        private Coroutine m_DelayedHealthCoroutine;
        private float m_DelayedHealthValue;

        // Use this for initialization
        private void Awake()
        {
            OnCombatModeChange();
        }

        private void Start()
        {
            OnHealthModifierChanged();
            OnDefenseModifierChanged();

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);

            GameManager.self.playerData.health.onTotalValueChanged.AddListener(OnHealthModifierChanged);
            GameManager.self.playerData.defense.onTotalValueChanged.AddListener(OnDefenseModifierChanged);
        }

        private void OnCombatModeChange()
        {
            if (m_HealthImage == null)
                return;

            m_HealthImage.color =
                CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;

            if (m_HealthBackgroundImage == null)
                return;

            float h, s, v;
            Color.RGBToHSV(m_HealthImage.color, out h, out s, out v);

            m_HealthDelayedImage.color = Color.HSVToRGB(h, s + 0.2f, v);
            m_HealthBackgroundImage.color = Color.HSVToRGB(h, s - 0.25f, v);
        }

        private void OnHealthModifierChanged()
        {
            if (m_HealthText != null)
            {
                var playerHeath = GameManager.self.playerData.health;
                m_HealthText.text =
                    Math.Ceiling(playerHeath.totalValue) + "/" + Mathf.Ceil(playerHeath.value);
            }

            if (m_DelayedHealthValue == 0)
                m_DelayedHealthValue = GameManager.self.playerData.health.totalValue;

            OnStatusModifierChanged();

            if (m_DelayedHealthCoroutine == null)
                m_DelayedHealthCoroutine = StartCoroutine(DelayedHealthEnumerator());
        }

        private void OnDefenseModifierChanged()
        {
            if (m_ShieldText != null)
            {
                var playerDefense = GameManager.self.playerData.defense;
                if (playerDefense.totalValue > 0f)
                    m_ShieldText.text = Math.Ceiling(playerDefense.totalValue).ToString();
                else
                    m_ShieldText.text = string.Empty;
            }

            OnStatusModifierChanged();
        }

        private void OnStatusModifierChanged()
        {
            if (m_HealthImage == null || m_ShieldImage == null)
                return;

            var totalValue =
                GameManager.self.playerData.health.totalValue
                + GameManager.self.playerData.defense.totalValue;

            var maxValue = GameManager.self.playerData.health.value;

            var imageBounds = m_HealthImage.rectTransform.rect.max;

            if (totalValue < maxValue)
            {
                m_HealthImage.fillAmount =
                    GameManager.self.playerData.health.totalValue / maxValue;

                m_HealthDelayedImage.rectTransform.anchorMin =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_HealthDelayedImage.rectTransform.anchorMin.y);
                m_HealthDelayedImage.rectTransform.anchorMax=
                    new Vector2(
                        m_HealthImage.fillAmount + 1f,
                        m_HealthDelayedImage.rectTransform.anchorMax.y);

                m_HealthDelayedImage.fillAmount =
                    (m_DelayedHealthValue / maxValue) - m_HealthImage.fillAmount;

                m_ShieldImage.rectTransform.anchorMin =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_ShieldImage.rectTransform.anchorMin.y);
                m_ShieldImage.rectTransform.anchorMax =
                    new Vector2(
                        m_ShieldImage.rectTransform.anchorMin.x + 1f,
                        m_ShieldImage.rectTransform.anchorMax.y);

                m_ShieldImage.fillAmount =
                    GameManager.self.playerData.defense.totalValue / maxValue;
            }
            else
            {
                m_HealthImage.fillAmount =
                    GameManager.self.playerData.health.totalValue / totalValue;

                m_ShieldImage.rectTransform.anchorMin =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_ShieldImage.rectTransform.anchorMin.y);
                m_ShieldImage.rectTransform.anchorMax =
                    new Vector2(
                        m_ShieldImage.rectTransform.anchorMin.x + 1f,
                        m_ShieldImage.rectTransform.anchorMax.y);

                m_ShieldImage.fillAmount =
                    GameManager.self.playerData.defense.totalValue / totalValue;
            }

            m_HealthText.rectTransform.anchorMax =
                new Vector2(
                    m_HealthImage.fillAmount,
                    m_HealthText.rectTransform.anchorMax.y);

            m_ShieldText.rectTransform.anchorMax =
                new Vector2(
                    m_ShieldImage.fillAmount,
                    m_ShieldImage.rectTransform.anchorMax.y);

            m_ShieldIcon.rectTransform.anchoredPosition =
                new Vector2(
                    -m_ShieldText.preferredWidth / 2f - 10f,
                    m_ShieldIcon.rectTransform.anchoredPosition.y);
        }

        private IEnumerator DelayedHealthEnumerator()
        {
            while (m_DelayedHealthValue != GameManager.self.playerData.health.totalValue)
            {
                m_DelayedHealthValue =
                    Mathf.MoveTowards(
                        m_DelayedHealthValue,
                        GameManager.self.playerData.health.totalValue,
                        7f * Time.deltaTime);

                OnStatusModifierChanged();
                yield return null;
            }

            m_DelayedHealthCoroutine = null;
        }
    }
}
