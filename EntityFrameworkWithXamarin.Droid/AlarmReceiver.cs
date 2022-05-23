using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;

namespace EntityFrameworkWithXamarin.Droid
{
    [BroadcastReceiver]
    class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "AlarmReceiver");
            //Acquire the lock
            wl.Acquire();

            Thread loggerThread = new Thread(new ThreadStart(Logger.Start));
            loggerThread.Start();


            Task.Run(() =>
            {
                Logger.StartAsJob();
                wl.Release();
                var TenMinsFromNow = (long)(Calendar.GetInstance(Android.Icu.Util.TimeZone.Default).TimeInMillis + TimeSpan.FromMinutes(5).TotalMilliseconds);
                Start(Application.Context, TenMinsFromNow);
            });

            //Release the lock
            System.Diagnostics.Debug.Print("DDbug " + DateTime.Now);
           // wl.Release();
        }

        public  void Start(Context context, long triggerAfterMilis)
        {
            var type = AlarmType.RtcWakeup;
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var timerIntent = PendingIntent.GetBroadcast(context, 0, new Intent(context, typeof(AlarmReceiver)), PendingIntentFlags.CancelCurrent);

            alarmManager.Cancel(timerIntent);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                alarmManager.SetAndAllowWhileIdle(type, triggerAfterMilis, timerIntent);
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                alarmManager.SetExact(type, triggerAfterMilis, timerIntent);
            else
                alarmManager.Set(type, triggerAfterMilis, timerIntent);
            System.Diagnostics.Debug.Print($"AlarmReceiver Started, trigger after {triggerAfterMilis} miliseconds.");
        }
    }
}