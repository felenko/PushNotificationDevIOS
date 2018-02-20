using BackendlessAPI;
using UIKit;
using System;


namespace PushNotificationDev
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.

            string appId = "9BA31C05-1D39-4DE3-FF50-38EED1144E00";

            string secretKey = "A708523C-34F2-9FAF-FFFC-D58AC0A96F00";
            Backendless.URL = "http://api.backendless.com";
            try
            {
                Backendless.InitApp(appId, secretKey);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error {e.Message}");
            }
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
