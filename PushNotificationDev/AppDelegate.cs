using Foundation;
using UIKit;
using BackendlessAPI;
using System;
using BackendlessAPI.Messaging;
using BackendlessAPI.Async; 
using BackendlessAPI.Exception;
using UserNotifications;
using AudioToolbox;
                    
namespace PushNotificationDev
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public void RegisterDeviceInBackendless(string token)
        {
            string id = null;
            var OS_VERSION = UIDevice.CurrentDevice.SystemVersion.ToString();
            var OS = "IOS";
            try
            {
                id = Guid.NewGuid().ToString();
            }
            catch (Exception e)
            {
                Console.Write($"Error {e.Message}");
            }

            var DEVICE_ID = id;
            //"174677789761"
            var deviceReg = new DeviceRegistration();
            deviceReg.Os = OS;
            deviceReg.OsVersion = OS_VERSION;
            deviceReg.Expiration = DateTime.Now.AddHours(3);
            deviceReg.DeviceId = DEVICE_ID;
            deviceReg.DeviceToken = token;
            Backendless.Messaging.DeviceRegistration = deviceReg;

            Backendless.Messaging.RegisterDevice(token, "default",  new AsyncCallback<string>(responseHanlder, errorHandler));
        }
        private void errorHandler(BackendlessFault fault)
        {
            System.Diagnostics.Debug.WriteLine("error sending token to backendless");
        }

        private void responseHanlder(string response)
        {
            //Toast.MakeText(this, "Registered", ToastLength.Short);
            //Toast.MakeText(this, Backendless.Messaging.DeviceRegistration.DeviceId, ToastLength.Short).Show();
            string id = Backendless.Messaging.DeviceRegistration.DeviceId;
        }
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method


            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                                   new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, 
                                                                      (boolVariable, error) =>{
                    System.Diagnostics.Debug.WriteLine(boolVariable.ToString(), error.ToString()); });
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }


            UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
            {
                if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized)
                    System.Diagnostics.Debug.WriteLine($"Error: {settings.AuthorizationStatus}");
            });

            if (launchOptions!=null && launchOptions.Keys.Length > 0)
            {


            }


            return true;
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
        {
            base.DidReceiveRemoteNotification(application, userInfo, completionHandler);

            System.Diagnostics.Debug.WriteLine("Received");
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            SystemSound.Vibrate.PlayAlertSound();
            SystemSound.Vibrate.PlaySystemSound();
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Get current device token
            var DeviceToken = deviceToken.Description;
            //"<d6912cef 723ba448 c168ce18 d10541a0 d5d576b0 7b510cf5 18290268 dea1e53a>"
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
                DeviceToken = DeviceToken.Trim('<').Trim('>');
            }

            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
                //TODO: Put your own logic here to notify your server that the device token has changed/been created!


            }
            RegisterDeviceInBackendless(DeviceToken);
            // Save new device token
            NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
        }
    }
}

