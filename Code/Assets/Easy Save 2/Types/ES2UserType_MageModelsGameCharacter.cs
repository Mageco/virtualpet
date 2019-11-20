using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Game;

public class ES2UserType_MageModelsGameCharacter : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Game.Character data = (Mage.Models.Game.Character)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.character_name);
		writer.Write(data.character_type);
		writer.Write(data.status);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Game.Character data = new Mage.Models.Game.Character();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Game.Character data = (Mage.Models.Game.Character)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.character_name = reader.Read<System.String>();
		data.character_type = reader.Read<System.String>();
		data.status = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsGameCharacter():base(typeof(Mage.Models.Game.Character)){}
}