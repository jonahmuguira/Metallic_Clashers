using UnityEngine;

namespace Combat
{
    using System.Collections;

    using CustomInput;
    using CustomInput.Information;

    public class TargetingEnemy : MonoBehaviour
    {
        private EnemyMono m_CurrentEnemyMono;
        private IEnumerator m_AnimateCorutine;
        private Vector3 originalPosition;
        private bool rising = true;

        public Transform marker;

        private void Start ()
        {
            m_AnimateCorutine = Animate();
            originalPosition = marker.position;
            m_CurrentEnemyMono = CombatManager.self.currentEnemy;
		    CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
            InputManager.self.onPress.AddListener(OnPress);
            StartCoroutine(m_AnimateCorutine);
        }
	
        private void OnCombatUpdate()
        {
            if (CombatCamera.isAnimating)
                return;

            var center = m_CurrentEnemyMono.GetComponent<MeshRenderer>().bounds.center;

            transform.position = center;
        }

        private void OnPress(TouchInformation touchInfo)
        {
            if (CombatCamera.isAnimating)
                return;

            var ray = Camera.main.ScreenPointToRay(touchInfo.position);
            var hit = new RaycastHit();

            try
            {
                Physics.Raycast(ray.origin, ray.direction, out hit);
            }
            catch { }

            if (hit.transform == null)
                return;

            var gameOb = hit.transform.gameObject;
            if (gameOb == m_CurrentEnemyMono.gameObject)
            {
                var currenyEnemyBounds = m_CurrentEnemyMono.GetComponent<MeshRenderer>().bounds;
                marker.position = currenyEnemyBounds.center +
                    new Vector3(0, currenyEnemyBounds.extents.y + .5f, 0);

                CombatManager.self.currentEnemy = m_CurrentEnemyMono;
                CombatCamera.isAnimating = true;
                StartCoroutine(m_AnimateCorutine);
            }
            else if (gameOb.GetComponent<EnemyMono>() != m_CurrentEnemyMono)
            {
                StopCoroutine(m_AnimateCorutine);
                m_CurrentEnemyMono = gameOb.GetComponent<EnemyMono>();
                marker.position = m_CurrentEnemyMono.transform.position + new Vector3(0, 1f, 0);
            }
            else
            {
                return;
            }
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                if (rising)
                {
                    marker.position += new Vector3(0, 2 *Time.deltaTime, 0);
                }

                else
                {
                    marker.position -= new Vector3(0, Time.deltaTime, 0);
                }

                if (marker.position.y > originalPosition.y + .5f)
                {
                    rising = false;
                }
                    

                if (marker.position.y < originalPosition.y - .5f)
                {
                    rising = true;
                }

                yield return null;
            }
        }
    }
}