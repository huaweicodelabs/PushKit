/*
       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

       Licensed under the Apache License, Version 2.0 (the "License");
       you may not use this file except in compliance with the License.
       You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing, software
       distributed under the License is distributed on an "AS IS" BASIS,
       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
       See the License for the specific language governing permissions and
       limitations under the License.
*/

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace XamarinHmsPushDemo.Hmssample
{
    [Activity(Name = "com.huawei.xamarinhmspushdemo.Deeplink2Activity", Label = "Deeplink2Activity")]
    public class Deeplink2Activity : Activity
    {
        private static readonly string TAG = "Deep Link 2 Activity";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_deeplink2);
            GetIntentData(Intent);
        }

        private void GetIntentData(Intent intent)
        {
            if (null != intent)
            {
                string msgid = intent.GetStringExtra("_push_msgid");
                string cmdType = intent.GetStringExtra("_push_cmd_type");
                int notifyId = intent.GetIntExtra("_push_notifyid", -1);
                Bundle bundle = intent.Extras;
                if (bundle != null)
                {
                    foreach (string key in bundle.KeySet())
                    {
                        string content = bundle.GetString(key);
                        Log.Info(TAG, "receive data from push, key = " + key + ", content = " + content);
                    }
                }
                Log.Info(TAG, "receive data from push, msgId = " + msgid + ", cmd = " + cmdType + ", notifyId = " + notifyId);
            }
            else
            {
                Log.Info(TAG, "intent = null");
            }
        }


        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            GetIntentData(intent);
        }
    }
}