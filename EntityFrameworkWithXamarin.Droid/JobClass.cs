using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Util;
using System.Threading.Tasks;
using static Java.Util.Jar.Attributes;

namespace EntityFrameworkWithXamarin.Droid
{
    [Service(Name = "JobScheduleSample.JobScheduleSample.JobClass",
        Permission = "android.permission.BIND_JOB_SERVICE")]
   public  class JobClass : JobService
    {
        public static readonly string TAG = typeof(JobClass).FullName;

        public override bool OnStartJob(JobParameters @params)
        {
             Task.Run(() =>
            {
                Logger.StartAsJob();
                // Thread loggerThread = new Thread(new ThreadStart(Logger.Start));
                //  loggerThread.Start();

                // Work is happening asynchronously

                // Have to tell the JobScheduler the work is done. 
                JobFinished(@params, false);
                scheduleRefresh();
            });

            if (Build.VERSION.SdkInt != BuildVersionCodes.N)
            {
              //  scheduleRefresh();
            }

            return true;
        }

        private static int sJobId = 1;
        private static long REFRESH_INTERVAL = 5 * 100000;
        private void scheduleRefresh()
        {
           // JobParameters @params=null;
           // JobFinished( @params, false);
            var jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);
            var javaClass = Java.Lang.Class.FromType(typeof(JobClass));
            ComponentName jobServiceName = new ComponentName(Application.Context, javaClass);

            JobInfo.Builder jobBuilder = new JobInfo.Builder(sJobId, jobServiceName);

            if (Build.VERSION.SdkInt != BuildVersionCodes.N)
            {
                jobBuilder.SetMinimumLatency(REFRESH_INTERVAL);
            }
            else
            {
                jobBuilder.SetPeriodic(REFRESH_INTERVAL);
            }
            var jobInfo = jobBuilder.Build();
            int result = jobScheduler.Schedule(jobInfo);
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job " + result);
            }

        }

        public override bool OnStopJob(JobParameters @params)
        {
            // throw new NotImplementedException();
            return false;
        }


    }
}