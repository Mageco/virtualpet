using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    public static TipManager instance;
    float time = 0;
    float maxTimeCheck = 1.3f;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time > maxTimeCheck)
        {
            time = 0;

        }
        else
            time += Time.deltaTime;
    }

}
