using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace Mage.Models.Attributes
{
    [Serializable]
    public class FBUserPropertyAttribute : Attribute
    {
        
        public FBUserPropertyCategory attributeType = FBUserPropertyCategory.Others;
        public FBUserPropertyAttribute(FBUserPropertyCategory type = FBUserPropertyCategory.Others)
        {
            attributeType = type;
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