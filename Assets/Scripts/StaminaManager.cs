using System;
using System.Collections;
using System.Globalization;
using Library;

using UnityEngine;

public class StaminaInformation
{
    public int value;
    public string timeLastPlayed;
}

public class StaminaManager : MonoSingleton<StaminaManager>
{     
    public int value;
    public float staminaRate;

    [SerializeField]
    private DateTime m_TimeLastPlayed;
    private float m_LastFrameTime;

	public void Start()
	{
	    var playerStaminaInfo = GameManager.self.playerData.staminaInformation;

	    value = playerStaminaInfo.value;

        m_TimeLastPlayed = DateTime.Parse(playerStaminaInfo.timeLastPlayed);
        var ts = DateTime.Now - m_TimeLastPlayed;
        var secondsPassed = 0;
        secondsPassed += ts.Days * 86164;
        secondsPassed += ts.Hours * 3600;
        secondsPassed += ts.Minutes * 60;
        secondsPassed += ts.Seconds;

        value += (int)(secondsPassed / staminaRate);
    }	

	private void Update()
	{
	    var currentTime = Time.time%staminaRate;
	    if (currentTime < m_LastFrameTime)
	    {
	        value++;
	    }
	    m_LastFrameTime = currentTime;
    }
}
