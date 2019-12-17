using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_ItemSaveData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		ItemSaveData data = (ItemSaveData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.id);
		writer.Write(data.value);
		writer.Write(data.itemType);
		writer.Write(data.position);
		writer.Write(data.rotation);
		writer.Write(data.scale);

	}
	
	public override object Read(ES2Reader reader)
	{
		ItemSaveData data = new ItemSaveData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		ItemSaveData data = (ItemSaveData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.id = reader.Read<System.Int32>();
		data.value = reader.Read<System.Single>();
		data.itemType = reader.Read<ItemSaveDataType>();
		data.position = reader.Read<UnityEngine.Vector3>();
		data.rotation = reader.Read<UnityEngine.Quaternion>();
		data.scale = reader.Read<UnityEngine.Vector3>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_ItemSaveData():base(typeof(ItemSaveData)){}
}