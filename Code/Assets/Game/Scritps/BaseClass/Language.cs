
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Language
{
	public string[] text = new string[1];
	
	public Language()
	{
		text [0] = "";
	}
	
	public Language(string s)
	{
		this.text = new string[1];
		text [0] = s;
	}
	
	public Language(string[] add)
	{
		this.text = new string[add.Length];
		System.Array.Copy(add, this.text, add.Length);
	}
	
	public Language(int size)
	{
		this.text = new string[size];
		for(int i=0; i<size; i++)  this.text[i] = "";
	}

	public int GetLanguage(string c)
	{
		for (int i = 0; i < text.Length; i++) {
			if (text [i] == c)
				return i;
		}
		return -1;
	}
	
	/*
	============================================================================
	Utility functions
	============================================================================
	*/
	public void Add(string add)
	{
		this.text = ArrayHelper.Add(add, this.text);
	}

	public void Remove(int index)
	{
		this.text = ArrayHelper.Remove(index, this.text);
	}
	
	public void Copy(int index)
	{
		if(index >= 0 && index < this.text.Length)
		{
			this.Add(this.text[index]);
		}
	}
	
	public int Count()
	{
		return this.text.Length;
	}
}
