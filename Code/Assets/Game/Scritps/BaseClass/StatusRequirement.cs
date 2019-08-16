
using UnityEngine;
using System.Collections;

public class StatusRequirement
{
	public StatusNeeded statusNeeded = StatusNeeded.STATUS_VALUE;
	public int statID = 0;
	public ValueCheck comparison = ValueCheck.EQUALS;
	public int value = 0;
	public ValueSetter setter = ValueSetter.VALUE;
	
	public StatusRequirement()
	{
		
	}
	
	public StatusRequirement(Hashtable ht)
	{
		this.SetData(ht);
	}
	
	/*
	============================================================================
	Data handling functions
	============================================================================
	*/
	public Hashtable GetData(Hashtable ht)
	{
		ht.Add("statusneeded", this.statusNeeded.ToString());
		ht.Add("statid", this.statID.ToString());
		ht.Add("comparison", this.comparison.ToString());
		ht.Add("value", this.value.ToString());
		ht.Add("setter", this.setter.ToString());
		return ht;
	}
	
	public void SetData(Hashtable ht)
	{
		this.statusNeeded = (StatusNeeded)System.Enum.Parse(typeof(StatusNeeded), (string)ht["statusneeded"]);
		this.comparison = (ValueCheck)System.Enum.Parse(typeof(ValueCheck), (string)ht["comparison"]);
		this.setter = (ValueSetter)System.Enum.Parse(typeof(ValueSetter), (string)ht["setter"]);
		this.statID = int.Parse((string)ht["statid"]);
		this.value = int.Parse((string)ht["value"]);
	}
	

}
