
using UnityEngine;
using System.Collections;

public class ValueChange
{
	public bool active = false;
	public SimpleOperator simpleOperator = SimpleOperator.ADD;
	public FormulaChooser formulaChooser = FormulaChooser.VALUE;
	public int value = 0;
	public int status = 0;
	public int formula = 0;
	public int randomMin = 0;
	public int randomMax = 0;
	public float efficiency = 1.0f;
	
	public ValueChange()
	{
		
	}
	
	public ValueChange(Hashtable ht)
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
		ht.Add("operator", this.simpleOperator.ToString());
		ht.Add("formulachooser", this.formulaChooser.ToString());
		if(this.efficiency != 1.0f) ht.Add("efficiency", this.efficiency.ToString());
		if(FormulaChooser.VALUE.Equals(this.formulaChooser))
		{
			ht.Add("value", this.value.ToString());
		}
		else if(FormulaChooser.STATUS.Equals(this.formulaChooser))
		{
			ht.Add("status", this.status.ToString());
		}
		else if(FormulaChooser.RANDOM.Equals(this.formulaChooser))
		{
			ht.Add("min", this.randomMin.ToString());
			ht.Add("max", this.randomMax.ToString());
		}
		return ht;
	}
	
	public void SetData(Hashtable ht)
	{
		this.active = true;
		this.simpleOperator = (SimpleOperator)System.Enum.Parse(typeof(SimpleOperator), (string)ht["operator"]);
		this.formulaChooser = (FormulaChooser)System.Enum.Parse(typeof(FormulaChooser), (string)ht["formulachooser"]);
		if(ht.ContainsKey("efficiency")) this.efficiency = float.Parse((string)ht["efficiency"]);
		
		if(FormulaChooser.VALUE.Equals(this.formulaChooser))
		{
			this.value = int.Parse((string)ht["value"]);
		}
		else if(FormulaChooser.STATUS.Equals(this.formulaChooser))
		{
			this.status = int.Parse((string)ht["status"]);
		}
		else if(FormulaChooser.RANDOM.Equals(this.formulaChooser))
		{
			this.randomMin = int.Parse((string)ht["min"]);
			this.randomMax = int.Parse((string)ht["max"]);
		}
	}
	

	
	public bool IsAdd()
	{
		return SimpleOperator.ADD.Equals(this.simpleOperator);
	}
	
	public bool IsSub()
	{
		return SimpleOperator.SUB.Equals(this.simpleOperator);
	}
	
	public bool IsSet()
	{
		return SimpleOperator.SET.Equals(this.simpleOperator);
	}
}