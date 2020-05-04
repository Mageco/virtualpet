using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinItem : MonoBehaviour
{
    public TextMeshPro coinNumber;
    public int lifeTime = 1;
    public ParticleSystem particle;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.Burst burst;

    public void Load(int value)
    {
        if(value > 0)
            coinNumber.text = "+" + value.ToString();
        else
            coinNumber.text = value.ToString();

        if (value < 0)
            coinNumber.color = Color.red;

        int number = Mathf.Abs(value);
        if (number > 10)
            number = 10;
        emissionModule = particle.emission;
        burst = emissionModule.GetBurst(0);
        Debug.Log("Burst Count " + burst.count.constant);
        burst.minCount = (short)number;
        burst.maxCount = (short)number;
        var c = burst.count;
        c.mode = ParticleSystemCurveMode.Constant;
        c.constant = number;
        c.constantMin = number;
        c.constantMax = number;
        emissionModule.SetBurst(0, burst);
        particle.Play();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
