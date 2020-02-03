using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Application;

public class ES2UserType_MageModelsApplicationEventCounter : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Application.EventCounter data = (Mage.Models.Application.EventCounter)obj;
		// Add your writer.Write calls here.
		writer.Write(data.Key);
		writer.Write(data.Value);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Application.EventCounter data = new Mage.Models.Application.EventCounter();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Application.EventCounter data = (Mage.Models.Application.EventCounter)c;
		// Add your reader.Read calls here to read the data into the object.
		data.Key = reader.Read<System.String>();
		data.Value = reader.Read<System.Int32>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsApplicationEventCounter():base(typeof(Mage.Models.Application.EventCounter)){}
}