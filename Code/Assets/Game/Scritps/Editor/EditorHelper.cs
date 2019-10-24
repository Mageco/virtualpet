
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
	
	
}