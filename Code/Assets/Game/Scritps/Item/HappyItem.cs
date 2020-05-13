using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HappyItem : MonoBehaviour
{ 
    public TextMeshPro happyNumber;
    public int lifeTime = 1;
    public ParticleSystem particle;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.Burst burst;
    int happy;

    public void Load(int value)
    {
        happy = value;
        happyNumber.text = "+" + value.ToString();
        emissionModule = particle.emission;
        burst = emissionModule.GetBurst(0);
        //Debug.Log("Burst Count " + burst.count.constant);
        if (value >= 6)
            value = 6;
        burst.minCount = (short)value;
        burst.maxCount = (short)value;
        var c = burst.count;
        c.mode = ParticleSystemCurveMode.Constant;
        c.constant = value;
        c.constantMin = value;
        c.constantMax = value;
        emissionModule.SetBurst(0, burst);
        particle.Play();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        GameManager.instance.AddHappy(happy, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
