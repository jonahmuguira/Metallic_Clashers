using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ParticleSystem2D : MonoBehaviour
{
    [SerializeField]
    private Particle2D m_Particle2DPrefab;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        var newParticle2D = new GameObject().AddComponent<Particle2D>();
        newParticle2D.transform.SetParent(transform, false);
    }
}
