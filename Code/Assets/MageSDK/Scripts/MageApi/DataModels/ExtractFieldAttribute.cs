using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace Mage.Models
{
    [Serializable]
    public class ExtractFieldAttribute : Attribute
    {

        public ExtractFieldAttribute()
        {
        }

        public void PrintExtractFieldInfo()
        {
            try
            {
                // Get the type of MyClass1.
                Type myType = this.GetType();
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
							if (myAttributes[j].ToString().Contains("Mage.Models.ExtractFieldAttribute")) {
								Debug.Log("Extracted member: " + myMembers[i].ToString());
							}
							//Debug.Log("The type of the attribute is " + myAttributes[j].ToString());
						}
					}
                }
            }
            catch (Exception e)
            {
                Debug.Log("An exception occurred: ");
            }
        }

        public static string GetMemberValue(object src, string memberName) 
        {
            string[] words = memberName.Split(' ');

            Type t = Type.GetType(words[0]);
            if (t.IsArray) {
                if (words[1] != "") 
                {
                    System.Object val = src.GetType().InvokeMember(words[1], BindingFlags.GetField, null, src, null);

                    IEnumerable items = val as IEnumerable;
                    string r = "";
                    foreach (object o in items)
                    {
                        r += ((o != null) ? o.ToString() : "") + ", ";
                    }

                    return r;
                } else {
                    return "";
                }
            } else {
                if (words[1] != "") 
                {

                    return src.GetType().InvokeMember(words[1], BindingFlags.GetField, null, src, null).ToString();
                }
                else 
                {
                    return "";
                }
            }
        }

        public static bool IsArrayMember(object src, string memberName) {
            string[] words = memberName.Split(' ');
            Type t = Type.GetType(words[0]);

            return t.IsArray;
        }

        public static string FieldLabel(string className, string memberName) {
            string[] words = memberName.Split(' ');
            if (words[1] != "") 
            {
                return "Field_"+className+"_" + words[1];
            }
            else {
                return "";
            }
        }
    }
}