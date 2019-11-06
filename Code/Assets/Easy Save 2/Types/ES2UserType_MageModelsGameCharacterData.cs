using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Game;

public class ES2UserType_MageModelsGameCharacterData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Game.CharacterData data = (Mage.Models.Game.CharacterData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.character_id);
		writer.Write(data.attr_name);
		writer.Write(data.attr_value);
		writer.Write(data.attr_type);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Game.CharacterData data = new Mage.Models.Game.CharacterData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Game.CharacterData data = (Mage.Models.Game.CharacterData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.character_id = reader.Read<System.String>();
		data.attr_name = reader.Read<System.String>();
		data.attr_value = reader.Read<System.String>();
		data.attr_type = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsGameCharacterData():base(typeof(Mage.Models.Game.CharacterData)){}
}