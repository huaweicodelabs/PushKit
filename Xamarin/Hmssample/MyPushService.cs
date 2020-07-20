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
using Com.Huawei.Hms.Push;
using Android.Util;
using Android.Text;

namespace XamarinHmsPushDemo.Hmssample
{

    [Service]
    [IntentFilter(new[] { "com.huawei.push.action.MESSAGING_EVENT" })]
    public class MyMessagingService : HmsMessageService
    {

        private static readonly string TAG = "My Messaging Service";
        private readonly static string PUSHDEMO_ACTION = "com.huawei.xamarinhmspushdemo.action";

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Info(TAG, "receive token :" + token);

            if (!TextUtils.IsEmpty(token))
            {
                // This method callback must be completed in 10 seconds. Otherwise, you need to start a new Job for callback processing.
                refreshedTokenToServer(token);
            }

            Intent intent = new Intent();
            intent.SetAction(PUSHDEMO_ACTION);
            intent.PutExtra("method", "onNewToken");
            intent.PutExtra("msg", "onNewToken called, token: " + token);

            SendBroadcast(intent);
        }

        private void refreshedTokenToServer(string token)
        {
            Log.Info(TAG, "sending token to server. token:" + token);
        }

        /**
            * This method is used to receive downstream data messages.
            * This method callback must be completed in 10 seconds. Otherwise, you need to start a new Job for callback processing.
            *
            * @param message RemoteMessage
          */
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Info(TAG, "onMessageReceived is called");
            if (message == null)
            {
                Log.Error(TAG, "Received message entity is null!");
                return;
            }

            Log.Info(TAG, "getCollapseKey: " + message.CollapseKey
                    + "\n getData: " + message.Data
                    + "\n getFrom: " + message.From
                    + "\n getTo: " + message.To
                    + "\n getMessageId: " + message.MessageId
                    + "\n getOriginalUrgency: " + message.OriginalUrgency
                    + "\n getUrgency: " + message.Urgency
                    + "\n getSendTime: " + message.SentTime
                    + "\n getMessageType: " + message.MessageType
                    + "\n getTtl: " + message.Ttl);

            RemoteMessage.Notification notification = message.GetNotification();
            if (notification != null)
            {
                Log.Info(TAG, "\n getImageUrl: " + notification.ImageUrl
                        + "\n getTitle: " + notification.Title
                        + "\n getTitleLocalizationKey: " + notification.TitleLocalizationKey
                        + "\n getBody: " + notification.Body
                        + "\n getBodyLocalizationKey: " + notification.BodyLocalizationKey
                        + "\n getIcon: " + notification.Icon
                        + "\n getSound: " + notification.Sound
                        + "\n getTag: " + notification.Tag
                        + "\n getColor: " + notification.Color
                        + "\n getClickAction: " + notification.ClickAction
                        + "\n getChannelId: " + notification.ChannelId
                        + "\n getLink: " + notification.Link
                        + "\n getNotifyId: " + notification.NotifyId);
            }

            Intent intent = new Intent();
            intent.SetAction(PUSHDEMO_ACTION);
            intent.PutExtra("method", "onMessageReceived");
            intent.PutExtra("msg", "onMessageReceived called, message id:" + message.MessageId + ", payload data:"
                    + message.Data);

            SendBroadcast(intent);

            bool judgeWhetherIn10s = false;

            // If the messages are not processed in 10 seconds, the app needs to use WorkManager for processing.
            if (judgeWhetherIn10s)
            {
                StartWorkManagerJob(message);
            }
            else
            {
                // Process message within 10s
                ProcessWithin10s(message);
            }
        }

        private void StartWorkManagerJob(RemoteMessage message)
        {
            Log.Info(TAG, "Start new Job processing.");
        }
        private void ProcessWithin10s(RemoteMessage message)
        {
            Log.Info(TAG, "Processing now.");
        }

        public override void OnMessageSent(string msgId)
        {
            Log.Info(TAG, "onMessageSent called, Message id:" + msgId);
            Intent intent = new Intent();
            intent.SetAction(PUSHDEMO_ACTION);
            intent.PutExtra("method", "onMessageSent");
            intent.PutExtra("msg", "onMessageSent called, Message id:" + msgId);

            SendBroadcast(intent);
        }

        public override void OnSendError(string msgId, Java.Lang.Exception exception)
        {
            Log.Info(TAG, "onSendError called, message id:" + msgId + ", ErrCode:"
                    + ((SendException)exception).ErrorCode + ", description:" + exception.Message);

            Intent intent = new Intent();
            intent.SetAction(PUSHDEMO_ACTION);
            intent.PutExtra("method", "onSendError");
            intent.PutExtra("msg", "onSendError called, message id:" + msgId + ", ErrCode:"
                + ((SendException)exception).ErrorCode + ", description:" + exception.Message);

            SendBroadcast(intent);
        }

    }

}