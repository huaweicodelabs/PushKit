package com.huawei.loveandshare

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.Bundle
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    private lateinit var tvToken: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        tvToken = findViewById(R.id.tv_log)
        val receiver = MyReceiver()
        val filter = IntentFilter()
        filter.addAction("com.huawei.codelabpush.ON_NEW_TOKEN")
        registerReceiver(receiver, filter)
    }

    inner class MyReceiver : BroadcastReceiver() {
        override fun onReceive(context: Context?, intent: Intent?) {
            if ("com.huawei.codelabpush.ON_NEW_TOKEN" == intent?.action) {
                val token = intent.getStringExtra("token")
                tvToken?.text = token
            }
        }
    }
}