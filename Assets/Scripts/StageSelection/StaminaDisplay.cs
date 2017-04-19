using UnityEngine;
using UnityEngine.UI;

namespace StageSelection
{
    public class StaminaDisplay : MonoBehaviour
    {
        private Text m_Text;
        [SerializeField]
        private Image m_FillImage;

        // Use this for initialization
        void Start ()
        {
            
            m_Text = transform.GetComponentInChildren<Text>();
        }
	
        // Update is called once per frame
        void Update ()
        {
            m_FillImage.fillAmount = (float)StaminaManager.self.value / StaminaManager.self.maxValue;
            m_Text.text = StaminaManager.self.value + "/" + StaminaManager.self.maxValue;
        }
    }
}
