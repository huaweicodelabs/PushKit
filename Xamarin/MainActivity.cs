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
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Huawei.Agconnect.Config;
using Com.Huawei.Hmf.Tasks;
using Com.Huawei.Hms.Aaid;
using Com.Huawei.Hms.Aaid.Entity;
using Com.Huawei.Hms.Common;
using Com.Huawei.Hms.Push;
using System;
using XamarinHmsPushDemo.Hmssample;
using Uri = Android.Net.Uri;

namespace XamarinHmsPushDemo
{

    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
        private static readonly string TAG = "Main Activity";

        private TextView tvSetPush;

        private TextView tvSetAAID;

        private TextView tvSetAutoInit;

        private static readonly int GET_AAID = 1;

        private static readonly int DELETE_AAID = 2;

        private static readonly string CODELABS_ACTION = "com.huawei.xamarinhmspushdemo.action";

        private MyReceiver receiver;

        private Handler handler;


        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            tvSetPush = FindViewById<TextView>(Resource.Id.btn_set_push);
            tvSetAAID = FindViewById<TextView>(Resource.Id.btn_get_aaid);
            tvSetAutoInit = FindViewById<TextView>(Resource.Id.btn_set_autoInit_enabled);

            tvSetPush.SetOnClickListener(this);
            tvSetAAID.SetOnClickListener(this);
            tvSetAutoInit.SetOnClickListener(this);

