
using UnityEngine;
using System.Collections;

public class VariableCondition
{
	// game variable
	public string[] variableKey  = new string[0];
	public string[] variableValue = new string[0];
	public bool[] checkType = new bool[0];
	// number variables
	public string[] numberVarKey = new string[0];
	public float[] numberVarValue = new float[0];
	public bool[] numberCheckType = new bool[0];
	public ValueCheck[] numberValueCheck = new ValueCheck[0];
	
	private static string VARIABLEKEY = "variablekey";
	private static string VARIABLEVALUE = "variablevalue";
	private static string NUMBERVARIABLE = "numbervariable";
	
	public VariableCondition()
	{
		
	}
	
	public void AddVariable()
	{
		this.variableKey = ArrayHelper.Add("key", this.variableKey);
		this.variableValue = ArrayHelper.Add("value", this.variableValue);
		this.checkType = ArrayHelper.Add(true, this.checkType);
	}
	
	public void RemoveVariable(int index)
	{
		this.variableKey = ArrayHelper.Remove(index, this.variableKey);
		this.variableValue = ArrayHelper.Remove(index, this.variableValue);
		this.checkType = ArrayHelper.Remove(index, this.checkType);
	}
	
	public void AddNumberVariable()
	{
		this.numberVarKey = ArrayHelper.Add("key", this.numberVarKey);
		this.numberVarValue = ArrayHelper.Add(0, this.numberVarValue);
		this.numberCheckType = ArrayHelper.Add(true, this.numberCheckType);
		this.numberValueCheck = ArrayHelper.Add(ValueCheck.EQUALS, this.numberValueCheck);
	}
	
	public void RemoveNumberVariable(int index)
	{
		this.numberVarKey = ArrayHelper.Remove(index, this.numberVarKey);
		this.numberVarValue = ArrayHelper.Remove(index, this.numberVarValue);
		this.numberCheckType = ArrayHelper.Remove(index, this.numberCheckType);
		this.numberValueCheck = ArrayHelper.Remove(index, this.numberValueCheck);
	}
	
	public bool CheckVariables()
	{
		bool apply = true;
		bool any = false;
		for(int i=0; i<this.variableKey.Length; i++)
		{
			bool check = GameHandler.CheckVariable(this.variableKey[i], this.variableValue[i]);
			
			if((check && this.checkType[i]) || (!check && !this.checkType[i]))
			{
				any = true;
			}
		}
		if(apply)
		{
			for(int i=0; i<this.numberVarKey.Length; i++)
			{
				bool check = GameHandler.CheckNumberVariable(this.numberVarKey[i], 
						this.numberVarValue[i], this.numberValueCheck[i]);
				
				if((check && this.numberCheckType[i]) || (!check && !this.numberCheckType[i]))
				{
					any = true;
				}
			}
		}
		if(!any && 
			(this.variableKey.Length > 0 || this.numberVarKey.Length > 0))
		{
			apply = false;
		}
		return apply;
	}
	


}
