// using System;
// using Unity.Notifications.Android;
using UnityEngine;

namespace TicTacToePro.Notifications
{
    public class NotificationManager: MonoBehaviour
    {
    #if UNITY_ANDROID
        public OctoNotificationsSettings Settings;

        void Start()
        {
            Load();

            Reset();
            CreateGameNotifications();
            CreateDailyChallengeNotification();
        }

        void Load()
        {
            AndroidNotificationCenter.Initialize();
        }

        void Reset()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        void CreateGameNotifications()
        {
            string channelId = FallbackText(Settings.GameChannelId, "TicTacToe_Game");

            var channel = new AndroidNotificationChannel()
            {
                Id = channelId,
                Name = FallbackText(Settings.GameChannelName, "TicTacToe"),
                Importance = Importance.Default,
                Description = FallbackText(Settings.GameChannelDescription, "Another opponent awaits you!"),
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var gameNotifications = Settings.GameNotifications;

            foreach (var notificationSetting in gameNotifications)
            {
                var notification = new AndroidNotification();
                notification.Title = FallbackText(notificationSetting.Title, "TicTacToe");
                notification.Text = FallbackText(notificationSetting.Text, "Another opponent awaits you!");
                notification.FireTime = System.DateTime.Now + TimeSpan.FromDays(notificationSetting.Days);
                notification.SmallIcon = notificationSetting.SmallIconId;
                notification.LargeIcon = notificationSetting.LargeIconId;

                AndroidNotificationCenter.SendNotification(notification, channelId);
            }
        }
        void CreateDailyChallengeNotification()
        {
            string channelId = FallbackText(Settings.DailyChallengeChannelId, "TicTacToe_Daily");

            var channel = new AndroidNotificationChannel()
            {
                Id = channelId,
                Name = FallbackText(Settings.DailyChallengeChannelName, "TicTacToeGameDaily"),
                Importance = Importance.Default,
                Description = FallbackText(Settings.DailyChallengeChannelDescription, "Daily Challenge TicTacToe Notification"),
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var notificationSetting = Settings.DailyChallengeNotification;

            var notification = new AndroidNotification();
            notification.Title = FallbackText(notificationSetting.Title, "TicTacToe");
            notification.Text = FallbackText(notificationSetting.Text, "Let's complete the daily challenge!");
            notification.FireTime = System.DateTime.Now + TimeSpan.FromDays(notificationSetting.Days);
            notification.SmallIcon = notificationSetting.SmallIconId;
            notification.LargeIcon = notificationSetting.LargeIconId;

            AndroidNotificationCenter.SendNotification(notification, channelId);
        }
        private string FallbackText(string source, string fallback)
        {
            if (string.IsNullOrEmpty(source))
                return fallback;

            return source;
        }
    #endif
    }
}
