using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Runtime.InteropServices;
using HockeyApp.iOS;


using System.Threading.Tasks;

namespace HockeySDKXamarinDemo.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
        const string AppID = "Your App ID";

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            DesciptionDelegate de = new DesciptionDelegate();
            de.description = "This is John";
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(AppID, de);
            manager.UserEmail = "v-zhjoh@microsoft.com";
            manager.UserId = "v-zhjoh";
            manager.UserName = "John Zhang";
            manager.DebugLogEnabled = true;
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation(); // This line is obsolete in crash only builds

            


            global::Xamarin.Forms.Forms.Init();
            LoadApplication(CreateApp());

            return base.FinishedLaunching(app, options);
        }



        private App CreateApp()
		{
			var app = new App();

			var ThrowNativeObjCExceptionButton = new Xamarin.Forms.Button {
				Text = "Throw native ObjC Exception"
			};
			ThrowNativeObjCExceptionButton.Clicked += ThrowNativeObjCException;

			app.AddChild(ThrowNativeObjCExceptionButton);

			return app;
		}
	
		[DllImport("libc")]
		private static extern int raise(int sig);

		private static void ThrowNativeObjCException(object sender, EventArgs e)
		{
			raise(6); // 6 == SIGABRT
		}
	}

    

    public class DesciptionDelegate : BITCrashManagerDelegate
    {


        public string description { set; get; } = null;

        public override string ApplicationLogForCrashManager(BITCrashManager crashManager)
        {
            return description;
        }

    }
}

