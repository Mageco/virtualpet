using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    public static TipManager instance;
    float time = 0;
    float maxTimeCheck = 1;
    float timeTip = 0;
    float maxTimeTip = 20;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            GameObject.Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.mapButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (time > maxTimeCheck)
        {
            time = 0;
            CheckUI();

        }
        else
            time += Time.deltaTime;

        if (timeTip > maxTimeTip)
        {
            timeTip = 0;
            CheckTip();

        }
        else
            timeTip += Time.deltaTime;
    }

    void CheckUI()
    {
        if (GameManager.instance.myPlayer.level >= 3)
            UIManager.instance.mapButton.gameObject.SetActive(true);
        else
            UIManager.instance.mapButton.gameObject.SetActive(false);
    }

    void CheckTip()
    {

    }
}
