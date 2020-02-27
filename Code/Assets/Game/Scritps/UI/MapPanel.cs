using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPanel : MonoBehaviour
{
    public Button[] mapIcons;


    private void Awake()
    {
        LoadMap();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadMap()
    {
        for(int i = 0; i < mapIcons.Length; i++)
        {
            if(GameManager.instance.myPlayer.level >= i*i + 2 * i)
            {
                mapIcons[i].interactable = true;

                if(i == 1)
                    mapIcons[i].GetComponentInChildren<Text>().text = DataHolder.Dialog(72).GetName(MageManager.instance.GetLanguage());
                else
                    mapIcons[i].GetComponentInChildren<Text>().gameObject.SetActive(false);
                Image[] images = mapIcons[i].GetComponentsInChildren<Image>();
                for(int j = 0; j < images.Length; j++)
                {
                    images[j].material = null;
                }
            }
            else
            {
                mapIcons[i].interactable = false;
                mapIcons[i].GetComponentInChildren<Text>().text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + (i * i + 2 * i).ToString();
            }

            int id = i;
            mapIcons[i].onClick.AddListener(delegate { OnScene(id);});
        }
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

    public void OnScene(int id)
    {
        MapType mapType = (MapType)id;
        if (SceneManager.GetActiveScene().name != mapType.ToString())
        {
            if(mapType == MapType.House)
            {
                UIManager.instance.OnMap(mapType);
                Close();
            }
            else if(mapType == MapType.Forest)
            {
                UIManager.instance.OnMapRequirement(mapType);
            }
            else
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(53).GetName(MageManager.instance.GetLanguage()));
            }

        }
            
        else
            Close();
    }
}
