using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Users{
	[Serializable]
	public class UserData : BaseModel
	{
		public string id = "";
		public string user_id = "";
		public string attr_name = "";
		public string attr_value = "";
		public string attr_type = "";

		public UserData() : base () {
		}

		public UserData(string attrName, string attrValue, string attrType) : base () {
			this.attr_name = attrName;
			this.attr_value = attrValue;
			this.attr_type = attrType;
		}

		public UserData(string attrName) : base () {
			this.attr_name = attrName;
		}

		public List<UserData> ExtractFields<T>(T obj) where T: BaseModel{
			List<UserData> extractFields = new List<UserData>();

			if (obj != null) {
				// Get the type of MyClass1.
				Type myType = typeof(T);
				// Get the members associated with MyClass1.
				MemberInfo[] myMembers = myType.GetMembers();

				// Display the attributes for each of the members of MyClass1.
				for (int i = 0; i < myMembers.Length; i++)
				{
					//Debug.Log("Member: " + myMembers[i].ToString());

					object[] myAttributes = myMembers[i].GetCustomAttributes(true);
					if (myAttributes.Length > 0)
					{
						for (int j = 0; j < myAttributes.Length; j++) {
							//Debug.Log("Extracted atrributes: " + myAttributes[j].ToString());
							if (myAttributes[j].ToString().Contains("Mage.Models.ExtractFieldAttribute")) {
								//Debug.Log("Extracted member: " + myMembers[i].ToString());
								string value = ExtractFieldAttribute.GetMemberValue(obj, myMembers[i].ToString());
								string name = ExtractFieldAttribute.FieldLabel(myType.Name, myMembers[i].ToString());

								if (ExtractFieldAttribute.IsArrayMember(obj, myMembers[i].ToString())) {
									string[] val = value.Split(',');

									for (int k = 0; k < val.Length; k++) {
										if (val[k] != "" && val[k] != "0") {
											UserData d = new UserData() {
											attr_name = name+"_"+k,
											attr_value = val[k],
											attr_type = "ExtractField"
										};

										extractFields.Add(d);
										}
									}

								} else {
									UserData d = new UserData() {
										attr_name = name,
										attr_value = value,
										attr_type = "ExtractField"
									};

									extractFields.Add(d);
								}
								
								//sDebug.Log("Added extracted: " + d.ToJson());
							}
							//Debug.Log("The type of the attribute is " + myAttributes[j].ToString());
						}
					}
				}
			}
			return extractFields;
		}
	}
}
