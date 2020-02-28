using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollItem : MonoBehaviour
{
    bool isActive = false;
    Animator animator;
    public ToyType toyType;
    bool isOn = false;

    void Awake(){
        animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    void OnMouseUp(){
        if(!isActive){
            isActive = true;
            StartCoroutine(OnActive());
        }
        
    }

    IEnumerator OnActive(){
        if(toyType == ToyType.Doll)
        {
            if (!isOn)
            {
                MageManager.instance.PlayMusicName("Are_You_Sleeping", true);
                isOn = true;
            }
            else
            {
                EnviromentManager.instance.LoadMusic();
                isOn = false;
            }
        }
        else if(toyType == ToyType.SpaceShip)
        {
            MageManager.instance.PlaySound3D("Item_Space", false,this.transform.position);
            MageManager.instance.PlaySound3D("Whistle_slide_up_01", false,this.transform.position);
        }
        animator.Play("Active");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isActive = false;
        animator.Play("Idle");
        MageManager.instance.PlaySound3D("whoosh_swish_med_03", false,this.transform.position);
    }

    private void OnDestroy()
    {
        if(isOn)
            EnviromentManager.instance.LoadMusic();
    }


}
