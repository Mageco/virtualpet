using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Users;

public class ES2UserType_MageModelsUsersUser : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Users.User data = (Mage.Models.Users.User)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.uuid);
		writer.Write(data.username);
		writer.Write(data.password);
		writer.Write(data.facebook_id);
		writer.Write(data.authentication_method);
		writer.Write(data.fullname);
		writer.Write(data.phone);
		writer.Write(data.email);
		writer.Write(data.avatar);
		writer.Write(data.status);
		writer.Write(data.notification_token);
		writer.Write(data.country_code);
		writer.Write(data.user_datas);
		writer.Write(data.characters);
		writer.Write(data.character_items);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Users.User data = new Mage.Models.Users.User();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Users.User data = (Mage.Models.Users.User)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.uuid = reader.Read<System.String>();
		data.username = reader.Read<System.String>();
		data.password = reader.Read<System.String>();
		data.facebook_id = reader.Read<System.String>();
		data.authentication_method = reader.Read<System.String>();
		data.fullname = reader.Read<System.String>();
		data.phone = reader.Read<System.String>();
		data.email = reader.Read<System.String>();
		data.avatar = reader.Read<System.String>();
		data.status = reader.Read<Mage.Models.Users.UserStatus>();
		data.notification_token = reader.Read<System.String>();
		data.country_code = reader.Read<System.String>();
		data.user_datas = reader.ReadList<Mage.Models.Users.UserData>();
		data.characters = reader.ReadList<Mage.Models.Game.Character>();
		data.character_items = reader.ReadList<Mage.Models.Game.CharacterItem>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsUsersUser():base(typeof(Mage.Models.Users.User)){}
}