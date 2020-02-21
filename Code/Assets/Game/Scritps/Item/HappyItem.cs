using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HappyItem : MonoBehaviour
{
    public TextMesh happyNumber;
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
        GameManager.instance.AddHappy(happy);
        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
