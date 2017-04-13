namespace StageSelection
{
    using UnityEngine;
    using UnityEngine.UI;

    public class RotateTransform : MonoBehaviour
    {
        [SerializeField]
        private float m_RotationSpeed;
        
        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0f, 0f, m_RotationSpeed * Time.deltaTime);
        }
    }
}
