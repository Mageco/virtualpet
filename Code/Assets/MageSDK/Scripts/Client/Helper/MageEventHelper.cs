using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using SimpleJSON;
using MageApi.Models;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageApi;
using Mage.Models.Application;
using Mage.Models;

namespace MageSDK.Client.Helper
{
    public class MageEventHelper
    {
        private static MageEventHelper _instance;

        public MageEventHelper()
        {
            //load something from local
        }

        public static MageEventHelper GetInstance()
        {
            if (null == _instance)
            {
                _instance = new MageEventHelper();
            }
            return _instance;
        }

        public void LoadMageEventData()
        {
            LoadEventCounterList();
            LoadMageEventCache();
        }
        #region event counter

        private void SaveEventCounterList(List<EventCounter> data)
        {
            if (!MageEngine.instance.resetUserDataOnStart)
            {
                ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
            }

            RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, data);
        }

        private List<EventCounter> LoadEventCounterList()
        {

            if (ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE))
            {
                List<EventCounter> t = ES2.LoadList<EventCounter>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
                if (t == null)
                {
                    t = InitDefaultEventCounter();
                }
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
                return t;

            }
            else
            {
                List<EventCounter> t = InitDefaultEventCounter();
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
                return t;
            }
        }

        private List<EventCounter> InitDefaultEventCounter()
        {
            List<EventCounter> tmp = new List<EventCounter>();
            foreach (MageEventType t in Enum.GetValues(typeof(MageEventType)))
            {
                tmp.Add(new EventCounter(t.ToString(), 0));
            }

            return tmp;
        }


        private List<EventCounter> GetEventCounterList()
        {
            List<EventCounter> eventCounterList = RuntimeParameters.GetInstance().GetParam<List<EventCounter>>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
            if (null == eventCounterList)
            {
                eventCounterList = new List<EventCounter>();
                SaveEventCounterList(eventCounterList);
            }

            return eventCounterList;
        }

        private void AddEventCounter(string eventName)
        {
            List<EventCounter> eventCounterList = GetEventCounterList();

            bool found = false;
            //search for eventName
            for (int i = 0; i < eventCounterList.Count; i++)
            {
                if (eventCounterList[i].Key == eventName)
                {
                    found = true;
                    eventCounterList[i].Value++;
                    var newKvp = new EventCounter(eventCounterList[i].Key, eventCounterList[i].Value);
                    eventCounterList.RemoveAt(i);
                    eventCounterList.Insert(i, newKvp);
                }
            }

            if (!found)
            {
                var newKvp = new EventCounter(eventName, 1);
                eventCounterList.Insert(eventCounterList.Count, newKvp);
            }

            SaveEventCounterList(eventCounterList);
        }

        public int GetEventCounter(string eventName)
        {

            List<EventCounter> eventCounterList = GetEventCounterList();

            //search for eventName
            for (int i = 0; i < eventCounterList.Count; i++)
            {
                if (eventCounterList[i].Key == eventName)
                {
                    return eventCounterList[i].Value;
                }
            }

            return 0;
        }

        public string ConvertEventCounterListToJson()
        {
            List<EventCounter> eventCounterList = GetEventCounterList();

            if (null != eventCounterList)
            {
                string output = "{";
                for (int i = 0; i < eventCounterList.Count; i++)
                {
                    output += "\"" + MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE_PREFIX + eventCounterList[i].Key + "\": " + eventCounterList[i].Value + ", ";
                }

                if (eventCounterList.Count > 0)
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

        private List<MageEvent> LoadMageEventCache()
        {

            if (ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_CACHE))
            {
                List<MageEvent> t = ES2.LoadList<MageEvent>(MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
                if (t == null)
                {
                    t = new List<MageEvent>();
                }
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, t);
                return t;

            }
            else
            {
                List<MageEvent> t = new List<MageEvent>();
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, t);
                return t;
            }
        }


        public List<MageEvent> GetMageEventsList()
        {
            return RuntimeParameters.GetInstance().GetParam<List<MageEvent>>(MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
        }

        public void SaveMageEventsList(List<MageEvent> data)
        {

            if (!MageEngine.instance.resetUserDataOnStart)
            {
                ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
            }

            RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, data);
        }

        public void OnEvent(MageEventType type, Action callbackSendEventApi, string eventDetail = "", string eventValue = "")
        {
            AddEventToCache(type, eventDetail, eventValue);
            AddEventCounter(type.ToString());
            callbackSendEventApi();
        }

        public void OnEvent(string type, Action callbackSendEventApi, string eventDetail = "", string eventValue = "")
        {
            AddEventToCache(type, eventDetail, eventValue);
            AddEventCounter(type.ToString());
            callbackSendEventApi();
        }

        public void OnEvent<T>(MageEventType type, T obj, Action callbackSendEventApi) where T : BaseModel
        {
            AddEventToCache(type);
            AddEventCounter(type.ToString());
            //callbackSendEventApi(new MageEvent(type, obj.ToJson()));
            callbackSendEventApi();
        }

        private void AddEventToCache(MageEventType type, string eventDetail = "", string eventValue = "")
        {
            List<MageEvent> cachedEvent = GetMageEventsList();
            cachedEvent.Add(new MageEvent(type, eventDetail, eventValue));
            SaveMageEventsList(cachedEvent);
        }

        private void AddEventToCache(string type, string eventDetail = "", string eventValue = "")
        {
            List<MageEvent> cachedEvent = GetMageEventsList();
            cachedEvent.Add(new MageEvent(type, eventDetail, eventValue));
            SaveMageEventsList(cachedEvent);
        }

        #endregion
    }

}

