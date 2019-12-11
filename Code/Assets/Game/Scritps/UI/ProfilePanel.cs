﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    public InputField nameInput;
    public Text petName;
    public Button editBButton;
    public Text level;
    public Text exp;
    public Text energy;
    public Image expProgress;
    public Image energyProgress;
    public Text weight;
    public Text dogType;
    public Text health;
    public Text happy;
    public Text stamina;
    public Text strength;
    public Text speed;
    public Text intelligent;
    Pet data;

    public void Load(int id){
        data = GameManager.instance.GetPet(id);
        petName.text = data.petName;
        level.text = "Level " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        energy.text = data.Energy.ToString("F0") + "/" + data.maxEnergy.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        energyProgress.fillAmount = data.energy/data.maxEnergy;
        dogType.text = data.languageItem[0].Name;
        health.text = data.Health.ToString("F0");
        happy.text = data.Happy.ToString("F0");
        stamina.text = data.maxEnergy.ToString("F0");
        speed.text = data.Speed.ToString("F0");
        intelligent.text = data.Intelligent.ToString("F0");
        
        nameInput.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnInputName(){
        data.petName = nameInput.text;
        petName.text = data.petName;
        editBButton.gameObject.SetActive(true);
        nameInput.gameObject.SetActive(false);
        petName.gameObject.SetActive(true);
    }

    public void OnEdit(){
        editBButton.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(true);
        petName.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
