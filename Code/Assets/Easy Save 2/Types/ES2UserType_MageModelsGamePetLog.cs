using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Game;

public class ES2UserType_MageModelsGamePetLog : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Game.PetLog data = (Mage.Models.Game.PetLog)obj;
		// Add your writer.Write calls here.
		writer.Write(data.itemId);
		writer.Write(data.action);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Game.PetLog data = new Mage.Models.Game.PetLog();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Game.PetLog data = (Mage.Models.Game.PetLog)c;
		// Add your reader.Read calls here to read the data into the object.
		data.itemId = reader.Read<System.Int32>();
		data.action = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsGamePetLog():base(typeof(Mage.Models.Game.PetLog)){}
}