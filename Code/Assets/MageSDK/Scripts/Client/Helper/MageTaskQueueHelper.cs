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
using Mage.Models.Users;
#if USE_UNITY_ADMOB && !UNITY_STANDALONE
using MageSDK.Client.Adaptors;
#endif

namespace MageSDK.Client.Helper
{
    public class MageTaskQueueHelper
    {
        private static MageTaskQueueHelper _instance;

        private Queue<MageTaskQueueItem> _taskQueue = new Queue<MageTaskQueueItem>();

        public MageTaskQueueHelper()
        {
            //load something from local
        }

        public static MageTaskQueueHelper GetInstance()
        {
            if (null == _instance)
            {
                _instance = new MageTaskQueueHelper();
            }
            return _instance;
        }

        #region functions
        public MageTaskQueueItem Dequeue()
        {
            if (_taskQueue.Count > 0)
            {
                return _taskQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public void Enqueue(MageTaskQueueItem item)
        {
            this._taskQueue.Enqueue(item);
        }

        public int GetQueueSize()
        {
            return this._taskQueue.Count;
        }

        public IEnumerator ProcessNextQueueTask()
        {
            MageTaskQueueItem currentTask = Dequeue();
            yield return null;
            if (currentTask != null && currentTask.action != null)
            {
                currentTask.action.GetType().InvokeMember("Invoke", System.Reflection.BindingFlags.InvokeMethod, null, currentTask.action, currentTask.parameters);
            }
        }
        #endregion functions
    }

    public class MageTaskQueueItem
    {
        public object action;
        public object[] parameters;
    }

}

