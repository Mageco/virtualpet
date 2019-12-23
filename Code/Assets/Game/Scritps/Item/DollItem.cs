using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollItem : MonoBehaviour
{
    float activeTime = 0;
    bool isActive = false;
    Animator animator;
    public ToyType toyType;
    bool isOn = false;
    int soundId = 0;

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
            MageManager.instance.PlaySoundName("Item_Space", false);
            MageManager.instance.PlaySoundName("Whistle_slide_up_01", false);
        }
        animator.Play("Active");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isActive = false;
        animator.Play("Idle");
        MageManager.instance.PlaySoundName("whoosh_swish_med_03", false);
    }

    private void OnDestroy()
    {
        if(isOn)
            EnviromentManager.instance.LoadMusic();
    }


}
