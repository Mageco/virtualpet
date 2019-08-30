﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharData
{
	public float food = 50;
	public float water = 50;
	public float sleep = 50;
	public float energy = 50;
	public float health = 50;
	public float shit = 0;
	public float pee = 0;
	public float happy = 50;
	public float stamina = 50;
	public float dirty = 50;
	public float wet = 0;

	[HideInInspector]
	public float basicEnergyConsume = 0.05f;
	[HideInInspector]
	public float actionEnergyConsume = 0;
	[HideInInspector]
	public float healthConsume = 0.01f;
	[HideInInspector]
	public float staminaConsume = 0.001f;
	[HideInInspector]
	public float dirtyFactor = 0.01f;
	[HideInInspector]
	public float happyConsume = 0.01f;
	[HideInInspector]
	public float sleepConsume = 0.05f;


	[HideInInspector]
	public float maxFood = 100;
	[HideInInspector]
	public float maxWater = 100;
	[HideInInspector]
	public float maxShit = 100;
	[HideInInspector]
	public float maxPee = 100;
	[HideInInspector]
	public float maxSleep = 100;
	[HideInInspector]
	public float maxEnergy = 100;
	[HideInInspector]
	public float maxHealth = 100;
	[HideInInspector]
	public float maxHappy = 100;
	[HideInInspector]
	public float maxStamina = 100;
	[HideInInspector]
	public float maxDirty = 100;
	[HideInInspector]
	public float maxWet = 100;


	void Load()
	{

	}

	void Save()
	{

	}

	public float Food
	{
		get
		{
			return this.food;
		}
		set
		{
			this.food = value;
			if (this.food < 0)
				this.food = 0;
			else if (this.food > maxFood)
				this.food = maxFood;
		}
	}

	public float Water
	{
		get
		{
			return this.water;
		}
		set
		{
			this.water = value;
			if (this.water < 0)
				this.water = 0;
			else if (this.water > maxWater)
				this.water = maxWater;
		}
	}

	public float Sleep
	{
		get
		{
			return this.sleep;
		}
		set
		{
			this.sleep = value;
			if (this.sleep < 0)
				this.sleep = 0;
			else if (this.sleep > maxSleep)
				this.sleep = maxSleep;
		}
	}

	public float Energy
	{
		get
		{
			return this.energy;
		}
		set
		{
			this.energy = value;
			if (this.energy < 0)
				this.energy = 0;
			else if (this.energy > maxEnergy)
				this.energy = maxEnergy;
		}
	}

	public float Health
	{
		get
		{
			return this.health;
		}
		set
		{
			this.health = value;
			if (this.health < 0)
				this.health = 0;
			else if (this.health > maxHealth)
				this.health = maxHealth;
		}
	}

	public float Shit
	{
		get
		{
			return this.shit;
		}
		set
		{
			this.shit = value;
			if (this.shit < 0)
				this.shit = 0;
			else if (this.shit > maxShit)
				this.shit = maxShit;
		}
	}

	public float Pee
	{
		get
		{
			return this.pee;
		}
		set
		{
			this.pee = value;
			if (this.pee < 0)
				this.pee = 0;
			else if (this.pee > maxPee)
				this.pee = maxPee;
		}
	}

	public float Happy
	{
		get
		{
			return this.happy;
		}
		set
		{
			this.happy = value;
			if (this.happy < 0)
				this.happy = 0;
			else if (this.happy > maxHappy)
				this.happy = maxHappy;
		}
	}

	public float Stamina
	{
		get
		{
			return this.stamina;
		}
		set
		{
			this.stamina = value;
			if (this.stamina < 0)
				this.stamina = 0;
			else if (this.stamina > maxStamina)
				this.stamina = maxStamina;
		}
	}

	public float Dirty
	{
		get
		{
			return this.dirty;
		}
		set
		{
			this.dirty = value;
			if (this.dirty < 0)
				this.dirty = 0;
			else if (this.dirty > maxDirty)
				this.dirty = maxDirty;
		}
	}

	public float Wet
	{
		get
		{
			return this.wet;
		}
		set
		{
			this.wet = value;
			if (this.wet < 0)
				this.wet = 0;
			else if (this.wet > maxWet)
				this.wet = maxWet;
		}
	}

}
