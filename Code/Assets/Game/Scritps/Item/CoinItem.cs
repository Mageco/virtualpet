using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    public TextMesh coinNumber;
    public int lifeTime = 1;
    public ParticleSystem particle;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.Burst burst;

    public void Load(int value)
    {
        coinNumber.text = "+" + value.ToString();
        emissionModule = particle.emission;
        burst = emissionModule.GetBurst(0);
        Debug.Log("Burst Count " + burst.count.constant);
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
        Destroy(this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
