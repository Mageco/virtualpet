
using System.Collections;
using UnityEditor;
using UnityEngine;

public class EditorHelper
{
	public static float mWidth = 250;
	
	// base stuff
	public static string[] CheckLanguageCount(string[] text)
	{
		return EditorHelper.CheckLanguageCount(text, DataHolder.Languages().GetDataCount());
	}
	
	public static string[] CheckLanguageCount(string[] text, int count)
	{
		if(count != text.Length)
		{
			string[] dmy = text;
			text = new string[count];
			for(int i=0; i<count; i++)
			{
				if(i<dmy.Length)
				{
					text[i] = dmy[i];
				}
				if(text[i] == null)
				{
					text[i] = "";
				}
			}
		}
		return text;
	}
	
	/*
	============================================================================
	Settings functions
	============================================================================
	*/
	public static ValueChange ValueChangeSettings(int index, ValueChange vc)
	{
		vc.active = EditorGUILayout.BeginToggleGroup(DataHolder.StatusValues().GetName(index), vc.active);
		if(vc.active)
		{
			vc.simpleOperator = (SimpleOperator)EditorTab.EnumToolbar("Operator", (int)vc.simpleOperator, typeof(SimpleOperator));
			vc.formulaChooser = (FormulaChooser)EditorTab.EnumToolbar("", (int)vc.formulaChooser, 
					typeof(FormulaChooser), (int)(EditorHelper.mWidth*1.5f));
			if(FormulaChooser.VALUE.Equals(vc.formulaChooser))
			{
				vc.value = EditorGUILayout.IntField("Value", vc.value, GUILayout.Width(EditorHelper.mWidth));
			}
			else if(FormulaChooser.STATUS.Equals(vc.formulaChooser))
			{
				vc.status = EditorGUILayout.Popup("Status Value", vc.status, 
						DataHolder.StatusValues().GetNameList(true), GUILayout.Width(EditorHelper.mWidth));
			}
			else if(FormulaChooser.RANDOM.Equals(vc.formulaChooser))
			{
				vc.randomMin = EditorGUILayout.IntField("Minimum", vc.randomMin, GUILayout.Width(EditorHelper.mWidth));
				vc.randomMax= EditorGUILayout.IntField("Maximum", vc.randomMax, GUILayout.Width(EditorHelper.mWidth));
			}
			vc.efficiency = EditorGUILayout.FloatField("Efficiency", vc.efficiency, GUILayout.Width(EditorHelper.mWidth));
		}
		EditorGUILayout.EndToggleGroup();
		EditorGUILayout.Separator();
		return vc;
	}
	
	public static VariableCondition VariableConditionSettings(VariableCondition vars)
	{
		
		if(GUILayout.Button("Add Variable", GUILayout.Width(mWidth)))
		{
			vars.AddVariable();
		}
		for(int i=0; i<vars.variableKey.Length; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Remove", GUILayout.Width(mWidth*0.5f)))
			{
				vars.RemoveVariable(i);
				break;
			}
			vars.checkType[i] = EditorGUILayout.Toggle(
					vars.checkType[i], GUILayout.Width(20));
			vars.variableKey[i] = EditorGUILayout.TextField(
					vars.variableKey[i], GUILayout.Width(mWidth*0.5f));
			
			if(vars.checkType[i]) GUILayout.Label("== ");
			else GUILayout.Label(" != ");
			
			vars.variableValue[i] = EditorGUILayout.TextField(
					vars.variableValue[i], GUILayout.Width(mWidth*0.5f));
			
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		if(GUILayout.Button("Add Number Variable", GUILayout.Width(mWidth)))
		{
			vars.AddNumberVariable();
		}
		for(int i=0; i<vars.numberVarKey.Length; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Remove", GUILayout.Width(mWidth*0.5f)))
			{
				vars.RemoveNumberVariable(i);
				break;
			}
			
			vars.numberCheckType[i] = EditorGUILayout.Toggle(
					vars.numberCheckType[i], GUILayout.Width(20));
			vars.numberVarKey[i] = EditorGUILayout.TextField(
					vars.numberVarKey[i], GUILayout.Width(mWidth*0.5f));
			if(!vars.numberCheckType[i]) GUILayout.Label("not");
			vars.numberValueCheck[i] = (ValueCheck)EditorGUILayout.EnumPopup(
					vars.numberValueCheck[i], GUILayout.Width(mWidth*0.4f));
			vars.numberVarValue[i] = EditorGUILayout.FloatField(
					vars.numberVarValue[i], GUILayout.Width(mWidth*0.5f));
			
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.Separator();
		return vars;
	}
	
	public static StatusRequirement StatusRequirementSettings(StatusRequirement req)
	{
		EditorGUILayout.BeginHorizontal();
		req.statusNeeded = (StatusNeeded)EditorTab.EnumToolbar("Needed", (int)req.statusNeeded, typeof(StatusNeeded), 300);
		if(StatusNeeded.STATUS_VALUE.Equals(req.statusNeeded))
		{
			req.statID = EditorGUILayout.Popup(req.statID, DataHolder.StatusValues().GetNameList(true), GUILayout.Width(mWidth*0.5f));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			req.comparison = (ValueCheck)EditorTab.EnumToolbar("Comparison", (int)req.comparison, typeof(ValueCheck), 300);
			req.value = EditorGUILayout.IntField(req.value, GUILayout.Width(mWidth*0.5f));
			req.setter = (ValueSetter)EditorTab.EnumToolbar("", (int)req.setter, typeof(ValueSetter));
		}
		else if(StatusNeeded.SKILL.Equals(req.statusNeeded))
		{
			req.statID = EditorGUILayout.Popup(req.statID, DataHolder.Skills().GetNameList(true), GUILayout.Width(mWidth*0.5f));
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		return req;
	}
	


}