using UnityEngine;

namespace StageSelection
{
    public class SwitchPanels : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_PopUpMenu;
        private Transform m_DataPanel;
        private Transform m_ItemPanel;

        // Use this for initialization
        private void Awake ()
        {
            m_PopUpMenu.SetActive(false);
            m_ItemPanel = transform.GetChild(0);
            m_DataPanel = transform.GetChild(1);
        }
	
        // Update is called once per frame
        public void SwitchPanelTabs()
        {
            var temp = 0;

            temp = m_ItemPanel.GetSiblingIndex();
            m_ItemPanel.SetSiblingIndex(m_DataPanel.GetSiblingIndex());
            m_DataPanel.SetSiblingIndex(temp);

            var boolin = (m_ItemPanel.GetSiblingIndex() != 0);

            m_PopUpMenu.SetActive(boolin);
        }
    }
}
