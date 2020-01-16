using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Users;

public class ES2UserType_MageModelsUsersMessage : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Users.Message data = (Mage.Models.Users.Message)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.message);
		writer.Write(data.title);
		writer.Write(data.sent_at);
		writer.Write(data.read_at);
		writer.Write(data.message_type);
		writer.Write(data.status);
		writer.Write(data.sender);
		writer.Write(data.receiver);
		writer.Write(data.action_ios);
		writer.Write(data.action_android);
		writer.Write(data.action_windows);
		writer.Write(data.action_others);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Users.Message data = new Mage.Models.Users.Message();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Users.Message data = (Mage.Models.Users.Message)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.String>();
		data.message = reader.Read<System.String>();
		data.title = reader.Read<System.String>();
		data.sent_at = reader.Read<System.DateTime>();
		data.read_at = reader.Read<System.DateTime>();
		data.message_type = reader.Read<Mage.Models.Users.MessageType>();
		data.status = reader.Read<Mage.Models.Users.MessageStatus>();
		data.sender = reader.Read<Mage.Models.Users.User>();
		data.receiver = reader.Read<Mage.Models.Users.User>();
		data.action_ios = reader.Read<System.String>();
		data.action_android = reader.Read<System.String>();
		data.action_windows = reader.Read<System.String>();
		data.action_others = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsUsersMessage():base(typeof(Mage.Models.Users.Message)){}
}