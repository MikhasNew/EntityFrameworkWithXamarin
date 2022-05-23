using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using EntityFrameworkWithXamarin.Core;
using Microsoft.EntityFrameworkCore;
using Nancy.Helpers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EntityFrameworkWithXamarin.Droid
{
    [Service]
  public  class ForegraundService : Service
    {

        /// <summary>
        /// переменные для парсера
        /// </summary>
        /// 
        private static List<TableItem> tbItems = new List<TableItem>();
        public static object obj = new object();
        const string TOKEN = "1123919290:AAFtjId-l9-_9uBepk9jLnWJD4m9Ms0qROU";
        // public TelegramBotClient bot = new TelegramBotClient(TOKEN);
        public static List<string[]> stopList = new List<string[]>();
        public static TelegramBotClient bot = new TelegramBotClient(TOKEN);
        static int offset = 0;
        static int timeaut = 0;
        public static bool isStarted = new bool();
        static string dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        static string fileName = "Users.db";
        public static string dbFullPath = Path.Combine(dbFolder, fileName);
        
        //public static JobScheduler jobScheduler;

        public static string TAG = typeof(ForegraundService).FullName;
        // Handler handler;

        AlarmReceiver alarmReciver = new AlarmReceiver();

        public override void OnCreate()
        {
            base.OnCreate();
            Main();
            Log.Info(TAG, "OnCreate: the service is initializing.");
           // handler = new Handler();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {


            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                   // UiUpdate("isStarted", true);
                }
                else
                {
                    Log.Info(TAG, "OnStartCommand: The service is starting.");
                    RegisterForegroundService();
                    //handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);

                    isStarted = true;
                    //*  UiUpdate("ServiceStarted", isStarted);
                    Log.Info(TAG, "Get UiState:" + isStarted.ToString());
                    // UiUpdate("isStarted", true);

                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, "OnStartCommand: The service is stopping.");

                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
            else if (intent.Action.Equals(Constants.ACTION_RETURN_SERVICE_STATE))
            {
                //*  UiUpdate("ServiceStarted", isStarted);
                Log.Info(TAG, "Get UiState:" + isStarted.ToString());
            }
            // alarmIt();
            // This tells Android not to restart the service if it is killed to reclaim resources.
           
            return StartCommandResult.Sticky;

        }
        public override IBinder OnBind(Intent intent)
        {

            return null;
        }
        public override void OnDestroy()
        {
            // We need to shut things down.
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");
            /*
            // Stop the handler.

            //handler.RemoveCallbacks(runnable);

            // Remove the notification from the status bar.
                        
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);

            Intent intent1 = new Intent(this, typeof(OnListenerConnectedToNotifi1));
            intent1.SetAction(Constants.STOP_ACTION);
            PendingIntent pendingIntent1 = PendingIntent.GetService(this, 0, intent1, 0);
            pendingIntent1.Send();

            // остановка manager. pendingIntent1 должен быть таким же как при запуске
            intent1.SetAction(Constants.REBIND_ACTION);
            pendingIntent1 = PendingIntent.GetService(this, 0, intent1, 0);
            manager.Cancel(pendingIntent1);
            */

            isStarted = false;
            base.OnDestroy();
        }
      /*  public override void OnTaskRemoved(Intent rootIntent)
        {
            if (Build.VERSION.SdkInt >=BuildVersionCodes.Kitkat)
            {
                Intent restartIntent = new Intent(this, Application.Class);

                AlarmManager am = (AlarmManager)GetSystemService(AlarmService);
                PendingIntent pi = PendingIntent.GetService(this, 1, restartIntent,
                        PendingIntentFlags.OneShot);
                restartIntent.PutExtra("RESTART",

                am.SetExact(AlarmManager.RTC, System.currentTimeMillis() + 3000, pi);
            }
        }*/
        public  void RegisterForegroundService()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(Constants.CHANNEL_ID, "Channel", NotificationImportance.Default)
                {
                    Description = "Foreground Service Channel"
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);

                var notification = new Notification.Builder(this, Constants.CHANNEL_ID)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_circle)
                // .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();


                // Enlist this instance of the service as a foreground service

                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            else
            {
                var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_circle)
               // .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();


                // Enlist this instance of the service as a foreground service

                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
        }

        ////////
        /// <summary>
        /// 
        /// </summary>
        static Action runnable;
        static Handler handler;
        private PowerManager.WakeLock wakeLock = null;
        async void Main()
        {

            PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
            wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, this.PackageName);
            wakeLock.Acquire();

            Console.WriteLine("Сервис запущен!");
            /// var logger = new Logger();
           
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            bot.StartReceiving();

            scheduleForegraundService();
            //scheduleрHandler();


            /*  const double interval10Minutes = 60 * 1 * 1000; // milliseconds to 10 minute
            System.Timers.Timer checkForTime = new System.Timers.Timer(interval10Minutes);
            
            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            checkForTime.Enabled = true; */

            // var TenMinsFromNow = (long)(Calendar.GetInstance(Android.Icu.Util.TimeZone.Default).TimeInMillis + TimeSpan.FromSeconds(30).TotalMilliseconds);
            // alarmReciver.Start(Application.Context, TenMinsFromNow);
            //scheduleRefresh();

            //  Thread loggerThread = new Thread(new ThreadStart(Logger.Start));
            //  loggerThread.Start();

            /*   while (true)
               {
                   try
                   {
                       GetMessage().Wait();
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine("Error " + ex);
                       Thread.Sleep(5000);
                       //  Console.WriteLine("Перезапускаем программу из Program");
                       //  Process.Start(@"C:\Users\Mi PC\source\repos\kufar\ConsoleParserKufar\ConsoleParserKufar\bin\Debug\netcoreapp3.0\ConsoleParserKufar2.exe");
                   }
               }*/
        }

        private static int sJobId = 1;
        private static long REFRESH_INTERVAL = 5 * 1000;
        private static void scheduleRefresh()
        {

            var jobScheduler = (JobScheduler)Application.Context.GetSystemService(JobSchedulerService);

            var javaClass = Java.Lang.Class.FromType(typeof(JobClass));
            ComponentName jobServiceName = new ComponentName(Application.Context, javaClass);

            JobInfo.Builder jobBuilder = new JobInfo.Builder(sJobId, jobServiceName);

            if (Build.VERSION.SdkInt != BuildVersionCodes.N)
            {
               // jobBuilder.SetPeriodic(REFRESH_INTERVAL);
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
        private static void scheduleрHandler()
        {
            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {
                // Thread loggerThread = new Thread(new ThreadStart(Logger.StartAsJob));
                // loggerThread.Start();
                Task.Run(() =>
                {
                    Logger.StartAsJob();
                    // Thread loggerThrea
                    handler.PostDelayed(runnable, 10*60*1000);
                });

            });
            handler.Post(runnable);
        }
        private void scheduleForegraundService()
        {
            var startServiceIntent = new Intent(this, typeof(ForegraundLogger));
            startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                StartForegroundService(startServiceIntent);
            else
                StartService(startServiceIntent);
        }
        void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.StartAsJob();
            // Thread loggerThread = new Thread(new ThreadStart(Logger.StartAsJob));
            // loggerThread.Start();
        }
        private static async  void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var opdetes = await bot.GetUpdatesAsync(offset, timeaut, allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery, UpdateType.ChosenInlineResult });

            foreach (var opdet in opdetes)
            {

                tbItems.Clear();
                if (opdet.CallbackQuery != null)
                {
                    offset = opdet.Id + 1;
                    var message = opdet.CallbackQuery.Message;
                    if (opdet.CallbackQuery.Data.Contains("#?"))
                    {
                        DellEntry(message.Chat.Id, opdet.CallbackQuery.Data.Replace("#?", ""));
                        //RecordEntry("great", $"./Remuv/{opdet.CallbackQuery.Data.Replace("#?", "")}");
                        await bot.AnswerCallbackQueryAsync(opdet.CallbackQuery.Id, "Фильтр будет удален" + opdet.CallbackQuery.Data, true);
                        var ikb = opdet.CallbackQuery.Message.ReplyMarkup.InlineKeyboard;
                        var ikbiu = ikb.ToArray();

                        foreach (List<InlineKeyboardButton> kb in ikb)
                        {
                            if (kb[0].CallbackData == opdet.CallbackQuery.Data)
                            {
                                kb.Remove(kb[0]);
                            }
                        }

                        await bot.EditMessageReplyMarkupAsync(opdet.CallbackQuery.Message.Chat.Id, opdet.CallbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(ikb));
                        // await Bot.SendTextMessageAsync(message.Chat.Id, "тест", replyToMessageId: message.MessageId);
                        // await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                    }
                    if (opdet.CallbackQuery.Data.Contains("?#"))
                    {




                    }

                }
                if (opdet.Message != null)
                {
                    offset = opdet.Id + 1;
                    var message = opdet.Message;
                    //стартовый диалог
                    if (message.Text == "/start")
                    {
                        SendMessage(message.Chat.Id.ToString(), "Привет!", message.Chat.FirstName + " " + message
                        .Chat.LastName + ", для ввода поискового запроса используй выражение типа #запрос, для удаления - #?запрос", "", null);
                    }
                    //переименование
                    var botName = bot.GetMeAsync().Result.Username;
                    if (message.Text.Contains($"{botName} /rename \n"))
                    {
                        var rnt = message.Text.Replace($"@{botName} /rename \n ", "");

                        //   FileInfo fileInf = new FileInfo($"./Fillters/{message.Chat.Id}!" +
                        //       $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        //    if (fileInf.Exists)
                        //    {
                        //       SendMessage(message.Chat.Id.ToString(), $"Фильтр с таким именем уже существует.", "Попробуйте еще раз", "", null);
                        //      return;
                        //   }

                        RenameEntryTbItem(message.Chat.Id, $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}",
                            $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}");

                        //  RecordEntry("great", $"./Rename/{message.Chat.Id}!" +
                        //      $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}" +
                        //      $"!{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        SendMessage(message.Chat.Id.ToString(), $"Запрос будед переименован в ближайшее время.", "", "", null);
                    }

                    int indexOfChar0 = message.Text.IndexOf('#');//симлол добавления строки поиска
                    int indexOfChar1 = message.Text.IndexOf('?');//символ удаления строки поиска

                    if (indexOfChar0 == 0)
                    {
                        lock (obj)
                        {
                            //удаление поискового запроса
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос будед удален в ближайшее время.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            //добавление поиска из #строки
                            else
                            {
                                tbItems.Add(new TableItem { A = "" });
                                AddEntry(message.Chat.Id, message.Text.Trim('#'), message.Text.Trim('#'));

                                // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{message.Text.Trim('#')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                    $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }

                        }
                    }

                    if (message.Text.Contains("https:"))
                    {
                        lock (obj)
                        {
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос удален.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            else
                            {

                                var mess = message.Text.Split("https://");
                                tbItems.Add(new TableItem { Url = $"https://{mess[1]}" });
                                if (mess[1].Contains("kufar.by"))
                                {
                                    string name = "";
                                    //var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var url = HttpUtility.UrlDecode(mess[1]).Split("query=");
                                    if (mess[0] == "")
                                         name = $"kufar {url[1].Replace("&ot", "")}";
                                      //  name = $"{url[1].Replace("&", " ")}";
                                    else
                                    {
                                        url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                        name = $"kufar {mess[0].Split(' ')[0]} {mess[0].Split(' ')[1].Trim(',')}";
                                    }
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("onliner.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var name = $"onliner {url[1].Replace("&f", "")}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("av.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split(".by/");
                                    var name = $"av {url[1].Replace("&f", "").Replace("&", " ").Replace("/", " ").Replace("?", "").Replace("[0]", "").Split(" ")[0]}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                 $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }
                        }

                    }
                    if (message.Text == "Добавить фильтр")
                    {

                        var irkm = new InlineKeyboardMarkup(new[] {

                                        new []
                                        {
                                            InlineKeyboardButton.WithUrl("Добавить фильтр по Барахолке", "https://baraholka.onliner.by"),
                                        },
                                        // second row
                                        new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Куфаре", "https://www.kufar.by/listings"),

                                        },
                                         new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Av.by (Test!)", "https://cars.av.by"),

                                        }

                                    });
                        SendMessage(message.Chat.Id.ToString(), $"Добавить фильтор.",
                       $"{message.Chat.FirstName}, чтобы добавить оповещение о новых объявлениях на Куфаре или Барахолке" +
                       $" в соответсии с выбранными критериями, поделись с ботом ссылкой на страницу поиска.  Ты также можешь" +
                       $" использовать строку ввода для поиска на всех ресурсах по #ключевому слову. ", "", irkm);
                    }
                    if (message.Text == "Удалить фильтр")
                    {
                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {
                            InlineKeyboardButton[] ikb = new[] { InlineKeyboardButton.WithCallbackData(file, $"#?{file}") };
                            buttonList.Add(ikb);


                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Удалить фильтр.",
                       $"{message.Chat.FirstName}, чтобы удалить поисковый запрос нажми одноименную кнопку. ", "", irkm);
                        //var ghhjkgh=  bot.GetChatAsync(message.Chat.Id);
                    }
                    if (message.Text == "Мои фильтры")
                    {

                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {

                            InlineKeyboardButton[] ikb = new[]
                            {
                                            InlineKeyboardButton.WithCallbackData
                                                        (file,
                                                        "1"),
                                           InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("переименовать",$"/rename \n {file} => ")
                                        };
                            buttonList.Add(ikb);

                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Вот они:",
                       "", "", irkm);

                    }
                    Console.WriteLine("Получено сообщение " + message.Text);


                    // await bot.SendTextMessageAsync(message.Chat.Id, "Привет я твой бот" + message.Chat.Username);


                }
            }
        }
        private  async void Bot_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
           var opdet = messageEventArgs.Message;

               tbItems.Clear();
            try
            {
                if (opdet.Text != null)
                {
                    offset = opdet.MessageId + 1;
                    var message = opdet;
                    //стартовый диалог
                    if (message.Text == "/start")
                    {
                        SendMessage(message.Chat.Id.ToString(), "Привет!", message.Chat.FirstName + " " + message
                        .Chat.LastName + ", для ввода поискового запроса используй выражение типа #запрос, для удаления - #?запрос", "", null);
                    }
                    //переименование
                    var botName = bot.GetMeAsync().Result.Username;
                    if (message.Text.Contains($"{botName} /rename \n"))
                    {
                        var rnt = message.Text.Replace($"@{botName} /rename \n ", "");

                        //   FileInfo fileInf = new FileInfo($"./Fillters/{message.Chat.Id}!" +
                        //       $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        //    if (fileInf.Exists)
                        //    {
                        //       SendMessage(message.Chat.Id.ToString(), $"Фильтр с таким именем уже существует.", "Попробуйте еще раз", "", null);
                        //      return;
                        //   }

                        RenameEntryTbItem(message.Chat.Id, $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}",
                            $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}");

                        //  RecordEntry("great", $"./Rename/{message.Chat.Id}!" +
                        //      $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}" +
                        //      $"!{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        SendMessage(message.Chat.Id.ToString(), $"Запрос будед переименован в ближайшее время.", "", "", null);
                    }

                    int indexOfChar0 = message.Text.IndexOf('#');//симлол добавления строки поиска
                    int indexOfChar1 = message.Text.IndexOf('?');//символ удаления строки поиска

                    if (indexOfChar0 == 0)
                    {
                        lock (obj)
                        {
                            //удаление поискового запроса
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос будед удален в ближайшее время.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            //добавление поиска из #строки
                            else
                            {
                                tbItems.Add(new TableItem { A = "" });
                                AddEntry(message.Chat.Id, message.Text.Trim('#'), message.Text.Trim('#'));

                                // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{message.Text.Trim('#')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                    $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }

                        }
                    }

                    if (message.Text.Contains("https:"))
                    {
                        lock (obj)
                        {
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос удален.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            else
                            {

                                var mess = message.Text.Split("https://");
                                tbItems.Add(new TableItem { Url = $"https://{mess[1]}" });
                                if (mess[1].Contains("21vek.by/special_offers"))
                                {
                                    string name = "";
                                    // var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var url = HttpUtility.UrlDecode(mess[1]).Split("#");
                                    if (url.Length >= 2)
                                        name = $"21Vek {url[1]}";
                                    else
                                        name = $"21Vek"; // зам -ил url na mess 06.12.20
                                   
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                }
                                if (mess[1].Contains("kufar.by"))
                                {
                                    string name = "";
                                    // var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var url = HttpUtility.UrlDecode(mess[1]).Split("query=");
                                    if (mess[0] == "")
                                        // name = $"{url[1].Replace("&ot", "")}";
                                        if (url.Count()==1)
                                            name = $"kufar {mess[1].Replace("&", " ")}";
                                        else
                                            name = $"kufar {url[1].Replace("&", " ")}"; // зам -ил url na mess 06.12.20
                                    else
                                        name = $"kufar {mess[0].Split(' ')[0]} {mess[0].Split(' ')[1].Trim(',')}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("onliner.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var name = $"onliner {url[1].Replace("&f", "")}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("av.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split(".by/");
                                    var name = $"av {url[1].Replace("&f", "").Replace("&", " ").Replace("/", " ").Replace("?", "").Replace("[0]", "").Split(" ")[0]}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                 $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }
                        }

                    }
                    if (message.Text == "Добавить фильтр")
                    {

                        var irkm = new InlineKeyboardMarkup(new[] {

                                        new []
                                        {
                                            InlineKeyboardButton.WithUrl("Добавить фильтр по Барахолке", "https://baraholka.onliner.by"),
                                        },
                                        // second row
                                        new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Куфаре", "https://www.kufar.by/listings"),

                                        },
                                         new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Av.by (Test!)", "https://cars.av.by"),

                                        }

                                    });
                        SendMessage(message.Chat.Id.ToString(), $"Добавить фильтор.",
                       $"{message.Chat.FirstName}, чтобы добавить оповещение о новых объявлениях на Куфаре или Барахолке" +
                       $" в соответсии с выбранными критериями, поделись с ботом ссылкой на страницу поиска.  Ты также можешь" +
                       $" использовать строку ввода для поиска на всех ресурсах по #ключевому слову. ", "", irkm);
                    }
                    if (message.Text == "Удалить фильтр")
                    {
                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {
                            InlineKeyboardButton[] ikb = new[] { InlineKeyboardButton.WithCallbackData(file, $"#?{file}") };
                            buttonList.Add(ikb);
                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Удалить фильтр.",
                       $"{message.Chat.FirstName}, чтобы удалить поисковый запрос нажми одноименную кнопку. ", "", irkm); 
                        //var ghhjkgh=  bot.GetChatAsync(message.Chat.Id);
                    }
                    if (message.Text == "Мои фильтры")
                    {

                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {

                            InlineKeyboardButton[] ikb = new[]
                            {
                                            InlineKeyboardButton.WithCallbackData
                                                        (file,
                                                        "1"),
                                           InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("переименовать",$"/rename \n {file} => ")
                                        };
                            buttonList.Add(ikb);

                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Вот они:",
                       "", "", irkm);

                    }
                    if (message.Text == "Проверить сейчас")
                    {
                        scheduleForegraundService();
                    }    
                    Console.WriteLine("Получено сообщение " + message.Text);


                    // await bot.SendTextMessageAsync(message.Chat.Id, "Привет я твой бот" + message.Chat.Username);


                }
            }
            catch (Exception e)
            { 
                var ee = e;
            }
        }

        /*private static async void Bot_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
           // var opdetes1 = messageEventArgs.Message;

            var opdetes = await bot.GetUpdatesAsync(offset, timeaut, allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery, UpdateType.ChosenInlineResult });

            foreach (var opdet in opdetes)
            {
               // if (opdet.Message.Text=="111")
                //    scheduleRefresh();


                tbItems.Clear();
                if (opdet.CallbackQuery != null)
                {
                    offset = opdet.Id + 1;
                    var message = opdet.CallbackQuery.Message;
                    if (opdet.CallbackQuery.Data.Contains("#?"))
                    {
                        DellEntry(message.Chat.Id, opdet.CallbackQuery.Data.Replace("#?", ""));
                        //RecordEntry("great", $"./Remuv/{opdet.CallbackQuery.Data.Replace("#?", "")}");
                        await bot.AnswerCallbackQueryAsync(opdet.CallbackQuery.Id, "Фильтр будет удален" + opdet.CallbackQuery.Data, true);
                        var ikb = opdet.CallbackQuery.Message.ReplyMarkup.InlineKeyboard;
                        var ikbiu = ikb.ToArray();

                        foreach (List<InlineKeyboardButton> kb in ikb)
                        {
                            if (kb[0].CallbackData == opdet.CallbackQuery.Data)
                            {
                                kb.Remove(kb[0]);
                            }
                        }

                        await bot.EditMessageReplyMarkupAsync(opdet.CallbackQuery.Message.Chat.Id, opdet.CallbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(ikb));
                        // await Bot.SendTextMessageAsync(message.Chat.Id, "тест", replyToMessageId: message.MessageId);
                        // await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                    }
                    if (opdet.CallbackQuery.Data.Contains("?#"))
                    {




                    }

                }
                if (opdet.Message != null)
                {
                    offset = opdet.Id + 1;
                    var message = opdet.Message;
                    //стартовый диалог
                    if (message.Text == "/start")
                    {
                        SendMessage(message.Chat.Id.ToString(), "Привет!", message.Chat.FirstName + " " + message
                        .Chat.LastName + ", для ввода поискового запроса используй выражение типа #запрос, для удаления - #?запрос", "", null);
                    }
                    //переименование
                    var botName = bot.GetMeAsync().Result.Username;
                    if (message.Text.Contains($"{botName} /rename \n"))
                    {
                        var rnt = message.Text.Replace($"@{botName} /rename \n ", "");

                        //   FileInfo fileInf = new FileInfo($"./Fillters/{message.Chat.Id}!" +
                        //       $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        //    if (fileInf.Exists)
                        //    {
                        //       SendMessage(message.Chat.Id.ToString(), $"Фильтр с таким именем уже существует.", "Попробуйте еще раз", "", null);
                        //      return;
                        //   }

                        RenameEntryTbItem(message.Chat.Id, $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}",
                            $"{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}");

                        //  RecordEntry("great", $"./Rename/{message.Chat.Id}!" +
                        //      $"{rnt.Split("=>")[0].TrimStart(' ').TrimEnd(' ')}" +
                        //      $"!{rnt.Split("=>")[1].TrimStart(' ').TrimEnd(' ')}.xml");
                        SendMessage(message.Chat.Id.ToString(), $"Запрос будед переименован в ближайшее время.", "", "", null);
                    }

                    int indexOfChar0 = message.Text.IndexOf('#');//симлол добавления строки поиска
                    int indexOfChar1 = message.Text.IndexOf('?');//символ удаления строки поиска

                    if (indexOfChar0 == 0)
                    {
                        lock (obj)
                        {
                            //удаление поискового запроса
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос будед удален в ближайшее время.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            //добавление поиска из #строки
                            else
                            {
                                tbItems.Add(new TableItem { A = "" });
                                AddEntry(message.Chat.Id, message.Text.Trim('#'), message.Text.Trim('#'));

                                // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{message.Text.Trim('#')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                    $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }

                        }
                    }

                    if (message.Text.Contains("https:"))
                    {
                        lock (obj)
                        {
                            if (indexOfChar1 == 1)
                            {
                                DellEntry(message.Chat.Id, message.Text.Trim('#').Trim('?'));
                                // RecordEntry("great", $"./Remuv/{message.Chat.Id}!{message.Text.Trim('#').Trim('?')}.xml");
                                SendMessage(message.Chat.Id.ToString(), $"Запрос удален.", $"{message.Chat.FirstName}, " +
                                   $"теперь бот не будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#').Trim('?')}/", "", null);
                            }
                            else
                            {

                                var mess = message.Text.Split("https://");
                                tbItems.Add(new TableItem { Url = $"https://{mess[1]}" });
                                if (mess[1].Contains("kufar.by"))
                                {
                                    string name = "";
                                    // var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var url = HttpUtility.UrlDecode(mess[1]).Split("query=");
                                    if (mess[0] == "")
                                        // name = $"{url[1].Replace("&ot", "")}";
                                        name = $"kufar {url[1].Replace("&", " ")}";
                                    else
                                        name = $"kufar {mess[0].Split(' ')[0]} {mess[0].Split(' ')[1].Trim(',')}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("onliner.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var name = $"onliner {url[1].Replace("&f", "")}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("av.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split(".by/");
                                    var name = $"av {url[1].Replace("&f", "").Replace("&", " ").Replace("/", " ").Replace("?", "").Replace("[0]", "").Split(" ")[0]}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                SendMessage(message.Chat.Id.ToString(), $"Запрос добавлен.", $"{message.Chat.FirstName}, " +
                                 $"теперь бот будет оповещать о новых объявлениях по запросу /{message.Text.Trim('#')}/", "", null);
                            }
                        }

                    }
                    if (message.Text == "Добавить фильтр")
                    {

                        var irkm = new InlineKeyboardMarkup(new[] {

                                        new []
                                        {
                                            InlineKeyboardButton.WithUrl("Добавить фильтр по Барахолке", "https://baraholka.onliner.by"),
                                        },
                                        // second row
                                        new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Куфаре", "https://www.kufar.by/listings"),

                                        },
                                         new []
                                        {
                                             InlineKeyboardButton.WithUrl("Добавить фильтр на Av.by (Test!)", "https://cars.av.by"),

                                        }

                                    });
                        SendMessage(message.Chat.Id.ToString(), $"Добавить фильтор.",
                       $"{message.Chat.FirstName}, чтобы добавить оповещение о новых объявлениях на Куфаре или Барахолке" +
                       $" в соответсии с выбранными критериями, поделись с ботом ссылкой на страницу поиска.  Ты также можешь" +
                       $" использовать строку ввода для поиска на всех ресурсах по #ключевому слову. ", "", irkm);
                    }
                    if (message.Text == "Удалить фильтр")
                    {
                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {
                            InlineKeyboardButton[] ikb = new[] { InlineKeyboardButton.WithCallbackData(file, $"#?{file}") };
                            buttonList.Add(ikb);


                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Удалить фильтр.",
                       $"{message.Chat.FirstName}, чтобы удалить поисковый запрос нажми одноименную кнопку. ", "", irkm);
                        //var ghhjkgh=  bot.GetChatAsync(message.Chat.Id);
                    }
                    if (message.Text == "Мои фильтры")
                    {

                        string[] fillters = GetEntryNames(message.Chat.Id); // путь к папке
                        List<string> myFiltres = new List<string>();

                        List<InlineKeyboardButton[]> buttonList = new List<InlineKeyboardButton[]>();
                        foreach (string file in fillters)
                        {

                            InlineKeyboardButton[] ikb = new[]
                            {
                                            InlineKeyboardButton.WithCallbackData
                                                        (file,
                                                        "1"),
                                           InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("переименовать",$"/rename \n {file} => ")
                                        };
                            buttonList.Add(ikb);

                        }
                        var irkm = new InlineKeyboardMarkup(buttonList);
                        SendMessage(message.Chat.Id.ToString(), $"Вот они:",
                       "", "", irkm);

                    }
                    Console.WriteLine("Получено сообщение " + message.Text);


                    // await bot.SendTextMessageAsync(message.Chat.Id, "Привет я твой бот" + message.Chat.Username);


                }
            }
        }*/

        static void SendMessage(string id, string title, string message, string refer, InlineKeyboardMarkup irkm)
        {
            lock (obj)
            {
                // const string TOKEN = "1123919290:AAFtjId-l9-_9uBepk9jLnWJD4m9Ms0qROU";
                // TelegramBotClient bot = new TelegramBotClient(TOKEN);
                // _ = bot.SendTextMessageAsync(id, message);
                Telegram.Bot.Types.Message sendetMessage;
                var reply = $"<b>{title}</b>\n"
             + $"<code>{message}</code>"
             + $"{refer}\n";

                // if (irkm == null)
                if (title == "Привет!")
                {
                    //
                    // sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html).Result;
                    //
                    ReplyMarkupBase rkm;
                    rkm = new ReplyKeyboardMarkup(new[]
                    {
                                new[] {new KeyboardButton("Добавить фильтр") },
                                new[] {new KeyboardButton("Удалить фильтр") },
                                new[] {new KeyboardButton("Мои фильтры") },
                                new[] {new KeyboardButton("Проверить сейчас") }
                    });
                    sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html, replyMarkup: rkm).Result;
                    // sendetMessage = bot..SendTextMessageAsync(id," ", replyMarkup: rkm).Result;
                }
                else
                {
                    sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html, replyMarkup: irkm).Result;
                }
                //= Program.bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html);

                if (title != "Привет!")
                {
                    var msg = new EntityFrameworkWithXamarin.Core.Message();
                    msg.ChatId = sendetMessage.Chat.Id;
                    msg.MessageId = sendetMessage.MessageId;
                    AddBotMessages(msg);
                }
            }
        }

        static async void SendMessageAsync(string id, string title, string message, string refer, InlineKeyboardMarkup irkm)
        {
                // const string TOKEN = "1123919290:AAFtjId-l9-_9uBepk9jLnWJD4m9Ms0qROU";
                // TelegramBotClient bot = new TelegramBotClient(TOKEN);
                // _ = bot.SendTextMessageAsync(id, message);
                Telegram.Bot.Types.Message sendetMessage;
                var reply = $"<b>{title}</b>\n"
             + $"<code>{message}</code>"
             + $"{refer}\n";

                // if (irkm == null)
                if (title == "Привет!")
                {
                    //
                    // sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html).Result;
                    //
                    ReplyMarkupBase rkm;
                    rkm = new ReplyKeyboardMarkup(new[]
                    {
                                new[] {new KeyboardButton("Добавить фильтр") },
                                new[] {new KeyboardButton("Удалить фильтр") },
                                new[] {new KeyboardButton("Мои фильтры") }
                    });
                    sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html, replyMarkup: rkm).Result;
                    // sendetMessage = bot..SendTextMessageAsync(id," ", replyMarkup: rkm).Result;
                }
                else
                {
                    sendetMessage = bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html, replyMarkup: irkm).Result;
                }
                //= Program.bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html);

                if (title != "Привет!")
                {
                    var msg = new EntityFrameworkWithXamarin.Core.Message();
                    msg.ChatId = sendetMessage.Chat.Id;
                    msg.MessageId = sendetMessage.MessageId;
                    AddBotMessages(msg);
                }
            
        }
        static void AddBotMessages(EntityFrameworkWithXamarin.Core.Message msg)
        {
            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _ = db.Database.MigrateAsync();
                    db.Messages.Add(msg);
                    db.SaveChanges();
                }
            }

        }
        static void RenameEntryTbItem(long id, string tbName, string newTbName)
        {
            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _ = db.Database.MigrateAsync();

                    var reguests = db.Reguests.Where(x => x.UserId == id).ToList();
                    //var user = db.Users.Include(x=> x.Reguests).FirstOrDefault(x => x.Id == id);// 
                    if (reguests.Count != 0)
                    {
                        foreach (var rgst in reguests)
                        {
                            if (rgst.Name == tbName)
                            {
                                rgst.Name = newTbName;
                                db.SaveChanges();
                                return;
                            }

                        }
                    }

                }
            }

        }
        //
        static void AddEntry(long id, string name, string Rgst)
        {
            if (name.Length > 25) //Усекаем длинну имени, т.к. размер callback_data кнопки клавиатуры не может быть длиннее 64 бит
                name = name.Substring(0, 25);

            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _ = db.Database.MigrateAsync();
                    if (db.Users.Count() == 0)
                    {
                        User newuser = new User { Id = Convert.ToInt32(id) };
                        // newuser.Reguests.Add(new Reguest { Name = reguest, RGSt = reguest });
                        db.Users.Add(newuser);
                        db.Reguests.Add(new Reguest { User = newuser, Name = name, RGSt = Rgst });
                        // db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[User] ON");
                        db.SaveChanges();
                        // db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees OFF");
                    }
                    else
                    {
                        var users = db.Users.ToList<User>();
                        var user = db.Users.FirstOrDefault(x => x.Id == id);
                        if (user == null)
                        {
                            user = new User { Id = Convert.ToInt32(id) };
                        }
                        db.Reguests.Add(new Reguest { User = user, Name = name, RGSt = Rgst });
                        // db.Users.Add(user);
                        db.SaveChanges();
                    }
                }
            }
        }
        static void DellEntry(long id, string name)
        {
            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _ = db.Database.MigrateAsync();

                    // var users = db.Users.ToList();
                    var reguests = db.Reguests.Where(x => x.UserId == id).ToList();
                    //var user = db.Users.Include(x=> x.Reguests).FirstOrDefault(x => x.Id == id);// 
                    if (reguests.Count != 0)
                    {
                        foreach (var rgst in reguests)
                        {
                            if (rgst.Name == name)
                            {
                                db.Reguests.Remove(rgst);
                                db.SaveChanges();
                                return;
                            }

                        }
                    }

                }
                // выводим данные после обновления
                Console.WriteLine("\nДанные после удаления:");


            }
        }
        static string[] GetEntryNames(long id)
        {
            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _ = db.Database.MigrateAsync();
                    var reguests = db.Reguests.Where(x => x.UserId == id).ToList().Select(x => x.Name).ToArray<string>();
                    return reguests;
                }
            }

        }
      
    }


}