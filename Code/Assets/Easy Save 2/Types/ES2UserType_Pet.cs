using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_Pet : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Pet data = (Pet)obj;
		// Add your writer.Write calls here.
		writer.Write(data.Food);
		writer.Write(data.Water);
		writer.Write(data.Sleep);
		writer.Write(data.Energy);
		writer.Write(data.Health);
		writer.Write(data.Damage);
		writer.Write(data.Shit);
		writer.Write(data.Pee);
		writer.Write(data.Dirty);
		writer.Write(data.Itchi);
		writer.Write(data.Curious);
		writer.Write(data.Intelligent);
		writer.Write(data.Exp);
		writer.Write(data.Speed);
		writer.Write(data.MaxFood);
		writer.Write(data.MaxWater);
		writer.Write(data.MaxShit);
		writer.Write(data.MaxPee);
		writer.Write(data.MaxSleep);
		writer.Write(data.MaxEnergy);
		writer.Write(data.MaxHealth);
		writer.Write(data.MaxDamage);
		writer.Write(data.MaxDirty);
		writer.Write(data.MaxItchi);
		writer.Write(data.MaxCurious);
		writer.Write(data.MaxTimeSick);
		writer.Write(data.RecoveryEnergy);
		writer.Write(data.RecoverHealth);
		writer.Write(data.iD);
		writer.Write(data.iconUrl);
		writer.Write(data.buyPrice);
		writer.Write(data.priceType);
		writer.Write(data.itemState);
		writer.Write(data.prefabName);
		writer.Write(data.isAvailable);
		writer.Write(data.itemTag);
		writer.Write(data.requireEquipments);
		writer.Write(data.requirePets);
		writer.Write(data.requireValue);
		writer.Write(data.requireValueType);
		writer.Write(data.petName);
		writer.Write(data.shopOrder);
		writer.Write(data.speed);
		writer.Write(data.level);
		writer.Write(data.exp);
		writer.Write(data.intelligent);
		writer.Write(data.happy);
		writer.Write(data.food);
		writer.Write(data.water);
		writer.Write(data.sleep);
		writer.Write(data.energy);
		writer.Write(data.health);
		writer.Write(data.damage);
		writer.Write(data.shit);
		writer.Write(data.pee);
		writer.Write(data.dirty);
		writer.Write(data.itchi);
		writer.Write(data.fear);
		writer.Write(data.curious);
		writer.Write(data.timeSick);
		writer.Write(data.maxTimeSick);
		writer.Write(data.isNew);
		writer.Write(data.rareType);
		writer.Write(data.rateFood);
		writer.Write(data.rateWater);
		writer.Write(data.rateSleep);
		writer.Write(data.recoverSleep);
		writer.Write(data.ratePee);
		writer.Write(data.rateShit);
		writer.Write(data.recoverDirty);
		writer.Write(data.recoverHealth);
		writer.Write(data.recoverEnergy);
		writer.Write(data.rateHappy);
		writer.Write(data.maxFood);
		writer.Write(data.maxWater);
		writer.Write(data.maxShit);
		writer.Write(data.maxPee);
		writer.Write(data.maxSleep);
		writer.Write(data.maxEnergy);
		writer.Write(data.maxHealth);
		writer.Write(data.maxDamage);
		writer.Write(data.maxDirty);
		writer.Write(data.maxItchi);
		writer.Write(data.maxCurious);
		writer.Write(data.actionType);
		writer.Write(data.position);
		writer.Write(data.scalePosition);
		writer.Write(data.height);
		writer.Write(data.enviromentType);

	}
	
	public override object Read(ES2Reader reader)
	{
		Pet data = new Pet();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Pet data = (Pet)c;
		// Add your reader.Read calls here to read the data into the object.
		data.Food = reader.Read<System.Single>();
		data.Water = reader.Read<System.Single>();
		data.Sleep = reader.Read<System.Single>();
		data.Energy = reader.Read<System.Single>();
		data.Health = reader.Read<System.Single>();
		data.Damage = reader.Read<System.Single>();
		data.Shit = reader.Read<System.Single>();
		data.Pee = reader.Read<System.Single>();
		data.Dirty = reader.Read<System.Single>();
		data.Itchi = reader.Read<System.Single>();
		data.Curious = reader.Read<System.Single>();
		data.Intelligent = reader.Read<System.Single>();
		data.Exp = reader.Read<System.Int32>();
		data.Speed = reader.Read<System.Single>();
		data.MaxFood = reader.Read<System.Single>();
		data.MaxWater = reader.Read<System.Single>();
		data.MaxShit = reader.Read<System.Single>();
		data.MaxPee = reader.Read<System.Single>();
		data.MaxSleep = reader.Read<System.Single>();
		data.MaxEnergy = reader.Read<System.Single>();
		data.MaxHealth = reader.Read<System.Single>();
		data.MaxDamage = reader.Read<System.Single>();
		data.MaxDirty = reader.Read<System.Single>();
		data.MaxItchi = reader.Read<System.Single>();
		data.MaxCurious = reader.Read<System.Single>();
		data.MaxTimeSick = reader.Read<System.Single>();
		data.RecoveryEnergy = reader.Read<System.Single>();
		data.RecoverHealth = reader.Read<System.Single>();
		data.iD = reader.Read<System.Int32>();
		data.iconUrl = reader.Read<System.String>();
		data.buyPrice = reader.Read<System.Int32>();
		data.priceType = reader.Read<PriceType>();
		data.itemState = reader.Read<ItemState>();
		data.prefabName = reader.Read<System.String>();
		data.isAvailable = reader.Read<System.Boolean>();
		data.itemTag = reader.Read<ItemTag>();
		data.requireEquipments = reader.ReadArray<System.Int32>();
		data.requirePets = reader.ReadArray<System.Int32>();
		data.requireValue = reader.Read<System.Int32>();
		data.requireValueType = reader.Read<PriceType>();
		data.petName = reader.Read<System.String>();
		data.shopOrder = reader.Read<System.Int32>();
		data.speed = reader.Read<System.Single>();
		data.level = reader.Read<System.Int32>();
		data.exp = reader.Read<System.Int32>();
		data.intelligent = reader.Read<System.Single>();
		data.happy = reader.Read<System.Int32>();
		data.food = reader.Read<System.Single>();
		data.water = reader.Read<System.Single>();
		data.sleep = reader.Read<System.Single>();
		data.energy = reader.Read<System.Single>();
		data.health = reader.Read<System.Single>();
		data.damage = reader.Read<System.Single>();
		data.shit = reader.Read<System.Single>();
		data.pee = reader.Read<System.Single>();
		data.dirty = reader.Read<System.Single>();
		data.itchi = reader.Read<System.Single>();
		data.fear = reader.Read<System.Single>();
		data.curious = reader.Read<System.Single>();
		data.timeSick = reader.Read<System.DateTime>();
		data.maxTimeSick = reader.Read<System.Single>();
		data.isNew = reader.Read<System.Boolean>();
		data.rareType = reader.Read<RareType>();
		data.rateFood = reader.Read<System.Single>();
		data.rateWater = reader.Read<System.Single>();
		data.rateSleep = reader.Read<System.Single>();
		data.recoverSleep = reader.Read<System.Single>();
		data.ratePee = reader.Read<System.Single>();
		data.rateShit = reader.Read<System.Single>();
		data.recoverDirty = reader.Read<System.Single>();
		data.recoverHealth = reader.Read<System.Single>();
		data.recoverEnergy = reader.Read<System.Single>();
		data.rateHappy = reader.Read<System.Int32>();
		data.maxFood = reader.Read<System.Single>();
		data.maxWater = reader.Read<System.Single>();
		data.maxShit = reader.Read<System.Single>();
		data.maxPee = reader.Read<System.Single>();
		data.maxSleep = reader.Read<System.Single>();
		data.maxEnergy = reader.Read<System.Single>();
		data.maxHealth = reader.Read<System.Single>();
		data.maxDamage = reader.Read<System.Single>();
		data.maxDirty = reader.Read<System.Single>();
		data.maxItchi = reader.Read<System.Single>();
		data.maxCurious = reader.Read<System.Single>();
		data.actionType = reader.Read<ActionType>();
		data.position = reader.Read<UnityEngine.Vector3>();
		data.scalePosition = reader.Read<UnityEngine.Vector3>();
		data.height = reader.Read<System.Single>();
		data.enviromentType = reader.Read<EnviromentType>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_Pet():base(typeof(Pet)){}
}