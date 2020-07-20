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
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace XamarinHmsPushDemo.Hmssample
{
    class TopicDialog : Dialog, View.IOnClickListener
    {
        private readonly View tView;

        private IOnDialogClickListener onDialogClickListener;

        private EditText edTopic;

        public TopicDialog(Context context, bool isAdd) : base(context, Resource.Style.custom_dialog)
        {
            tView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_add_topic, null);
            InitView(isAdd, context);
        }

        private void InitView(bool isAdd, Context context)
        {
            tView.FindViewById<TextView>(Resource.Id.tv_cancel).SetOnClickListener(this);
            tView.FindViewById<TextView>(Resource.Id.tv_confirm).SetOnClickListener(this);
            edTopic = tView.FindViewById<EditText>(Resource.Id.ed_topic);

            edTopic.SetHint(isAdd ? Resource.String.add_topic : Resource.String.delete_topic);

            edTopic.EditorAction += (object sender, TextView.EditorActionEventArgs e) =>
            {
                if (e.ActionId == ImeAction.Unspecified)
                {
                    InputMethodManager imm =
                        (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    if (imm != null)
                    {
                        imm.HideSoftInputFromWindow(Window.DecorView.WindowToken, 0);
                    }
                }
            };

            SetCanceledOnTouchOutside(false);
            SetContentView(tView);
        }


        public void OnClick(View v)
        {
            int viewId = v.Id;
            switch (viewId)
            {
                case Resource.Id.tv_cancel:
                    if (onDialogClickListener != null)
                    {
                        onDialogClickListener.OnCancelClick();
                    }
                    break;
                case Resource.Id.tv_confirm:
                    if (onDialogClickListener != null)
                    {
                        onDialogClickListener.OnConfirmClick(edTopic.Text.ToString());
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetOnDialogClickListener(IOnDialogClickListener onDialogClickListener)
        {
            this.onDialogClickListener = onDialogClickListener;
        }
    }


}