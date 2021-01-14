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
    public class FBUserPropertyAttribute : Attribute
    {
        public const string target = "FBUserProperty";
        public const string fieldPrefix = "p_";
        public FBUserPropertyCategory attributeType = FBUserPropertyCategory.Others;
        public FBUserPropertyAttribute(FBUserPropertyCategory type = FBUserPropertyCategory.Others)
        {
            attributeType = type;
        }

        public static UserData ExtractUserDataFields(object obj, string className, string memberName) {
            string value = MageAttributeHelper.GetMemberValue(obj, memberName);
            string name = FieldLabel(className, memberName);

            if (MageAttributeHelper.IsArrayMember(obj, memberName)) {
                string[] val = value.Split(',');

                for (int k = 0; k < val.Length; k++) {
                    if (val[k].Trim() != "" && val[k].Trim() != "0") {
                        UserData d = new UserData() {
                            attr_name = name+"_"+k,
                            attr_value = val[k],
                            attr_type = target
                        };
                        return d;
                    }
                }

            } else {
                UserData d = new UserData() {
                    attr_name = name,
                    attr_value = value,
                    attr_type = target
                };
                return d;
            }

            return null;
        }

        public static string FieldLabel(string className, string memberName) {
            string[] words = memberName.Split(' ');
            if (words[1] != "") 
            {
                return className.Substring(0,1).ToLower() +"_" + words[1];
            }
            else {
                return "";
            }
        }
    }

    public enum FBUserPropertyCategory 
    {
        Exp,
        Currency,
        Level,
        Achievement,
        Others
    }
}