using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StaminaDisplay : MonoBehaviour
{
    private Text m_Text;

	// Use this for initialization
	void Start ()
	{
	    m_Text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    m_Text.text = StaminaManager.self.value + "/" + StaminaManager.self.maxValue;
	}
}
