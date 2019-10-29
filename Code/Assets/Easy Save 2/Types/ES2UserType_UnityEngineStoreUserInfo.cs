using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Store;

public class ES2UserType_UnityEngineStoreUserInfo : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		UnityEngine.Store.UserInfo data = (UnityEngine.Store.UserInfo)obj;
		// Add your writer.Write calls here.
		writer.Write(data.channel);
		writer.Write(data.userId);
		writer.Write(data.userLoginToken);

	}
	
	public override object Read(ES2Reader reader)
	{
		UnityEngine.Store.UserInfo data = new UnityEngine.Store.UserInfo();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		UnityEngine.Store.UserInfo data = (UnityEngine.Store.UserInfo)c;
		// Add your reader.Read calls here to read the data into the object.
		data.channel = reader.Read<System.String>();
		data.userId = reader.Read<System.String>();
		data.userLoginToken = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_UnityEngineStoreUserInfo():base(typeof(UnityEngine.Store.UserInfo)){}
}