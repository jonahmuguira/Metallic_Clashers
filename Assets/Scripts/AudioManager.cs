using System.Collections.Generic;
using Input.Information;
using UnityEngine;

public class AudioManager : SubManager<AudioManager>
{
    public List<AudioClip> musicList;
    public AudioClip clickSound;

    private AudioSource m_MusicSource;
    private AudioSource m_MenuSource;

    public void ChangeMusic(int sceneIndex)
    {
        m_MusicSource.clip = musicList[sceneIndex];
        m_MusicSource.Play();
    }

    protected override void OnPress(TouchInformation touchInfo)
    {
        m_MenuSource.Play();
    }

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);

        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_MusicSource.loop = true;

        m_MenuSource = gameObject.AddComponent<AudioSource>();
        m_MenuSource.clip = clickSound;
    }
}