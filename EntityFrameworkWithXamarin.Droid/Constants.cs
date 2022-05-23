using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EntityFrameworkWithXamarin.Droid
{
	public static class Constants
	{
		public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
		public const string SERVICE_STARTED_KEY = "has_service_been_started";
		public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
		public const string NOTIFICATION_BROADCAST_ACTION = "ForegraundService.Notification.Action";

		public const string ACTION_START_SERVICE = "ForegraundService.action.START_SERVICE";
		public const string ACTION_STOP_SERVICE = "ForegraundService.action.STOP_SERVICE";
		public const string ACTION_RETURN_SERVICE_STATE = "ForegraundService.action.RETURN_SERVICE_STATE";
		public const string ACTION_MAIN_ACTIVITY = "ForegraundService.action.MAIN_ACTIVITY";
		public const string ACTION_UPDATE_FILTR = "ForegraundService.action.UPDATE_FILTR";

		public const string REBIND_ACTION = "ForegraundService.action.REBIND_ACTION";
		public const string STOP_ACTION = "ForegraundService.action.STOP_ACTION";

		public const string CHANNEL_ID = "ForegraundService_notification";

		public const string REQUEST_NOTIFI = "BindNotificationListenerService";
		public static string JOB_SCHEDUKER = "JobSchedulerService";


		

	}
}