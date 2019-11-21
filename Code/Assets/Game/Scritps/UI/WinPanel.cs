using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Text exp;
    public Text coin;
    public Text diamon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Animator animator;

    void Awake(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int star,int e, int d, int c){
        animator = this.GetComponent<Animator>();
        animator.SetInteger("star",star);
        if(e > 0)
            exp.text = e.ToString();
        else
            exp.transform.parent.gameObject.SetActive(false);
        
        if(c > 0)
            coin.text = c.ToString();
        else
            coin.transform.parent.gameObject.SetActive(false);

        if(d > 0)
            diamon.text = d.ToString();
        else
            diamon.transform.parent.gameObject.SetActive(false);
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
