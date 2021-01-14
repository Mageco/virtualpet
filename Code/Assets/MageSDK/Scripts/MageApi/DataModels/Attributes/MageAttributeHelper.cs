using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
using MageApi;

namespace Mage.Models.Attributes
{
    [Serializable]
    public class MageAttributeHelper
    {
        ///<summary> Helper function to print field info </summary>///
        public static void PrintExtractFieldInfo(object obj)
        {
            try
            {
                // Get the type of MyClass1.
                Type myType = obj.GetType();
                // Get the members associated with MyClass1.
                MemberInfo[] myMembers = myType.GetMembers();

                // Display the attributes for each of the members of MyClass1.
                for (int i = 0; i < myMembers.Length; i++)
                {
                    //Debug.Log("Member: " + myMembers[i].ToString());

                    object[] myAttributes = myMembers[i].GetCustomAttributes(true);
                    if (myAttributes.Length > 0)
                    {

                        for (int j = 0; j < myAttributes.Length; j++)
                        {
                            //Debug.Log("Attribute: " + myAttributes[j].ToString());
                            if (myAttributes[j].ToString().Contains("Mage.Models.Attributes.FBUserPropertyAttribute"))
                            {

                                Debug.Log("Extracted member: " + myMembers[i].ToString());
                                FBUserPropertyAttribute attribute = (FBUserPropertyAttribute)myAttributes[j];
                                Debug.Log("Attribute type: " + attribute.attributeType.ToString());

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

        ///<summary> get value of member, if it is string then convert to a string </summary>///
        public static string GetMemberValue(object src, string memberName)
        {
            string[] words = memberName.Split(' ');

            Type t = Type.GetType(words[0]);
            if (t.IsArray)
            {
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
                }
                else
                {
                    return "";
                }
            }
            else
            {
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

        public static object GetMemberValueObject(object src, string memberName)
        {
            string[] words = memberName.Split(' ');
            return src.GetType().InvokeMember(words[1], BindingFlags.GetField, null, src, null);
        }

        public static System.Object GetMemberObject(object src, string memberName)
        {
            string[] words = memberName.Split(' ');

            if (words[1] != "")
            {
                System.Object val = src.GetType().InvokeMember(words[1], BindingFlags.GetField, null, src, null);
                return val;
            }
            else
            {
                return null;
            }
        }

        public static bool IsArrayMember(object src, string memberName)
        {
            string[] words = memberName.Split(' ');
            Type t = Type.GetType(words[0]);

            return t.IsArray;
        }

        public static string FieldLabelMage(string className, string memberName)
        {
            string[] words = memberName.Split(' ');
            if (words[1] != "")
            {
                return "Field_" + className + "_" + words[1];
            }
            else
            {
                return "";
            }
        }

        public static string FieldLabelFirebase(string className, string memberName)
        {
            string[] words = memberName.Split(' ');
            if (words[1] != "")
            {
                return className.Substring(0, 1).ToLower() + "_" + words[1];
            }
            else
            {
                return "";
            }
        }

        ///<summary> Extract fields and return a list of User Data of extract fields </summary>///
        public static List<UserData> ExtractUserDataFields<T>(T obj) where T : BaseModel
        {
            List<UserData> extractFields = new List<UserData>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = typeof(T);
                // Get the members associated with obj.
                MemberInfo[] myMembers = myType.GetMembers();

                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myMembers.Length; i++)
                {
                    object[] myAttributes = myMembers[i].GetCustomAttributes(true);
                    if (myAttributes.Length > 0)
                    {
                        for (int j = 0; j < myAttributes.Length; j++)
                        {
                            Type attributeType = Type.GetType(myAttributes[j].ToString());
                            MethodInfo handleAttribute = attributeType.GetMethod("HandleAttribute");
                            if (handleAttribute != null)
                            {
                                UserData tmp = (UserData)handleAttribute.Invoke(obj, new object[] { obj, myType.Name, myMembers[i].ToString() });
                                if (tmp != null)
                                {
                                    extractFields.Add(tmp);
                                }
                            }
                        }
                    }
                }
            }
            return extractFields;
        }

        public static List<UserData> ExtractUserDataFields<T>(T obj, Type typeFilter) where T : BaseModel
        {
            List<UserData> extractFields = new List<UserData>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = typeof(T);
                // Get the members associated with obj.
                MemberInfo[] myMembers = myType.GetMembers();

                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myMembers.Length; i++)
                {
                    object[] myAttributes = myMembers[i].GetCustomAttributes(true);
                    if (myAttributes.Length > 0)
                    {
                        for (int j = 0; j < myAttributes.Length; j++)
                        {
                            Type attributeType = Type.GetType(myAttributes[j].ToString());
                            if (attributeType == typeFilter)
                            {
                                MethodInfo handleAttribute = attributeType.GetMethod("HandleAttribute");
                                if (handleAttribute != null)
                                {
                                    UserData tmp = (UserData)handleAttribute.Invoke(obj, new object[] { obj, myType.Name, myMembers[i].ToString() });
                                    if (tmp != null)
                                    {
                                        extractFields.Add(tmp);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return extractFields;
        }

        public static Dictionary<string, object> ExtractRawFields<T>(T obj, Type attribute) where T : BaseModel
        {
            Dictionary<string, object> extractFields = new Dictionary<string, object>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = typeof(T);
                // Get the members associated with obj.
                MemberInfo[] myMembers = myType.GetMembers();

                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myMembers.Length; i++)
                {
                    object[] myAttributes = myMembers[i].GetCustomAttributes(true);
                    if (myAttributes.Length > 0)
                    {
                        for (int j = 0; j < myAttributes.Length; j++)
                        {
                            Type attributeType = Type.GetType(myAttributes[j].ToString());
                            if (attributeType == attribute)
                            {
                                extractFields.Add(myMembers[i].Name, GetMemberValueObject(obj, myMembers[i].ToString()));
                            }

                        }
                    }
                }
            }
            return extractFields;
        }


        public static Hashtable CopyMetaFields<T>(T obj) where T : BaseModel
        {
            Hashtable result = new Hashtable();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = typeof(T);
                // Get the members associated with obj.
                MemberInfo[] myMembers = myType.GetMembers();

                // Scan through the attributes for each of the members of obj.
                for (int i = 0; i < myMembers.Length; i++)
                {
                    object[] myAttributes = myMembers[i].GetCustomAttributes(true);
                    if (myAttributes.Length > 0)
                    {
                        for (int j = 0; j < myAttributes.Length; j++)
                        {
                            if (myAttributes[j].ToString().Contains(typeof(ExtractFieldAttribute).FullName) || myAttributes[j].ToString().Contains(typeof(FBUserPropertyAttribute).FullName))
                            {
                                result.Add(myMembers[i].ToString(), GetMemberObject(obj, myMembers[i].ToString()));
                            }

                        }
                    }
                }
            }
            return result;
        }

        public static string ConvertListToJson(List<UserData> list)
        {
            if (null != list)
            {
                string output = "{";
                for (int i = 0; i < list.Count; i++)
                {
                    output += "\"" + list[i].attr_name + "\": " + JsonValue(list[i].attr_value) + ", ";
                }

                if (list.Count > 0)
                {
                    output = output.Substring(0, output.Length - 2);
                }

                output += "}";
                return output;
            }
            else
            {
                return "";
            }
        }

        private static string JsonValue(string x)
        {
            int testInt = 0;
            bool isInt = int.TryParse(x, out testInt);
            double testDouble = 0;
            bool isDouble = Double.TryParse(x, out testDouble);

            string padding = isInt && isDouble ? "" : "\"";
            return padding + x + padding;
        }
    }
}