            FindViewById(Resource.Id.btn_add_topic).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_get_token).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_delete_token).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_delete_topic).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_action).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_generate_intent).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_is_autoInit_enabled).SetOnClickListener(this);
            receiver = new MyReceiver(this);
            IntentFilter filter = new IntentFilter();
            filter.AddAction(CODELABS_ACTION);
            RegisterReceiver(receiver, filter);

            handler = new mHandler(GET_AAID, DELETE_AAID, tvSetAAID, Resources.GetString(Resource.String.get_aaid), Resources.GetString(Resource.String.delete_aaid));

        }

        public void ShowLog(string log)
        {
            RunOnUiThread(() =>
            {
                View tvView = FindViewById(Resource.Id.tv_log);
                View svView = FindViewById(Resource.Id.sv_log);
                if (tvView is TextView)
                {
                    ((TextView)tvView).Text = log;
                }
                if (svView is ScrollView)
                {
                    ((ScrollView)svView).FullScroll(FocusSearchDirection.Down);
                }
            });
        }

        /**
        * GetAAID(), This method is used to obtain an AAID in asynchronous mode. You need to add a listener to listen to the operation result.
        * DeleteAAID(), delete a local AAID and its generation timestamp.
        * @param isGet getAAID or deleteAAID
        */

        private void SetAAID(bool isGet)
        {
            /**
             * isGet seperates getAAID or DeleteAAID
             */
           if (isGet)
           {
               // Get AAID
               Log.Info(TAG, "Starting AAID");
               Task idResult = HmsInstanceId.GetInstance(this).AAID;
               idResult.AddOnSuccessListener(new OnSuccessListenerImp(handler, DELETE_AAID, this)).AddOnFailureListener(new OnFailureListenerImp(this));
           }
           else
           {
                // DELETE AAID
                System.Threading.Thread thread = new System.Threading.Thread(() =>
               {
                   try
                   {
                       HmsInstanceId.GetInstance(this).DeleteAAID();
                       Log.Info(TAG, "AAID deleted");
                       ShowLog("delete aaid and its generation timestamp success.");
                       handler.SendEmptyMessage(GET_AAID);
                   }
                   catch (Exception e)
                   {
                       Log.Error(TAG, "deleteAAID failed" + e.ToString());
                       ShowLog("delete failed.");
                   }

               });

               Log.Info(TAG, "start the thread");
               thread.Start();
           }
       }


       /*
        * GetToken(String appId, String scope), This method is used to obtain a token required for accessing HUAWEI Push Kit.
        * If there is no local AAID, this method will automatically generate an AAID when it is called because the Huawei Push server needs to generate a token based on the AAID.
        * This method is a synchronous method, and you cannot call it in the main thread. Otherwise, the main thread may be blocked.
       */
        private void GetToken()
        {
            ShowLog("getToken:begin");

            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {

                try
                {
                    string appid = AGConnectServicesConfig.FromContext(this).GetString("client/app_id");
                    string token = HmsInstanceId.GetInstance(this).GetToken(appid, "HCM");
                    Log.Info(TAG, "token = " + token);

                    if (!TextUtils.IsEmpty(token))
                    {
                        SendRegTokenToServer(token);
                    }

                    ShowLog("get token:" + token);
                }
                catch (Exception e)
                {
                    Log.Info(TAG, e.ToString());
                    ShowLog("get token failed, " + e);
                }
            }

               );
            Log.Info(TAG, "start the thread");
            thread.Start();
        }

        /**
         * void DeleteToken(String appId, String scope) throws ApiException
         * This method is used to obtain a token. After a token is deleted, the corresponding AAID will not be deleted.
         * This method is a synchronous method. Do not call it in the main thread. Otherwise, the main thread may be blocked.
         */
        private void DeleteToken()
        {
            ShowLog("deleteToken:begin");
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                {
                    string appid = AGConnectServicesConfig.FromContext(this).GetString("client/102126689");
                    HmsInstanceId.GetInstance(this).DeleteToken(appid, "HCM");
                    Log.Info(TAG, "deleteToken success.");
                    ShowLog("deleteToken success");
                }
                catch (ApiException e)
                {
                    Log.Error(TAG, "deleteToken failed." + e);
                    ShowLog("deleteToken failed." + e);
                }
            });
            Log.Info(TAG, "start the thread");
            thread.Start();
        }

        /**
        * Set up enable or disable the display of notification messages.
        * @param enable enabled or not
        */

        private void SetReceiveNotifyMsg(bool enable)
        {
            ShowLog("Control the display of notification messages:begin");
            if (enable)
            {
                HmsMessaging.GetInstance(this).TurnOnPush().AddOnCompleteListener(new OnCompleteListenerImp(tvSetPush, this, Resources.GetString(Resource.String.set_push_unable)));
            }
            else
            {
                HmsMessaging.GetInstance(this).TurnOffPush().AddOnCompleteListener(new OnCompleteListenerFailImp(tvSetPush, this, Resources.GetString(Resource.String.set_push_enable)));
            }
        }

        /**
     * In Opening a Specified Page of an App, how to Generate Intent parameters.
     */
        [Obsolete]
        private void GenerateIntentUri()
        {
            Intent intent = new Intent(Intent.ActionView);

            // You can add parameters in either of the following ways:
            // Define a scheme protocol, for example, pushscheme://com.huawei.codelabpush/deeplink?.
            // way 1 start: Use ampersands (&) to separate key-value pairs. The following is an example:
            intent.SetData(Uri.Parse("pushscheme://com.huawei.xamarinhmspushdemo/deeplink?name=abc&age=180"));


            // way 1 end. In this example, name=abc and age=180 are two key-value pairs separated by an ampersand (&).

            // way 2 start: Directly add parameters to the Intent.
            // intent.SetData(Uri.Parse("pushscheme://com.huawei.codelabpush/deeplink?"));
            // intent.PutExtra("name", "abc");
            // intent.PutExtra("age", 180);
            // way 2 end.

            // The following flag is mandatory. If it is not added, duplicate messages may be displayed.
            intent.AddFlags(ActivityFlags.ClearTop);
            string intentUri = intent.ToUri(Intent.UriIntentScheme);



            // The value of intentUri will be assigned to the intent parameter in the message to be sent.
            Log.Info("intentUri", intentUri);
            ShowLog(intentUri);

            // You can start the deep link activity with the following code.
            intent.SetClass(this, typeof(DeeplinkActivity));
            StartActivity(intent);
        }

        /**
        * Simulate pulling up the application custom page by action.
        */
        private void OpenActivityByAction()
        {
            Intent intent = new Intent("com.huawei.xamarinhmspushtest.intent.action.test");

            // You can start the deep link activity with the following code.
            // intent.SetClass(this, typeof(Deeplink2Activity));
            StartActivity(intent);
        }

        private void IsAutoInitEnabled()
        {
            Log.Info(TAG, "isAutoInitEnabled:" + HmsMessaging.GetInstance(this).AutoInitEnabled);
            ShowLog("isAutoInitEnabled:" + HmsMessaging.GetInstance(this).AutoInitEnabled);
        }

        private void SetAutoInitEnabled(bool enable)
        {
            if (enable)
            {
                HmsMessaging.GetInstance(this).AutoInitEnabled = true;
                Log.Info(TAG, "setAutoInitEnabled: true");
                ShowLog("setAutoInitEnabled: true");
                tvSetAutoInit.Text = Resources.GetString(Resource.String.AutoInitDisabled);
            }
            else
            {
                HmsMessaging.GetInstance(this).AutoInitEnabled = false;
                Log.Info(TAG, "setAutoInitEnabled: false");
                ShowLog("setAutoInitEnabled: false");
                tvSetAutoInit.Text = Resources.GetString(Resource.String.AutoInitEnabled);
            }
        }

        /**
         * to subscribe to topics in asynchronous mode.
         */
        private void AddTopic()
        {
            TopicDialog topicDialog = new TopicDialog(this, true);

            topicDialog.SetOnDialogClickListener(new OnDialogClickListenerImp(topicDialog, 1, this));
            topicDialog.Show();
        }

        /**
         * to unsubscribe to topics in asynchronous mode.
         */
        private void DeleteTopic()
        {
            TopicDialog topicDialog = new TopicDialog(this, false);
            topicDialog.SetOnDialogClickListener(new OnDialogClickListenerImp(topicDialog, 0, this));
            topicDialog.Show();
        }

        private void SendRegTokenToServer(String token)
        {
            Log.Info(TAG, "sending token to server. token:" + token);
        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [Obsolete]
        public void OnClick(View view)
        {
            int viewId = view.Id;
            switch (viewId)
            {
                case Resource.Id.btn_get_aaid:
                    SetAAID(tvSetAAID.Text.ToString().Equals(GetString(Resource.String.get_aaid)));
                    break;
                case Resource.Id.btn_get_token:
                    GetToken();
                    break;
                case Resource.Id.btn_delete_token:
                    DeleteToken();
                    break;
                case Resource.Id.btn_set_push:
                    SetReceiveNotifyMsg(tvSetPush.Text.ToString().Equals(GetString(Resource.String.set_push_enable)));
                    break;
                case Resource.Id.btn_add_topic:
                    AddTopic();
                    break;
                case Resource.Id.btn_delete_topic:
                    DeleteTopic();
                    break;
                case Resource.Id.btn_action:
                    OpenActivityByAction();
                    break;
                case Resource.Id.btn_generate_intent:
                    GenerateIntentUri();
                    break;
                case Resource.Id.btn_is_autoInit_enabled:
                    IsAutoInitEnabled();
                    break;
                case Resource.Id.btn_set_autoInit_enabled:
                    SetAutoInitEnabled(tvSetAutoInit.Text.ToString().Equals(GetString(Resource.String.AutoInitEnabled)));
                    break;
                default:
                    break;
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(receiver);
        }

    }

    class OnCompleteListenerImp : Java.Lang.Object, IOnCompleteListener
    {
        private static readonly string TAG = "MainActivity: OnCompleteListenerImp";
        private readonly TextView mTvSetPush;
        private readonly MainActivity mMainActivity;
        private readonly string msg;

        public OnCompleteListenerImp(TextView TvSetPush, MainActivity mainActivity, string msg)
        {
            mTvSetPush = TvSetPush;
            mMainActivity = mainActivity;
            this.msg = msg;
        }


        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Log.Info(TAG, "turnOnPush Complete");
                mMainActivity.ShowLog("turnOnPush Complete");
                mTvSetPush.Text = msg;
            }
            else
            {
                Log.Error(TAG, "turnOnPush Failed");
                mMainActivity.ShowLog("turnOnPush failed: cause=" + task.Exception.Message);
            }
        }

    }

    class OnCompleteListenerFailImp : Java.Lang.Object, IOnCompleteListener
    {
        private static readonly string TAG = "MainActivity: OnCompleteListenerFailImp";
        private readonly TextView tvSetPush;
        private readonly MainActivity mainActivity;
        private readonly string msg;

        public OnCompleteListenerFailImp(TextView TvSetPush, MainActivity mainActivity, string msg)
        {
            this.mainActivity = mainActivity;
            this.tvSetPush = TvSetPush;
            this.msg = msg;
        }


        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Log.Info(TAG, "turnOffPush Complete");
                mainActivity.ShowLog("turnOffPush Complete");
                tvSetPush.Text = msg;
            }
            else
            {
                Log.Info(TAG, "turnOffPush failed: cause=" + task.Exception.Message);
                mainActivity.ShowLog("turnOffPush failed: cause=" + task.Exception.Message);
            }
        }

    }

    public class MyReceiver : BroadcastReceiver
    {
        private readonly MainActivity mainActivity;
        public MyReceiver(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.Extras;
            if (bundle != null && bundle.GetString("msg") != null)
            {
                string content = bundle.GetString("msg");
                mainActivity.ShowLog(content);
            }
        }


    }

    class OnCompleteListenerMessagingImp : Java.Lang.Object, IOnCompleteListener
    {
        private static readonly string TAG = "MainActivity: OnCompleteListenerMessagingImp";
        private readonly int type;
        private readonly MainActivity mainActivity;
        public OnCompleteListenerMessagingImp(int type, MainActivity mainActivity)
        {
            this.type = type;
            this.mainActivity = mainActivity;
        }

        public void OnComplete(Task task)
        {

            if (task.IsSuccessful)
            {
                if (type == 0)
                {
                    Log.Info(TAG, "Subscribe Complete");
                    mainActivity.ShowLog("subscribe Complete");
                }
                else if (type == 1)
                {
                    Log.Info(TAG, "UnSubscribe Complete");
                    mainActivity.ShowLog("unsubscribe Complete");
                }
                else
                {
                    Log.Info(TAG, "Something went wrong");
                    mainActivity.ShowLog("Something went wrong");
                }

            }
            else
            {
                if (type == 0)
                {
                    Log.Info(TAG, "UnSubscribe Complete");
                    mainActivity.ShowLog("subscribe failed: ret=" + task.Exception.Message);
                }
                else if (type == 1)
                {
                    Log.Info(TAG, "UnSubscribe failed");
                    mainActivity.ShowLog("unsubscribe failed: ret=" + task.Exception.Message);
                }
                else
                {
                    Log.Info(TAG, "Something went wrong");
                    mainActivity.ShowLog("Something went wrong");
                }
            }
        }
    }


    class OnSuccessListenerImp : Java.Lang.Object, IOnSuccessListener
    {
        private static readonly string TAG = "MainActivity: OnSuccessListenerImp";
        private readonly int DELETE_AAID;
        private readonly Handler handler;
        private readonly MainActivity mainActivity;

        public OnSuccessListenerImp(Handler handler, int DELETE_AAID, MainActivity mainActivity)
        {
            this.DELETE_AAID = DELETE_AAID;
            this.handler = handler;
            this.mainActivity = mainActivity;
        }

        public void OnSuccess(Java.Lang.Object p0)
        {
            AAIDResult iAaid = (AAIDResult)p0;
            string aaId = iAaid.Id;
            Log.Info(TAG, "getAAID success:" + aaId);
            mainActivity.ShowLog("getAAID success:" + aaId);
            handler.SendEmptyMessage(DELETE_AAID);
        }
    }

    class OnFailureListenerImp : Java.Lang.Object, IOnFailureListener
    {
        private static readonly string TAG = "MainActivity: OnFailureListenerImp";
        private readonly MainActivity mainActivity;

        public OnFailureListenerImp(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Log.Error(TAG, "getAAID failed:" + e);
            mainActivity.ShowLog("getAAID failed." + e);
        }
    }

    class OnDialogClickListenerImp : IOnDialogClickListener
    {
        private static readonly string TAG = "MainActivity: OnDialogClickListenerImp";
        private readonly TopicDialog topicDialog;
        private readonly int actionType;
        private readonly MainActivity mainActivity;

        public OnDialogClickListenerImp(TopicDialog topicDialog, int actionType, MainActivity mainActivity)
        {
            this.topicDialog = topicDialog;
            this.actionType = actionType;
            this.mainActivity = mainActivity;
        }
        public void OnCancelClick()
        {
            topicDialog.Dismiss();
        }

        public void OnConfirmClick(string msg)
        {
            if (actionType == 1)
            {
                topicDialog.Dismiss();
                try
                {
                    HmsMessaging.GetInstance(mainActivity)
                        .Subscribe(msg)
                        .AddOnCompleteListener(new OnCompleteListenerMessagingImp(0, mainActivity));
                }
                catch (Exception e)
                {
                    mainActivity.ShowLog("subscribe failed: exception=" + e.Message);
                    Log.Error(TAG, "subscribe failed: exception=" + e);
                }
            }
            else
            {
                topicDialog.Dismiss();
                try
                {
                    HmsMessaging.GetInstance(mainActivity)
                        .Unsubscribe(msg)
                        .AddOnCompleteListener(new OnCompleteListenerMessagingImp(1, mainActivity));
                }
                catch (Exception e)
                {
                    mainActivity.ShowLog("unsubscribe failed: exception=" + e.Message);
                    Log.Error(TAG, "unsubscribe failed: exception=" + e);
                }

            }

        }
    }

    class mHandler : Handler
    {
        private readonly int GET_AAID, DELETE_AAID;
        private readonly string get_aaid, delete_aaid;
        private readonly TextView TvSetAAID;

        public mHandler(int GET_AAID, int DELETE_AAID, TextView tvSetAAID, string get_aaid, string delete_aaid)
        {
            this.GET_AAID = GET_AAID;
            this.DELETE_AAID = DELETE_AAID;
            this.TvSetAAID = tvSetAAID;
            this.get_aaid = get_aaid;
            this.delete_aaid = delete_aaid;
        }
        public override void HandleMessage(Message message)
        {
            if (message.What == GET_AAID)
            {
                TvSetAAID.Text = get_aaid;
            }
            else if (message.What == DELETE_AAID)
            {
                TvSetAAID.Text = delete_aaid;
            }

        }
    }


}