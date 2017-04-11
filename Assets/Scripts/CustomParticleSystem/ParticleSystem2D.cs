namespace CustomParticleSystem
{
    using System;
    using System.Linq;

    using CustomInput;
    using CustomInput.Information;

    using CustomParticleSystem;

    using UnityEngine;

    using Random = UnityEngine.Random;

    [RequireComponent(typeof(RectTransform))]
    public class ParticleSystem2D : MonoBehaviour
    {
        [SerializeField]
        private Particle2D m_Particle2DPrefab;

        [Space, SerializeField, Range(0.1f, 3f)]
        private float m_ParticleDuration;

        private GameObject m_ParticleAnchor;

        private void Awake()
        {
            InputManager.self.onHold.AddListener(OnHold);
            InputManager.self.onDrag.AddListener(OnDrag);
        }

        private void Start()
        {
            m_ParticleAnchor = new GameObject("Particle Anchor");
            m_ParticleAnchor.transform.SetParent(transform.parent, false);
        }

        private void OnHold(TouchInformation dragInformation)
        {
            CreateParticles();
        }
        private void OnDrag(DragInformation dragInformation)
        {
            CreateParticles();
        }

        private void CreateParticles()
        {
            var newParticle2D = Instantiate(m_Particle2DPrefab);
            newParticle2D.transform.SetParent(m_ParticleAnchor.transform, false);
            newParticle2D.transform.position = transform.position;

            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            newParticle2D.velocity =
                new Vector2(Mathf.Cos(angle) * 7f, Mathf.Sin(angle) * 7f);

            newParticle2D.friction = new Vector2(1f, 1f);
            newParticle2D.duration = m_ParticleDuration;
        }
    }
}
