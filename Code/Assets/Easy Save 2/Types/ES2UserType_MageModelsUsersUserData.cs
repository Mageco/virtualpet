using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Users;

public class ES2UserType_MageModelsUsersUserData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Users.UserData data = (Mage.Models.Users.UserData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.user_id);
		writer.Write(data.attr_name);
		writer.Write(data.attr_value);
		writer.Write(data.attr_type);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Users.UserData data = new Mage.Models.Users.UserData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Users.UserData data = (Mage.Models.Users.UserData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.user_id = reader.Read<System.String>();
		data.attr_name = reader.Read<System.String>();
		data.attr_value = reader.Read<System.String>();
		data.attr_type = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsUsersUserData():base(typeof(Mage.Models.Users.UserData)){}
}