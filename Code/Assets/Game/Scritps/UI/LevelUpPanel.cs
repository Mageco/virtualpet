using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    public Transform anchor;
    public GameObject itemPrefab;
    public Text coin;
    public Text diamon;
    List<GameObject> icons = new List<GameObject>();
    Animator anim;


    void Awake()
    {
        anim = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Load()
    {
       


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
