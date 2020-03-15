using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Application;

public class ES2UserType_MageModelsApplicationApplicationData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Application.ApplicationData data = (Mage.Models.Application.ApplicationData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.attr_name);
		writer.Write(data.attr_value);
		writer.Write(data.attr_type);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Application.ApplicationData data = new Mage.Models.Application.ApplicationData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Application.ApplicationData data = (Mage.Models.Application.ApplicationData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.attr_name = reader.Read<System.String>();
		data.attr_value = reader.Read<System.String>();
		data.attr_type = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsApplicationApplicationData():base(typeof(Mage.Models.Application.ApplicationData)){}
}