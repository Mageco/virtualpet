
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
	
	

	
	
}