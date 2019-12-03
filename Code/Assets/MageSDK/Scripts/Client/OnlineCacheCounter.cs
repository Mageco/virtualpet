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

namespace MageSDK.Client {
	public class OnlineCacheCounter {
        private int counter = 0;
        private int max = 0;

        public OnlineCacheCounter(int counter = 0, int max = 0) {
            this.counter = counter;
            this.max = max;
        }

        public bool IsMax () {
            if (counter == max) {
                counter = 0;
                return true;
            } else {
                counter ++;
                return false;
            }
        }
	}

}

