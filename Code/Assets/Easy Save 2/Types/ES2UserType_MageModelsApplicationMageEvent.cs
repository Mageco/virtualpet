using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Application;

public class ES2UserType_MageModelsApplicationMageEvent : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Application.MageEvent data = (Mage.Models.Application.MageEvent)obj;
		// Add your writer.Write calls here.
		writer.Write(data.eventName);
		writer.Write(data.eventDetail);
		writer.Write(data.eventDate);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Application.MageEvent data = new Mage.Models.Application.MageEvent();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Application.MageEvent data = (Mage.Models.Application.MageEvent)c;
		// Add your reader.Read calls here to read the data into the object.
		data.eventName = reader.Read<Mage.Models.Application.MageEventType>();
		data.eventDetail = reader.Read<System.String>();
		data.eventDate = reader.Read<System.DateTime>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsApplicationMageEvent():base(typeof(Mage.Models.Application.MageEvent)){}
}