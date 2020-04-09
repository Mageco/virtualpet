using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Users;

public class ES2UserType_MageModelsUsersLeaderBoardItem : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Mage.Models.Users.LeaderBoardItem data = (Mage.Models.Users.LeaderBoardItem)obj;
		// Add your writer.Write calls here.
		writer.Write(data.rank);
		writer.Write(data.board_name);
		writer.Write(data.score);
		writer.Write(data.user_id);
		writer.Write(data.fullname);
		writer.Write(data.avatar);

	}
	
	public override object Read(ES2Reader reader)
	{
		Mage.Models.Users.LeaderBoardItem data = new Mage.Models.Users.LeaderBoardItem();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		Mage.Models.Users.LeaderBoardItem data = (Mage.Models.Users.LeaderBoardItem)c;
		// Add your reader.Read calls here to read the data into the object.
		data.rank = reader.Read<System.Int32>();
		data.board_name = reader.Read<System.String>();
		data.score = reader.Read<System.Int32>();
		data.user_id = reader.Read<System.String>();
		data.fullname = reader.Read<System.String>();
		data.avatar = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_MageModelsUsersLeaderBoardItem():base(typeof(Mage.Models.Users.LeaderBoardItem)){}
}