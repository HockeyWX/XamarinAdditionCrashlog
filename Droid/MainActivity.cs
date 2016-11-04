using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Java.Lang;
using Java.IO;
using HockeyApp.Android;


#if HOCKEYAPP
using HockeyApp.Android;
#elif INSIGHTS
using Xamarin;
#endif
using System.Threading.Tasks;

namespace HockeySDKXamarinDemo.Droid
{
	[Activity(Label = "TestHockeyApp.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

		public const string AppID = "Your App ID";
       
        protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            HockeyApp.Android.Utils.HockeyLog.LogLevel = 3;

            MyCrashManagerListener listener = new MyCrashManagerListener();
            listener.userID = "v-zhjoh";
            listener.userContact = "v-zhjoh@microsoft.com";
            Android.Util.Log.Debug("HockeyApp","Add Debug info into Description");
           
            CrashManager.Register(this, AppID, listener);
            //CrashManager.Register(this, AppID);

            UpdateManager.Register(this, AppID);
            FeedbackManager.Register(this, AppID);

            global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(CreateApp());
		}

		protected override void OnResume()
		{
			base.OnResume();

			Tracking.StartUsage(this);
		}

		protected override void OnPause()
		{
			Tracking.StopUsage(this);

			base.OnPause();
		}


		private App CreateApp()
		{
			var app = new App();

			var ThrowNativeJavaExceptionButton = new Xamarin.Forms.Button {
				Text = "Throw Native Java Exception"
			};
			ThrowNativeJavaExceptionButton.Clicked += ThrowNativeJavaException;

            var FbBtn = new Xamarin.Forms.Button
            {
                Text = "Feedback"
            };
            FbBtn.Clicked += FeedbackClick;

            app.AddChild(ThrowNativeJavaExceptionButton);
            app.AddChild(FbBtn);


            return app;
		}

		public override bool OnPrepareOptionsMenu(IMenu menu) {
			try {
				// I am always getting menu.HasVisibleItems = false in my app
				if (menu != null && menu.HasVisibleItems) {
					// Exception is happening when the following code is executed
					var result = base.OnPrepareOptionsMenu(menu);
					return result;
				}
			}
			catch
			{
			}
			return true;
		}

		private void ThrowNativeJavaException(object sender, EventArgs e)
		{
			NativeJava.NativeJavaException.ThrowException("This is a native java exception.");
		}

        private void FeedbackClick(object sender, EventArgs e) {
            FeedbackManager.ShowFeedbackActivity(ApplicationContext);
        } 

    }


    public class MyCrashManagerListener : CrashManagerListener {

        public string userID { set; get; } = null;

        public override string UserID
        {
            get { return userID; }
        }

        public string userContact { set; get; } = null;

        public override string Contact
        {
            get { return userContact; }
        }

        //public string userDescription { set; get; } = null;

        public override string Description
        {
            get
            {
                return getDescription();
            }
        }

        public string getDescription()
        {
            string description = "";

            try
            {

                Java.Lang.Process process = Runtime.GetRuntime().Exec("logcat -d HockeyApp:D *:S");

                BufferedReader bufferedReader =
                    new BufferedReader(new InputStreamReader(process.InputStream));

                Java.Lang.StringBuilder log = new Java.Lang.StringBuilder();
                string line;
                while ((line = bufferedReader.ReadLine()) != null)
                {
                    log.Append(line);
                    log.Append(System.Environment.NewLine);
                }
                bufferedReader.Close();

                description = log.ToString();
            }
            catch (IOException e)
            {
            }

            return description;
        }

    }
}

