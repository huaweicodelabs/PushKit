package com.example.pushdemo.myapplication009;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.text.TextUtils;

import com.huawei.agconnect.config.AGConnectServicesConfig;
import com.huawei.hms.aaid.HmsInstanceId;
import com.huawei.hms.common.ApiException;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        // This method is used to obtain a token required for accessing HUAWEI Push Kit.
        new Thread() {
            @Override
            public void run() {
                // If there is no local AAID, this method will automatically generate an AAID when it is called because the Huawei Push server needs to generate a token based on the AAID.
                // This method is a synchronous method, and you cannot call it in the main thread. Otherwise, the main thread may be blocked.
                try {
                    // read from agconnect-services.json.
                    String appId = AGConnectServicesConfig.fromContext(MainActivity.this).getString("client/app_id");
                    String token = HmsInstanceId.getInstance(MainActivity.this).getToken(appId, "HCM");
                    if (!TextUtils.isEmpty(token)) {
                        // get token successful, send token to server.
                        // a new token will be returned from HmsMessageService's method onNewToken(String).
                    }
                } catch (ApiException e) {
                    // get token failed.
                }
            }
        }.start();
    }
}