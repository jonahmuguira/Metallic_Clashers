using System.Collections.Generic;
using Input.Information;

using StageSelection;

using UnityEngine;
using UnityEngine.EventSystems;

public class AudioManager : SubManager<AudioManager>
{
    public List<AudioClip> musicList;
    public AudioClip clickSound;

    private AudioSource m_MusicSource;
    private AudioSource m_MenuSource;

    public void ChangeMusic(int sceneIndex)
    {
        if (m_MusicSource == null)
        {
            m_MusicSource = gameObject.AddComponent<AudioSource>();
            m_MusicSource.loop = true;
        }
        m_MusicSource.clip = musicList[sceneIndex];
        m_MusicSource.Play();
    }

    protected override void OnPress(TouchInformation touchInfo)
    {
        if(EventSystem.current.currentSelectedGameObject != null)
            m_MenuSource.Play();

        // See if Hit certain GameObject
        var ray = Camera.main.ScreenPointToRay(touchInfo.position);
        var hit = new RaycastHit();                                     // Make a Hit

        //See if the ray hit anything
        if (!(Physics.Raycast(ray.origin, ray.direction, out hit)))       // If not, stop execution
            return;

        var obj = hit.transform.gameObject;
        if(obj.GetComponent<MonoNode>() != null)
            m_MenuSource.Play();
    }

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);

        m_MenuSource = gameObject.AddComponent<AudioSource>();
        m_MenuSource.clip = clickSound;

        ChangeMusic(0);
    }
}