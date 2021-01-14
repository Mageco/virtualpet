using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Attributes;
using System.Reflection;
using System.ComponentModel;
using Mage.Models.Users;

namespace Mage.Models
{
    [Serializable]
    public class BaseModel
    {

        private Hashtable _valueTracker = new Hashtable();
        private bool _valueTrackerLoaded = false;
        public BaseModel()
        {
            _valueTracker = new Hashtable();
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static TResult CreateFromJSON<TResult>(string jsonString) where TResult : BaseModel
        {
            try
            {
                return JsonUtility.FromJson<TResult>(jsonString);
            }
            catch (Exception e)
            {
                return default(TResult);
            }

        }

        public static T Clone<T>(T obj) where T : BaseModel
        {
            try
            {
                return JsonUtility.FromJson<T>(obj.ToJson());
            }
            catch (Exception e)
            {
                return default(T);
            }

        }

        public string ToEncryptedJson(string key)
        {
            return ApiUtils.GetInstance().EncryptStringWithKey(JsonUtility.ToJson(this), key);
        }

        public static TResult CreatFromEncryptJson<TResult>(string encryptedString, string key) where TResult : BaseModel
        {
            try
            {
                string jsonString = ApiUtils.GetInstance().DecryptStringWithKey(encryptedString, key);
                return JsonUtility.FromJson<TResult>(jsonString);
            }
            catch (Exception e)
            {
                return default(TResult);
            }
        }

        #region OldValueTracking
        public T GetMemberOldValue<T>(string key)
        {

            if (_valueTracker.Contains(key))
            {
                return (T)_valueTracker[key];
            }
            else
            {
                return default(T);
            }
        }


        public void SetMemberOldValue(string key, object obj)
        {
            if (_valueTracker.Contains(key))
            {
                _valueTracker[key] = obj;
            }
            else
            {
                _valueTracker.Add(key, obj);
            }
        }

        public void LoadValueTracker<T>(T obj) where T : BaseModel
        {
            //if (!_valueTrackerLoaded) {
            _valueTracker = MageAttributeHelper.CopyMetaFields<T>(obj);
            //}
        }

        public string PrintValueTracker()
        {
            string result = "";
            foreach (DictionaryEntry info in _valueTracker)
            {
                result += (info.Key.ToString() + ": " + info.Value.ToString()) + "\r\n";
            }

            return result;
        }

        public static Dictionary<string, object> ToDictionary(object obj)
        {
            Dictionary<string, object> extractFields = new Dictionary<string, object>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = obj.GetType();

                FieldInfo[] myFields = myType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myFields.Length; i++)
                {
                    if (!myFields[i].IsNotSerialized)
                    {
                        object child = obj.GetType().InvokeMember(myFields[i].Name, BindingFlags.GetField, null, obj, null);
                        FieldInfo[] childFields = child.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        if (childFields.Length > 0)
                        {
                            extractFields.Add(myFields[i].Name, BaseModel.ToDictionary(child));
                        }
                        else
                        {
                            if (IsGenericList(child))
                            {
                                Type[] genericTypes = child.GetType().GetGenericArguments();
                                if (genericTypes.Length == 1)
                                {
                                    
                                    
                                    if (typeof(BaseModel).IsAssignableFrom(genericTypes[0]))
                                    {
                                        Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                        IList res = (IList)Activator.CreateInstance(t, new object[] { child });

                                        List<Dictionary<string, object>> tmpDictionaryList = new List<Dictionary<string, object>>();
                                        foreach (var item in res)
                                        {
                                            Dictionary<string, object> tmpElement = BaseModel.ToDictionary(item);
                                            tmpDictionaryList.Add(tmpElement);
                                        }
                                        extractFields.Add(myFields[i].Name, tmpDictionaryList);
                                    }
                                    else
                                    {
                                        Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                        IList res = (IList)Activator.CreateInstance(t, new object[] { child });
                                        Dictionary<string, object> tmpDictionaryList = new Dictionary<string, object>();
                                        int index = 0;
                                        foreach (var item in res)
                                        {
                                            tmpDictionaryList.Add(index.ToString(), item);
                                            index ++;
                                        }
                                        extractFields.Add(myFields[i].Name, tmpDictionaryList);
                                    }
                                }
                            }
                            else
                            {
                                extractFields.Add(myFields[i].Name, child);
                            }
                        }
                    }
                }
            }
            return extractFields;
        }

