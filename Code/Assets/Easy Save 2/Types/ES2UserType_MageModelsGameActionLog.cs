using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Game;

public class ES2UserType_MageModelsGameActionLog : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Game.ActionLog data = (Mage.Models.Game.ActionLog)obj;
		// Add your writer.Write calls here.
		writer.Write(data.sequence);
		writer.Write(data.action_date);
		writer.Write(data.action_detail);
		writer.Write(data.time_stamp);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Game.ActionLog data = new Mage.Models.Game.ActionLog();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Game.ActionLog data = (Mage.Models.Game.ActionLog)c;
		// Add your reader.Read calls here to read the data into the object.
		data.sequence = reader.Read<System.Int32>();
		data.action_date = reader.Read<System.String>();
		data.action_detail = reader.Read<System.String>();
		data.time_stamp = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsGameActionLog():base(typeof(Mage.Models.Game.ActionLog)){}
}