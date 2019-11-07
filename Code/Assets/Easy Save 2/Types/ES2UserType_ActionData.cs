using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_ActionData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		ActionData data = (ActionData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.actionType);
		writer.Write(data.startTime);
		writer.Write(data.endTime);

	}
	
	public override object Read(ES2Reader reader)
	{
		ActionData data = new ActionData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		ActionData data = (ActionData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.actionType = reader.Read<ActionType>();
		data.startTime = reader.Read<System.DateTime>();
		data.endTime = reader.Read<System.DateTime>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_ActionData():base(typeof(ActionData)){}
}