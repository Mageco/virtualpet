using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceUI : MonoBehaviour
{
    public ServiceType serviceType;
    public Image icon;
    public Text time;
    public Button readyButton;
    PlayerService service;
    public GameObject readyPanel;
    public GameObject activePanel;

    private void Start()
    {
        icon.sprite = Resources.Load<Sprite>("Icons/NPC_Icon/" + serviceType.ToString());
    }

    public void Load()
    {
        service = ServiceManager.instance.GetService(serviceType);
        if (service != null && service.isActive)
        {
            activePanel.SetActive(true);
            readyButton.interactable = false;
            readyPanel.SetActive(false);
        }
        else
        {
            readyButton.interactable = true ;
            activePanel.SetActive(false);
            readyPanel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (service != null && service.isActive)
        {
            time.text = service.GetTime();
        }
    }

    public void OnService()
    {
        UIManager.instance.OnServicePanel();
    }
}
