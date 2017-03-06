using System.Collections.Generic;
using Library;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(AudioListener))]
public class AudioManager : MonoSingleton<AudioManager>
{
    public List<AudioClip> musicList;
    private AudioSource m_Source;

    public void ChangeMusic(int sceneIndex)
    {
        if (m_Source == null)
        {
            m_Source = gameObject.GetComponent<AudioSource>();
            m_Source.loop = true;
        }
        m_Source.clip = musicList[sceneIndex];
        m_Source.Play();
    }
}
