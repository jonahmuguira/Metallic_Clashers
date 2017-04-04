namespace Combat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Random = UnityEngine.Random;

    [Serializable]
    public class CombatCamera : MonoBehaviour
    {
        [SerializeField]
        private List<TransformAnimation> m_Animations = new List<TransformAnimation>();

        [SerializeField]
        private float m_ScreenShakeTime;
        [SerializeField]
        private AnimationCurve m_ScreenShakeCurve;

        private IEnumerator m_AnimationEnumerator;
        private IEnumerator m_ScreenShakeEnumerator;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalQuaternion;

        public bool isAnimating = true;

        public List<TransformAnimation> animations { get { return m_Animations; } }

        private void Start()
        {
            m_OriginalPosition = transform.position;
            m_OriginalQuaternion = transform.rotation;

            m_AnimationEnumerator = Animate();

            GameManager.self.playerData.onTakeDamage.AddListener(OnPlayerTakeDamage);
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatUpdate()
        {
            if (!isAnimating)
            {
                transform.position = m_OriginalPosition;
                transform.rotation = m_OriginalQuaternion;
            }
            else if (m_AnimationEnumerator != null)
                m_AnimationEnumerator.MoveNext();

            if (m_ScreenShakeEnumerator != null)
                m_ScreenShakeEnumerator.MoveNext();
        }

        private void OnPlayerTakeDamage()
        {
            m_ScreenShakeEnumerator = ScreenShakEnumerator();
        }

        private IEnumerator Animate()
        {
            Random.InitState(DateTime.Now.Millisecond);

            var originalPosition = transform.localPosition;
            var originalEulerAngles = transform.localEulerAngles;
            var originalZoom = transform.GetChild(0).localPosition;

            TransformAnimation currentAnimation = null;
            while (true)
            {
                transform.localPosition = originalPosition;
                transform.localEulerAngles = originalEulerAngles;
                transform.GetChild(0).localPosition = originalZoom;

                var tempList = m_Animations.ToList();
                if (currentAnimation != null)
                    tempList.Remove(currentAnimation);

                var randomIndex = Random.Range(0, tempList.Count);
                currentAnimation = tempList[randomIndex];

                //TODO: Do something based on TargetType
                var currentEnumerator = currentAnimation.Animate(transform, null);

                while (currentEnumerator.MoveNext()) { yield return null; }
            }

            m_AnimationEnumerator = null;
        }

        private IEnumerator ScreenShakEnumerator()
        {
            var deltaTime = 0f;
            while (deltaTime < m_ScreenShakeTime)
            {
                var xOffset = transform.right * Random.Range(-0.2f, 0.2f);
                var yOffset = transform.up * Random.Range(-0.2f, 0.2f);

                transform.position =
                    (transform.position + xOffset + yOffset) *
                    m_ScreenShakeCurve.Evaluate(deltaTime / m_ScreenShakeTime);

                deltaTime += Time.deltaTime;

                yield return null;
            }
        }
    }
}