using System;
using Library;

using UnityEngine;

[Serializable]
public class StaminaInformation
{
    public uint value;
    public string timeLastPlayed;
    public uint maxValue;
}

public class StaminaManager : MonoSingleton<StaminaManager>
{
    [SerializeField]
    private uint m_Value = 0;
    public uint maxValue = 100;
    [SerializeField]
    private float m_StaminaRate = 2.5f;

    private float m_Timer;

    public uint value { get { return m_Value; } }

	public void Start()
	{
	    var playerStaminaInfo = GameManager.self.playerData.staminaInformation; // Get the Stamina Info

	    m_Value = playerStaminaInfo.value;    // Set m_Value to StaminaInfo m_Value
	    maxValue = playerStaminaInfo.maxValue;

        var timeLastPlayed = DateTime.Parse(playerStaminaInfo.timeLastPlayed);   // Get Last Time the app was open 
        var ts = DateTime.Now - timeLastPlayed; // Calculate the time span
        var secondsPassed = 0;

        // Convert it all to Seconds
        secondsPassed += ts.Days * 86164;
        secondsPassed += ts.Hours * 3600;
        secondsPassed += ts.Minutes * 60;
        secondsPassed += ts.Seconds;

        // Add the time that was passed
        m_Value += (uint)(secondsPassed / m_StaminaRate);

        // Limit the m_Value
	    if (m_Value > maxValue)
	    {
	        m_Value = maxValue;
	    }
    }	

	private void Update()
	{
        // If m_Value is at maxValue, don't allow to add time
        if (m_Value >= maxValue)
	    {
            m_Timer = m_StaminaRate;
            return;
        }

        m_Timer -= Time.deltaTime;
	    if (!(m_Timer <= 0))  // As long as timer isn't less than or equal to 0, stop process here
            return;

	    m_Value++;
        m_Timer = m_StaminaRate;
	}
}
