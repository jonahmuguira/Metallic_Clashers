using System;
using System.Collections;

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
    private DateTime m_TimeLastPlayed = DateTime.Now;
    private float m_LastFrameTime;

	public void Start()
	{
	    var playerDateLastPlayer = GameManager.self.playerData.dateLastPlayed;
	    if (playerDateLastPlayer == null)
	    {
            playerDateLastPlayer = new StaminaInformation
            {
                timeLastPlayed = DateTime.Now.ToString("u")
            };
	        return;
	    }

        m_TimeLastPlayed = DateTime.Parse(playerDateLastPlayer.timeLastPlayed);

	    var yearChange = DateTime.Now.Year - m_TimeLastPlayed.Year;
	}	

	private IEnumerable UpdateStamina()
	{
	    var currentTime = Time.time%staminaRate;
        if (currentTime < m_LastFrameTime)
        {
            value++;
        }
        m_LastFrameTime = currentTime;
	    yield return null;
	}
}
