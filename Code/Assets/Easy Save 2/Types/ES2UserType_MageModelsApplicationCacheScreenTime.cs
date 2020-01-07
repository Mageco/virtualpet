using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Application;

public class ES2UserType_MageModelsApplicationCacheScreenTime : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Application.CacheScreenTime data = (Mage.Models.Application.CacheScreenTime)obj;
		// Add your writer.Write calls here.
		writer.Write(data.Key);
		writer.Write(data.Value);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Application.CacheScreenTime data = new Mage.Models.Application.CacheScreenTime();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Application.CacheScreenTime data = (Mage.Models.Application.CacheScreenTime)c;
		// Add your reader.Read calls here to read the data into the object.
		data.Key = reader.Read<System.String>();
		data.Value = reader.Read<System.Double>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsApplicationCacheScreenTime():base(typeof(Mage.Models.Application.CacheScreenTime)){}
}