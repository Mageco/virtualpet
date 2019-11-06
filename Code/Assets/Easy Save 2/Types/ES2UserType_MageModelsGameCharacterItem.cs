using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Game;

public class ES2UserType_MageModelsGameCharacterItem : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Game.CharacterItem data = (Mage.Models.Game.CharacterItem)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.item_local_id);
		writer.Write(data.character_id);
		writer.Write(data.status);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Game.CharacterItem data = new Mage.Models.Game.CharacterItem();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Game.CharacterItem data = (Mage.Models.Game.CharacterItem)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.item_local_id = reader.Read<System.String>();
		data.character_id = reader.Read<System.String>();
		data.status = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsGameCharacterItem():base(typeof(Mage.Models.Game.CharacterItem)){}
}