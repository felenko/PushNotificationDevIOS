using BackendlessAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using BackendlessAPI.Messaging;
using Weborb.Client;

namespace NetConsoleSender
{

    class Program
    {
        static void Main(string[] args)
        {
            String appId = "9BA31C05-1D39-4DE3-FF50-38EED1144E00";
            String secretKey = "61D82F0F-FA49-1BAE-FF3E-C03483D69600";
            Backendless.URL = "http://api.backendless.com";
            try
            {
                Backendless.InitApp(appId, secretKey);
                var msgStatus = PublishMessageAsPushNotificationSync("default");
            }
            catch (Exception e)
            {
                Console.Write($"Error {e.Message}");
            }
        }

        static private MessageStatus PublishMessageAsPushNotificationSync(string message)
        {

            var publishOptions = new PublishOptions();
            publishOptions.Headers = new Dictionary<string, string>()
            {
                {"android-content-title", "Notification title for Android"},
                {"android-content-text", "Notification text for Android"},
                { "android-ticker-text", "ticker"},
                { "ios-badge", "bage"},

            };


            DeliveryOptions deliveryOptions = new DeliveryOptions() {PushPolicy = PushPolicyEnum.ALSO, PushBroadcast = 1};

           // deliveryOptions.SetPushBroadcast(PushBroadcastMask.ANDROID | PushBroadcastMask.IOS);
            MessageStatus messageStatus = Backendless.Messaging.Publish("dfslkjfslf",
                message,
                publishOptions
                );
            if (messageStatus.ErrorMessage == null) {
                Console.WriteLine($"MessageStatus = {messageStatus.Status} Id {messageStatus.MessageId}");
                MessageStatus  updatedStatus = Backendless.Messaging.GetMessageStatus(messageStatus.MessageId);

                return messageStatus;
            }
            else {
                Console.WriteLine($"Server reported an error: {messageStatus.ErrorMessage}");
                return null;
            }
        }
    }
}
