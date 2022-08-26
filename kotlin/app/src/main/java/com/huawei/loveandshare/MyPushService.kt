package com.huawei.loveandshare

import android.content.Intent
import android.os.Bundle
import android.util.Log
import com.huawei.hms.push.HmsMessageService
import com.huawei.hms.push.RemoteMessage

class MyPushService : HmsMessageService() {
    override fun onNewToken(token: String, bundle: Bundle) {
        Log.i(TAG, "receive token:$token")
        if (token.isNotEmpty()) {
            sendTokenToDisplay(token)
        }
    }

    override fun onTokenError(e: Exception) {
        super.onTokenError(e)
    }

    override fun onMessageReceived(remoteMessage: RemoteMessage?) {
        if (remoteMessage == null) {
            return
        }
        if ((remoteMessage.data?.length ?: -1) > 0) {
            Log.i(TAG, "Message data payload: " + remoteMessage.data)
        }
        if (remoteMessage.notification != null) {
            Log.i(TAG, "Message Notification Body: " + remoteMessage.notification.body)
        }
    }

    override fun onMessageSent(msgId: String?) {}

    override fun onSendError(msgId: String?, exception: Exception?) {}

    private fun sendTokenToDisplay(token: String) {
        val intent = Intent("com.huawei.codelabpush.ON_NEW_TOKEN")
        intent.putExtra("token", token)
        sendBroadcast(intent)
    }

    companion object {
        private const val TAG: String = "PushDemoLog"
    }
}