        public static T FromDictionary<T>(Dictionary<string, object> input) where T : BaseModel
        {
            T tmp = (T)Activator.CreateInstance(typeof(T));

            foreach (KeyValuePair<string, object> pair in input)
            {
                try
                {
                    FieldInfo field = typeof(T).GetField(pair.Key);

                    if (field != null)
                    {
                        if ((field.FieldType.IsGenericType && (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))))
                        {
                            
                            Type[] genericTypes = field.FieldType.GetGenericArguments();
                            if (genericTypes.Length == 1)
                            {
                                if (genericTypes[0].IsAssignableFrom(typeof(BaseModel)))
                                {
                                    Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                    IList res = (IList)Activator.CreateInstance(t);

                                    MethodInfo method = typeof(BaseModel).GetMethod("FromDictionary");
                                    MethodInfo generic = method.MakeGenericMethod(genericTypes[0]);
                                    //scan through all item in list
                                    foreach (Dictionary<string, object> item in (List<Dictionary<string, object>>)pair.Value)
                                    {
                                        res.Add(generic.Invoke(null, new object[] { item }));
                                    }
                                    field.SetValue(tmp, res);
                                }
                                else
                                {
                                    Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                    IList res = (IList)Activator.CreateInstance(t);

                                    foreach (KeyValuePair<string, object> item in (Dictionary<string, object>)pair.Value)
                                    {
                                        res.Add(item.Value);
                                    }

                                    field.SetValue(tmp, res);
                                }
                            }
                        }
                        else
                        {
                            if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                            {
                                MethodInfo method = typeof(BaseModel).GetMethod("FromDictionary");
                                MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                                field.SetValue(tmp, generic.Invoke(null, new object[] { pair.Value }));
                            } else if (pair.Value.GetType().IsArray) 
                            {
                                field.SetValue(tmp, pair.Value);
                            }
                            else
                            {
                                switch (field.FieldType.ToString())
                                {
                                    case "System.Int32":
                                        field.SetValue(tmp, (int)Convert.ChangeType(pair.Value, typeof(int)));
                                        break;
                                    case "System.Int64":
                                        field.SetValue(tmp, (long)Convert.ChangeType(pair.Value, typeof(long)));
                                        break;
                                    case "System.Single":
                                        field.SetValue(tmp, (float)Convert.ChangeType(pair.Value, typeof(float)));
                                        break;
                                    case "System.Double":
                                        double.TryParse(pair.Value.ToString(), out double x);
                                        field.SetValue(tmp, (double)Convert.ChangeType(pair.Value, typeof(double)));
                                        break;
                                    case "System.DateTime":
                                        field.SetValue(tmp, (DateTime)Convert.ChangeType(pair.Value, typeof(System.DateTime)));
                                        break;
                                    default:
                                        field.SetValue(tmp, pair.Value.ToString());
                                        break;
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    //Debug.Log(e.StackTrace);
                }
            }
            return tmp;
        }

        public static bool IsGenericList(object o)
        {
            var oType = o.GetType();
            return (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)));
        }
        public static object GetMemberValueObject(object src, string memberName)
        {
            string[] words = memberName.Split(' ');
            return src.GetType().InvokeMember(words[1], BindingFlags.GetField, null, src, null);
        }

        public void SaveAttribute(string attributeName, string value)
        {
            FieldInfo field = this.GetType().GetField(attributeName);

            if (field != null)
            {
                field.SetValue(this, value);
            }
        }

        #endregion

    }
}
