using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Nancy.Helpers;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EntityFrameworkWithXamarin.Droid
{
  [Service]
  public  class ForegraundLogger : Service
    {
        private string TAG;
        private bool isStarted;
        static string dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        static string fileName = "Users.db";
        public static string dbFullPath = Path.Combine(dbFolder, fileName);

        public override void OnCreate()
        {
            base.OnCreate();
            scheduleрHandler();
           // StartAsJob();
            Log.Info(TAG, "OnCreateForgraundLogger: the service is initializing.");
           
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {


            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                    StartAsJob();
                    return StartCommandResult.Sticky; 
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
            Log.Info(TAG, "OnDestroyLogger: The started service is shutting down.");
            
            isStarted = false;
            base.OnDestroy();
        }

        void RegisterForegroundService()
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
                .SetContentText(Resources.GetString(Resource.String.notification_text_logger))
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_circle)
                //.SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();
                // Enlist this instance of the service as a foreground service
                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID + 1, notification);
            }
            else
            {
                var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_circle)
                 //.SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();
                // Enlist this instance of the service as a foreground service
                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID + 1, notification);
            }
        }
        //////////////
        ///
        static object obj = new object();
        static bool enabled = true;

        static HtmlWeb htmlWeb = new HtmlWeb();

        static string ds = "desiareliz";
        static Int16 kfr = 1;
        static Int16 onlnr = 2;
        static Int16 av = 3;
        static Int16 Vek21 = 4;
        static Int16[] setes = { 1, 2, 3, 4 };
        static List<string> pagesTables = new List<string>();
        static List<TableItem> tbItems = new List<TableItem>();
        static string title;
        static bool sendTmMessage = false;
        static Action runnable;
        static Handler handler;
        private PowerManager.WakeLock wakeLock = null;

        private  void scheduleрHandler()
        {
            PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
            wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, this.PackageName);
            wakeLock.Acquire();

            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {
                // Thread loggerThread = new Thread(new ThreadStart(Logger.StartAsJob));
                // loggerThread.Start();
                Task.Run(() =>
                {
                    StartAsJob();
                    // Thread loggerThrea
                    handler.PostDelayed(runnable, 10 * 60 * 1000);
                });

            });
            handler.Post(runnable);
        }

        public static void StartAsJob()
        {
           // SendMessage("115243113", "Парсер запущен", DateTime.Now.ToString(), "");
            var fillters = GetEntryReguests();
            foreach (var file in fillters)
            {

                sendTmMessage = false;

                tbItems.Clear();
                RecordEntry(ds, file);

                if (file.TdItems.Count() > 1)
                {
                    sendTmMessage = true;
                }

                // var serchText = file.RGSt;
                var id = file.UserId;

                // if (tbItems[0].Url != null)
                //   Parser(new Uri(tbItems[0].Url), id);
                //  if (tbItems[0].A != null)
                //      Parser(serchText, id.ToString());

                if (file.RGSt.Contains("https:"))
                    Parser(new Uri(file.RGSt), id.ToString());
                else
                    Parser(file.Name, id.ToString());

                RecordEntry("s", file);
                Console.WriteLine(" ");
            }
            tbItems.Clear();
            ForegraundService.isStarted = true;

            DellMessage();
            SendMessage("115243113", "Последняя проверка: ", DateTime.Now.ToString(),"", "");

            // Thread.Sleep(500000); // пауза, мы ж не хотим перегревать процессор?

            //  DellMessage();

            //  Thread.Sleep(5000);

            //}
            /* catch (Exception ex)
             {
                 if (ex.Source == "System.Net.Requests")
                 {
                     Console.WriteLine("Проблемы с соединением");
                     Stop();
                     //  Console.WriteLine("Перезапускаем программу из Program");
                     //  Process.Start(@"C:\Users\Mi PC\source\repos\kufar\ConsoleParserKufar\ConsoleParserKufar\bin\Debug\netcoreapp3.0\ConsoleParserKufar2.exe");

                 }
                 else
                 {
                     Console.WriteLine("Иные проблемы /n" + ex.ToString());
                     Stop();
                     //  Console.WriteLine("Перезапускаем программу из Program");
                     //  Process.Start(@"C:\Users\Mi PC\source\repos\kufar\ConsoleParserKufar\ConsoleParserKufar\bin\Debug\netcoreapp3.0\ConsoleParserKufar2.exe");

                 }
             }*/
        }
        public static void Start()
        {
            //try
            // {
            // XmlSerializer formatter = new XmlSerializer(typeof(List<TableItem>));
            while (enabled)
            {

                var fillters = GetEntryReguests();
                foreach (var file in fillters)
                {
                    sendTmMessage = false;

                    tbItems.Clear();
                    RecordEntry(ds, file);

                    if (file.TdItems.Count() > 1)
                    {
                        sendTmMessage = true;
                    }

                    // var serchText = file.RGSt;
                    var id = file.UserId;

                    // if (tbItems[0].Url != null)
                    //   Parser(new Uri(tbItems[0].Url), id);
                    //  if (tbItems[0].A != null)
                    //      Parser(serchText, id.ToString());

                    if (file.RGSt.Contains("https:"))
                        Parser(new Uri(file.RGSt), id.ToString());
                    else
                        Parser(file.Name, id.ToString());

                    RecordEntry("s", file);
                    Console.WriteLine(" ");
                }
                tbItems.Clear();
                ForegraundService.isStarted = true;

                Thread.Sleep(5000); // пауза, мы ж не хотим перегревать процессор?
                DellMessage();
                //  Thread.Sleep(5000);

            }
            //}
            /* catch (Exception ex)
             {
                 if (ex.Source == "System.Net.Requests")
                 {
                     Console.WriteLine("Проблемы с соединением");
                     Stop();
                     //  Console.WriteLine("Перезапускаем программу из Program");
                     //  Process.Start(@"C:\Users\Mi PC\source\repos\kufar\ConsoleParserKufar\ConsoleParserKufar\bin\Debug\netcoreapp3.0\ConsoleParserKufar2.exe");

                 }
                 else
                 {
                     Console.WriteLine("Иные проблемы /n" + ex.ToString());
                     Stop();
                     //  Console.WriteLine("Перезапускаем программу из Program");
                     //  Process.Start(@"C:\Users\Mi PC\source\repos\kufar\ConsoleParserKufar\ConsoleParserKufar\bin\Debug\netcoreapp3.0\ConsoleParserKufar2.exe");

                 }
             }*/
        }
        public static void Stop()
        {
            Console.WriteLine("выполнение парсинга приостановлено IsStarted == false");
            ForegraundService.isStarted = false;
            enabled = false;
        }
        public static void Parser(string serchText, string id)
        {
            try
            {
                int scet = 2;//setes.Count(); //количество источников

                foreach (int sete in new[] { 1, 2 }) //in setes
                {
                    HtmlDocument document = null;
                    string className = "";
                    string url = "";
                    string curcUri = "";
                    int lastPages = 0;

                    if (sete == kfr)
                    {
                        Thread.Sleep(2450);
                        url = $"https://www.kufar.by/listings?query={serchText}&ot=1&rgn=&ar=";
                        document = htmlWeb.Load(url);

                        if (document.DocumentNode.SelectNodes(".//div[contains(@data-name,'listings-pagination')]")[0].InnerHtml != "")
                        {
                            className = document.DocumentNode.SelectNodes(".//div[contains(@data-name,'listings-pagination')]")[0].ChildNodes[0].ChildNodes[0].Attributes["class"].Value;
                            curcUri = "https://www.kufar.by";
                        }
                    }
                    if (sete == onlnr)
                    {
                        url = $"https://baraholka.onliner.by/search.php?q={serchText}&cat=1&by=created&start=0";
                        document = htmlWeb.Load(url);
                        className = "pages-fastnav";
                        curcUri = "https://baraholka.onliner.by";
                    }
                    if (className != "")
                        lastPages = ParserPageCounn(ref document, className);
                    if (lastPages == 0)
                    {
                        scet = scet - 1;
                        Console.WriteLine($"Нет результатов поиска {sete}, попробуйте другой запрос");
                        continue;
                    }

                    string nexturl = url.Replace(curcUri, "");
                    if (lastPages > 50)
                        lastPages = 20;
                    for (int i = 1; i <= lastPages; i++)
                    {
                        Console.WriteLine(i.ToString());
                        if (nexturl != "")
                            nexturl = ParserPageTable(curcUri + nexturl, id, className, sete).Result;
                    }

                }
                if (scet == 0) // проверяем сколько источников не дали результата
                {
                    Console.WriteLine($"Нет результатов поиска по всем источникам, попробуйте другой запрос");
                    SendMessage(id, $"Сожалеем!", $"По запросу /{serchText}/ источники не дали результатов. Удали введенный фильтр и попробуй другой запрос.","", ""); ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("parser string: " + ex.Message.ToString());
            }
        }
        public static void Parser(Uri url, string id)
        {
            // try
            // {
            int sete = 0;
            int scet = 2;// setes.Count(); //количество источников
            if (url.ToString().Contains("kufar.by"))
                sete = kfr;
            if (url.ToString().Contains("onliner.by"))
                sete = onlnr;
            if (url.ToString().Contains("av.by"))
                sete = av;
            if (url.ToString().Contains("21vek.by"))
                sete = Vek21;

            HtmlDocument document = null;
            string className = "";
            //string url = "";
            string curcUri = "";
            int lastPages = 0;

            if (sete == kfr)
            {
                document = htmlWeb.Load(url.ToString());
                var nnameCount = document.DocumentNode.SelectNodes(".//span") //!
                .Where(el => el.InnerText == "1").Count();
                if (nnameCount != 0)
                {
                    className = document.DocumentNode.SelectNodes(".//span")
                                .Where(el => el.InnerText == "1")
                                .Select(el => el.ParentNode.Attributes["class"].Value).ToList().Last();
                    curcUri = url.ToString().Split("kufar.by")[0] + "kufar.by";
                    // curcUri = "https://www.kufar.by";
                }
                else
                {
                    if (url.ToString().Contains(".kufar.by/user/"))
                        ParseKufarFrontEnd(url, id);
                    return;

                }
            }

            if (url.ToString().Contains("https://ab.onliner.by"))
            {
                SendMessage(id, $"Сожалеем!", $"Данный раздел не поддерживатеся {url.ToString()}./n Удали введенный фильтр и попробуй другой запрос.","", ""); ;
                return;
            }
            if (url.ToString().Contains("https://r.onliner.by/"))
            {
                SendMessage(id, $"Сожалеем!", $"Данный раздел не поддерживатеся {url.ToString()}./n Удали введенный фильтр и попробуй другой запрос.","", ""); ;
                return;
            }
            
            if (url.ToString().Contains("https://s.onliner.by/"))
            {

                ParseOnlierFrontEnd(url, id);

                //  SendMessage(id, $"Сожалеем!", $"Данный раздел не поддерживатеся {url.ToString()}./n Удали введенный фильтр и попробуй другой запрос.", ""); ;
                return;
            }
            if (sete == Vek21)
            {
                Parse21VekFrontEnd(url,id);
                return;
            }

            if (sete == onlnr)
            {
                document = htmlWeb.Load(url);
                curcUri = url.ToString().Split("onliner.by")[0] + "onliner.by";
                className = "pages-fastnav";
                //  curcUri = "https://baraholka.onliner.by";
            }
            if (sete == av)
            {
                document = htmlWeb.Load(url);
                curcUri = url.ToString().Split("av.by")[0] + "av.by";
                className = "listing__pages";

            }
            if (className != "")
                lastPages = ParserPageCounn(ref document, className);
            if (lastPages == 0)
            {
                scet = scet - 1;
                Console.WriteLine($"Нет результатов поиска {sete}, попробуйте другой запрос");
                Console.WriteLine($"Нет результатов поиска по ссылке, попробуйте другой запрос");
                SendMessage(id, $"Сожалеем!", $"По ссылке/n {url.ToString()}/n поиск не дал результатов. Удали введенный фильтр и попробуй другой запрос.", "",""); ;
                return;
            }

            string nexturl = url.ToString().Replace(curcUri, "");
            if (lastPages > 50)
                lastPages = 20;
            for (int i = 1; i <= lastPages; i++)
            {
                Console.WriteLine(i.ToString());
                if (nexturl != "")
                {
                    // if (nexturl.Contains(curcUri))
                    // {
                    //     nexturl = ParserPageTable(nexturl, id, className, sete);
                    //     continue;
                    // }
                    nexturl = ParserPageTable(curcUri + nexturl, id, className, sete).Result;
                    string oldNexturl = null;
                    if (nexturl != null)
                        oldNexturl = nexturl.Replace(curcUri, "");
                    nexturl = oldNexturl;
                }
            }

            // }
            // catch (Exception ex)
            // {
            //  Console.WriteLine("parser uri: " + ex.ToString());

            // }
        }
        public static int ParserPageCounn(ref HtmlDocument document, string className)
        {
            try
            {
                var jhfjg = document.DocumentNode.InnerHtml;
                if (document.DocumentNode.SelectNodes($"//*[@class='{className}']") != null)
                {

                    if (className == "listing__pages")//для ав
                    {
                        var nodeAv = document.DocumentNode.SelectNodes($"//*[@class='{className}']").Last();
                        var nodesAv = nodeAv.ChildNodes;
                        var itemCount = nodesAv["div"].ChildNodes.Last().InnerText.Split(" ");
                        // int result = int.Parse(itemCount.Substring(itemCount.IndexOf(" ")));
                        var intMatch = itemCount.Where(x => int.TryParse(x, out int jjhj)).ToList();

                        if (int.Parse(intMatch[1]) % int.Parse(intMatch[0]) != 0)
                            return int.Parse(intMatch[1]) / int.Parse(intMatch[0]) + 1;
                        else
                            return int.Parse(intMatch[1]) / int.Parse(intMatch[0]);

                        // return int.Parse( nodesAv["div"].ChildNodes.Where(el => el.InnerHtml.Contains("href")).First().ChildNodes.First().Attributes["href"].Value.Split("page=")[1]);//для авбай
                    }
                    var node = document.DocumentNode.SelectNodes($"//*[@class='{className}']").First();
                    var nodes = node.ChildNodes;
                    nodes.Last().Remove();
                    //nodes.First().Remove();
                    return int.Parse(nodes.Last().InnerText);//для онл и куфар
                }
                else
                {
                    if (className == "pages-fastnav")//onliner
                    {
                        var tableItemsCount = document.DocumentNode
                            .SelectNodes("//tr")
                            .Where(el => el.InnerHtml.Contains("/h2"))
                            .Where(el => el.InnerHtml.Contains("cost"))
                            .Where(el => el.InnerHtml.Contains("frst ph colspan"))
                            .ToList().Count;
                        if (tableItemsCount > 0)
                            return 1;
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (className == "pages")
                    return 1; //av.by
                return 0;//onl, kufr
            }

        }
        public static string ParserNextPage(ref HtmlDocument document, string className)
        {
            try
            {
                if (document.DocumentNode.SelectNodes($"//*[@class='{className}']") != null)
                {
                    if (className == "listing__pages")//для ав
                    {
                        var nodeAv = document.DocumentNode.SelectNodes($"//*[@class='{className}']").Last();
                        var nodesAv = nodeAv.ChildNodes;
                        var nextPageAv = nodesAv["div"].ChildNodes.Where(el => el.InnerHtml.Contains("href"));
                        if (nextPageAv.Count() == 0)
                            return null;
                        return nextPageAv.First().ChildNodes.First().Attributes["href"].Value.Replace(";", "&"); //для авбай
                    }
                    var node = document.DocumentNode.SelectNodes($"//*[@class='{className}']").First();
                    var nodes = node.SelectNodes(".//a");
                    if (nodes == null)
                    {
                        return $"";
                    }

                    if (className == "listing__pages")//av.by
                    {
                        //var kjhkhj = document.DocumentNode.SelectNodes($"//*[@class='pages-arrows-link']").Where(el=>el.InnerText=="Ð¡Ð»ÐµÐ´ÑÑÑÐ°Ñ ÑÑÑÐ°Ð½Ð¸ÑÐ° â").First().Attributes["href"].Value.ToString();
                        return document.DocumentNode.SelectNodes($"//*[@class='pages-arrows-link']").Where(el => el.InnerText == "Ð¡Ð»ÐµÐ´ÑÑÑÐ°Ñ ÑÑÑÐ°Ð½Ð¸ÑÐ° â").First().Attributes["href"].Value.ToString();
                    }
                    return nodes.Last().Attributes["href"].Value.ToString().Replace(";", "&").Replace("&amp", "").Replace("./viewforum.php", "/viewforum.php");
                }
                else { return $""; }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }

        }
        public static async Task<string> ParserPageTable(string url, string id, string className, int sete)
        {
            Thread.Sleep(2450);
            var document = await htmlWeb.LoadFromWebAsync(url);
            var NodeTables = new List<HtmlNode>();

            if (sete == kfr)
            {

                NodeTables.AddRange(
                document.DocumentNode
                .SelectNodes(".//article")[0]
               //.SelectNodes(".//div[contains(@data-name,'listings')]")[0]
               .SelectNodes(".//a")

               .ToList());

            }
            if (sete == onlnr)
            {
                NodeTables.AddRange(
                document.DocumentNode
               .SelectNodes("//tr")
               .Where(el => el.InnerHtml.Contains("/h2"))
               .Where(el => el.InnerHtml.Contains("cost"))
               .Where(el => el.InnerHtml.Contains("frst ph colspan"))
               .ToList());
            }
            if (sete == av)
            {
                if (url.Contains("https://parts.av.by"))//запчасти
                {
                    var sdf = document.DocumentNode
                     .SelectNodes($"//div[contains(@class,  'tyre-listing-wrap')]").Last().ChildNodes;

                    NodeTables.AddRange(
                    document.DocumentNode
                   .SelectNodes($"//div[contains(@class,  'tyre-listing-wrap')]").Last().ChildNodes
                   .Where(el => el.InnerHtml.Contains("tyre-listing-main"))
                   .ToList());
                }
                else//все отстальное
                {
                    /* NodeTables.AddRange(
                     document.DocumentNode
                    .SelectNodes($"//div[contains(@class,  'listing-wrap')]").Last().ChildNodes
                    .Where(el => el.InnerHtml.Contains("listing-item-body"))
                    .ToList());
                                */
                    NodeTables.AddRange(
                     document.DocumentNode
                    .SelectNodes($"//div[contains(@class,  'listing__items')]").Last().ChildNodes
                    .Where(el => el.InnerHtml.Contains("listing-item"))
                    .ToList());
                }
            }
            int ммм = 0;
            foreach (HtmlNode htmlNode in NodeTables)
            {
                string refer = "";
                string cost = "";
                string txt = "";
                string location = "";

                if (sete == kfr)
                {
                    refer = htmlNode.Attributes["href"].Value;
                    //cost = htmlNode.SelectNodes(".//span")[1].InnerText;
                    var costElement = htmlNode.SelectNodes(".//span")
                    .Where(el => el.InnerText.Contains(" р."));
                    if (costElement.Count() != 0)
                    {
                        var costElementParent = costElement.First().ParentNode.ChildNodes;
                        foreach (HtmlNode cst in costElementParent)
                        {
                            cost = cost + cst.InnerText + ":";
                        }
                    }
                    else
                        cost = "";
                    if (url.Contains("https://re.kufar.by"))
                    {
                        var txtElement = htmlNode.SelectNodes(".//div");
                        if (txtElement != null)
                        {
                            var c = txtElement.GetNodeIndex(txtElement.Last()) - 1;
                            txt = txtElement[txtElement.GetNodeIndex(txtElement.Last()) - 1].InnerText + ", " + txtElement.Last().InnerText;
                        }

                        else
                            txt = "";
                    }
                    else
                    {
                        var tt = htmlNode.SelectNodes(".//h3");
                        if (tt == null)
                            txt = htmlNode.SelectNodes(".//span")[1].InnerText;
                        else
                            txt = htmlNode.SelectNodes(".//h3")[0].InnerText;
                        location = htmlNode.SelectNodes(".//span")[2].InnerText;
                    }
                    // cost = htmlNode.ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                    // txt = htmlNode.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText;
                }
                if (sete == onlnr)
                {
                    if (htmlNode.SelectNodes(".//div[contains(@class, 'price-primary')]") != null)
                    {
                        cost = htmlNode.SelectNodes(".//div[contains(@class, 'price-primary')]").Last().InnerText; ;//.ChildNodes.Select(el=>el.SelectNodes("//div[contains(@class, 'price-primary')]"));
                    }
                    refer = "https://baraholka.onliner.by" + htmlNode.SelectNodes(".//h2")
                       .Select(el => el.LastChild)
                       .Select(el => el.Attributes["href"].Value).Last().Replace("./viewforum.php", "/viewforum.php").Replace("./viewtopic.php", "/viewtopic.php");
                    txt = htmlNode.SelectNodes(".//h2").Last().InnerText;
                    location = htmlNode.SelectNodes(".//strong")[0].InnerText;

                }
                if (sete == av)
                {
                    if (url.Contains("https://cars.av.by"))
                    {
                        if (htmlNode.SelectNodes(".//div[contains(@class, 'listing-item__prices')]") != null)//listing-item-price
                        {
                            cost = htmlNode.SelectNodes(".//div[contains(@class, 'listing-item__prices')]").First().InnerText;//htmlNode.SelectNodes(".//strong").Last().InnerText.Replace("Ñ\u0080", "p. ") + htmlNode.SelectNodes(".//small").Last().InnerText;
                        }
                        var titleNode = htmlNode.SelectNodes(".//h3")[0]     ///h4
                            .SelectSingleNode(".//a");
                        refer = titleNode.Attributes["href"].Value;
                        txt = titleNode.InnerText.Replace("      ", " ").Replace("   ", " ").Replace("   ", " ").Replace("\n", "")
                            .Replace("(Ñ\u0080ÐµÑ\u0081Ñ\u0082Ð°Ð¹Ð»Ð¸Ð½Ð³)", "(рестайлинг)") + " " + htmlNode.SelectNodes(".//div[contains(@class, 'listing-item__params')]").Last().InnerText;
                        location = htmlNode.SelectNodes(".//div[contains(@class, 'listing-item__location')]").Last().InnerText;//htmlNode.SelectNodes(".//strong")[0].InnerText;
                    }
                    if (url.Contains("https://parts.av.by"))//запчасти c ав
                    {
                        cost = htmlNode.SelectNodes(".//h5").Last().InnerText.Split("Ñ\u0080")[0] + "p. ";
                        var titleNode = htmlNode.SelectNodes(".//h4")[0]
                        .SelectSingleNode(".//a");
                        refer = titleNode.Attributes["href"].Value;

                        System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("ISO-8859-1");
                        System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
                        var inputEncoding = Encoding.GetEncoding("iso-8859-1");

                        var text = inputEncoding.GetBytes(titleNode.InnerText + " " + htmlNode.SelectNodes(".//h4").Last().ParentNode.ChildNodes["p"].InnerText);
                        txt = Encoding.UTF8.GetString(text).Replace("    ", " ").Replace("    ", " ").Replace("   ", " ").Replace("   ", " ");

                        text = inputEncoding.GetBytes(htmlNode.SelectNodes(".//h5").Last().ParentNode.ChildNodes["p"].InnerText);
                        location = Encoding.UTF8.GetString(text);
                    }

                }

                var tbIt = new TableItem(refer, txt, cost, sete.ToString(), location);
                var oldItem = tbItems
                    .Where(x => x.A != null)
                    .ToList()
                    .Find(x => x.A.Contains(refer));

                if (oldItem == null)
                {
                    tbItems.Add(tbIt);
                    // tbItems.Insert(1, tbIt);
                    Console.WriteLine($"Новое объявление \n{ txt} { cost} { refer} ");
                    if (sendTmMessage)
                        SendMessage(id, title, $"Новое объявление: \n{ txt} { cost}",location, refer);
                }
                else
                {
                    if (oldItem.Cost != cost)
                    {
                        string[] subOldCost = oldItem.Cost.Split(":");
                        string[] subCost = cost.Split(":");
                        bool newPrice = true;
                        foreach (string str in subOldCost)
                        {
                            
                            if (str != "")
                            {
                                if (subCost.Contains(str))
                                {
                                    newPrice = false;
                                    break;
                                }
                            }
                           
                        }
                        if (newPrice)
                        {
                            Console.WriteLine($"Новая цена \n{ txt} { cost} <- {oldItem.Cost} { refer} ");
                            if (sendTmMessage)
                                SendMessage(id, title, $"Новая цена: \n{ txt} { cost} ({oldItem.Cost})",location, refer);
                            tbItems.First(x => x.Id == oldItem.Id).Cost = cost;
                            //tbItems.Insert(0, tbIt);
                        }


                    }
                    if (oldItem.Txt != txt)
                    {
                        Console.WriteLine($"Новое описание \n{ txt} { cost} {refer} ");
                        if (sendTmMessage)
                            SendMessage(id, title, $"Новое описание: \n{ txt} { cost}",location, refer);
                        tbItems.First(x => x.Id == oldItem.Id).Txt = txt;
                        // tbItems.Remove(oldItem);
                        // tbItems.Insert(1, tbIt);

                        //newItemConteint = true;
                    }

                }
            }
            return ParserNextPage(ref document, className);
        }
        
        public static void ParseOnlierFrontEnd(Uri baseadress, string id)
        {
            // tbItems.Clear();

            string refer = "";
            string cost = "";
            string txt = "";
            string location = "";

            var sstr = baseadress.ToString().Split("onliner.by/");
            string apiAdres = sstr[0] + "onliner.by/api/" + sstr[1];
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(baseadress.ToString());
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var json = client.GetStringAsync(apiAdres);
                // JObject j = JObject.Parse(json);
                // Console.WriteLine(j.ToString());
                var message = JsonConvert.DeserializeObject<OnlinerItems>(json.Result);


                if (message.Page.Last.ToString() != "")
                {
                    for (int l = 1; l <= message.Page.Last; l++)
                    {
                        Console.WriteLine(l.ToString());
                        // string curcApiAdres = apiAdres.Split("&page=")
                        string nextApiAdres = apiAdres.Replace("page=", "") + "&page=" + l.ToString(); //удоляем подстроку page= для корректного перехода не из перовой страницы
                        json = client.GetStringAsync(nextApiAdres);
                        message = JsonConvert.DeserializeObject<OnlinerItems>(json.Result);

                        for (int i = 0; i < message.Tasks.Count(); i++)
                        {
                            if (message.Tasks[i].HtmlUrl != null)
                                refer = message.Tasks[i].HtmlUrl.ToString();
                            if (message.Tasks[i].Price != null)
                                cost = message.Tasks[i].Price.Amount + " " + message.Tasks[i].Price.Currency;
                            if (message.Tasks[i].Description != null)
                                txt = message.Tasks[i].Description;
                            if (message.Tasks[i].Location != null)
                                location = message.Tasks[i].Location.FormattedLocality;

                            var tbIt = new TableItem(refer, txt, cost, location);
                            var oldItem = tbItems
                                .Where(x => x.A != null)
                                .ToList()
                                .Find(x => x.A.Contains(refer));

                            if (oldItem == null)
                            {
                                tbItems.Add(tbIt);
                                // tbItems.Insert(1, tbIt);
                                Console.WriteLine($"Новое объявление { txt} { cost} { refer} ");
                                if (sendTmMessage)
                                    SendMessage(id, title, $"Новое объявление: { txt} { cost}",location, refer);
                            }
                            else
                            {
                                if (oldItem.Cost != cost)
                                {
                                    string[] subOldCost = oldItem.Cost.Split(":");
                                    bool newPrice = true;
                                    foreach (string str in subOldCost)
                                    {
                                        if (str != "")
                                        {
                                            if (cost.Contains(str))
                                            {
                                                newPrice = false;
                                            }
                                        }
                                    }
                                    if (newPrice)
                                    {
                                        Console.WriteLine($"Новая цена  { txt} { cost} <- {oldItem.Cost} { refer} ");
                                        if (sendTmMessage)
                                            SendMessage(id, title, $"Новая цена: { txt} { cost} ({oldItem.Cost})",location, refer);
                                        // tbItems.Remove(oldItem);
                                        // tbItems.Insert(1, tbIt);
                                        tbItems.First(x => x.Id == oldItem.Id).Cost = cost;
                                    }


                                }
                                if (oldItem.Txt != txt)
                                {
                                    Console.WriteLine($"Новое описание  { txt} { cost} {refer} ");
                                    if (sendTmMessage)
                                        SendMessage(id, title, $"Новое описание: { txt} { cost}",location, refer);
                                    tbItems.First(x => x.Id == oldItem.Id).Txt = txt;
                                    //  tbItems.Remove(oldItem);
                                    //  tbItems.Insert(1, tbIt);

                                    //newItemConteint = true;
                                }

                            }
                        }
                    }
                }
                else
                {

                }

            }
        }
        public static void ParseKufarFrontEnd(Uri baseadress, string id)
        {
            // tbItems.Clear();

           // string fixedUri = baseadress.AbsoluteUri.Replace(baseadress.Query, string.Empty);

            string refer = "";
            string cost = "";
            string txt = "";
            string location = "";

            // var sstr = baseadress.ToString().Split("onliner.by/");
            string sstrBase = "https://cre-api.kufar.by/items-search/v1/engine/v1/search/rendered-paginated?size=30&";
            var sstr = baseadress.ToString().Split("?");
            var sstrName = sstr[0].Split("/").Last();
            string sstrParam = "";

            if (sstr.Length >= 2)
                sstrParam = sstr[1];

            if (sstrParam.Contains("cursor="))
            {
                var indexCursor1 = sstr[1].Split("cursor=");
                sstrParam = indexCursor1[0];// удалям из парамтров токам
            }
            string apiAdres = sstrBase + sstrParam + $"&atid={sstrName}&lang=ru";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(baseadress.ToString());
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var json = client.GetStringAsync(apiAdres);

                // JObject j = JObject.Parse(json);
                // Console.WriteLine(j.ToString());
                var message = JsonConvert.DeserializeObject<KufarItems>(json.Result);


                if (message.Pagination.Pages.Last().Num.ToString() != "")
                {
                    for (int l = 1; l <= message.Pagination.Pages.Last().Num; l++)
                    {

                        Console.WriteLine(l.ToString());
                        string nexOken;
                        if (l == 1)
                            nexOken = "";
                         
                        else
                        {
                            nexOken = message.Pagination.Pages.First(el => el.Label == "next").Token.ToString();
                            nexOken = $"&cursor={nexOken}";
                        }

                        string nextApiAdres = sstrBase + sstrParam + $"{nexOken}&atid={sstrName}&lang=ru"; //удоляем подстроку page= для корректного перехода не из перовой страницы
                        json = client.GetStringAsync(nextApiAdres);

                        message = JsonConvert.DeserializeObject<KufarItems>(json.Result);

                        for (int i = 0; i < message.Ads.Count(); i++)
                        {
                            if (message.Ads[i].AdLink != null)
                                refer = message.Ads[i].AdLink.ToString();
                            if (message.Ads[i].PriceByn != null)
                                cost = message.Ads[i].PriceByn / 100 + ".p : $" + message.Ads[i].PriceUsd / 100;
                            if (message.Ads[i].Subject != null)
                                txt = message.Ads[i].Subject;
                             if (message.Ads[i].AdParameters[1].Vl.String != null)
                                 location = message.Ads[i].AdParameters[1].Vl.String;

                            var tbIt = new TableItem(refer, txt, cost, location);
                            var oldItem = tbItems
                                .Where(x => x.A != null)
                                .ToList()
                                .Find(x => x.A.Contains(refer));

                            if (oldItem == null)
                            {
                                tbItems.Add(tbIt);
                                // tbItems.Insert(1, tbIt);
                                Console.WriteLine($"Новое объявление:\n{ txt} { cost} { refer} ");
                                if (sendTmMessage)
                                    SendMessage(id, title, $"Новое объявление:\n{ txt} { cost}",location, refer);
                            }
                            else
                            {
                                if (oldItem.Cost != cost)
                                {
                                    string[] subOldCost = oldItem.Cost.Split(":");
                                    bool newPrice = true;
                                    foreach (string str in subOldCost)
                                    {
                                        if (str != "")
                                        {
                                            if (cost.Contains(str))
                                            {
                                                newPrice = false;
                                            }
                                        }
                                    }
                                    if (newPrice)
                                    {
                                        Console.WriteLine($"Новая цена:\n{ txt} { cost} <- {oldItem.Cost} { refer} ");
                                        if (sendTmMessage)
                                            SendMessage(id, title, $"Новая цена:\n{ txt} { cost} ({oldItem.Cost})",location, refer);
                                        // tbItems.Remove(oldItem);
                                        // tbItems.Insert(1, tbIt);
                                        tbItems.First(x => x.Id == oldItem.Id).Cost = cost;
                                    }


                                }
                                if (oldItem.Txt != txt)
                                {
                                    Console.WriteLine($"Новое описание:\n{ txt} { cost} {refer} ");
                                    if (sendTmMessage)
                                        SendMessage(id, title, $"Новое описание:\n{ txt} { cost}",location, refer);
                                    tbItems.First(x => x.Id == oldItem.Id).Txt = txt;
                                    //  tbItems.Remove(oldItem);
                                    //  tbItems.Insert(1, tbIt);

                                    //newItemConteint = true;
                                }

                            }
                        }
                    }
                }
                else
                {

                }




            }
        }
        public static void Parse21VekFrontEnd(Uri baseadress, string id)
        {
             //tbItems.Clear();

          
            string sstrBase = "https://www.21vek.by/special_offers/sales.html";
            var sstr = baseadress.ToString().Split("#");
            
            string sstrName = "";
            if (sstr.Count()>1)
            sstrName = sstr.Last().Replace("all", "");
            
            string sstrParam = "";

          
            string apiAdres = sstrBase + sstrParam + $"#{sstrName}";
            

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
                var parameters = new Dictionary<string, string>();
                parameters["hash"] =$"#{sstrName}";
                parameters["link"] = "/special_offers/sales.html";
                var response = client.PostAsync(sstrBase, new FormUrlEncodedContent(parameters)).Result;
                
                var json = response.Content.ReadAsStringAsync();

                // JObject j = JObject.Parse(json);
                // Console.WriteLine(j.ToString());
                var message = JsonConvert.DeserializeObject<Vek21Items>(json.Result);
               // var message = JsonConvert.DeserializeObject<KufarItems>(json.Result);


                if (message.total_pages>=1)
                {
                    for (int l = 1; l <= message.total_pages; l++)
                    {
                        Console.WriteLine(l.ToString());

                        string nextApiAdres = $"https://www.21vek.by/special_offers/sales.html?page={l.ToString()}&hash={sstrName}"; //удоляем подстроку page= для корректного перехода не из перовой страницы
                        json = client.GetStringAsync(nextApiAdres);

                        message = JsonConvert.DeserializeObject<Vek21Items>(json.Result);

                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(message.content);

                        var NodeTables = new List<HtmlNode>();
                        var nodes = document.DocumentNode
                            .SelectNodes("//article")
                           ;
                        NodeTables.AddRange(
                                document.DocumentNode
                                .SelectNodes("//article")
                                .ToList());
                        foreach (HtmlNode htmlNode in NodeTables)
                        {
                            string refer = "";
                            string cost = "";
                            string txt = "";
                            string location = "";

                            var titleNode = htmlNode.SelectNodes(".//h3")[0]     ///h4
                            .SelectSingleNode(".//a");
                            refer = titleNode.Attributes["href"].Value;
                            txt = titleNode.InnerText;

                            var link = htmlNode.SelectNodes(".//span[@data-price]").LastOrDefault();
                            cost = link.Attributes["data-price"].Value;
                            
                          

                            var tbIt = new TableItem(refer, txt, cost, location);
                            var oldItem = tbItems
                                .Where(x => x.A != null)
                                .ToList()
                                .Find(x => x.A.Contains(refer));

                            if (oldItem == null)
                            {
                                tbItems.Add(tbIt);
                                // tbItems.Insert(1, tbIt);
                                Console.WriteLine($"Новое объявление:\n{ txt} { cost} { refer} ");
                                if (sendTmMessage)
                                    SendMessage(id, title, $"Новое объявление:\n{ txt} { cost}", location, refer);
                            }
                            else
                            {
                                if (oldItem.Cost != cost)
                                {
                                    string[] subOldCost = oldItem.Cost.Split(":");
                                    bool newPrice = true;
                                    foreach (string str in subOldCost)
                                    {
                                        if (str != "")
                                        {
                                            if (cost.Contains(str))
                                            {
                                                newPrice = false;
                                            }
                                        }
                                    }
                                    if (newPrice)
                                    {
                                        Console.WriteLine($"Новая цена:\n{ txt} { cost} <- {oldItem.Cost} { refer} ");
                                        if (sendTmMessage)
                                            SendMessage(id, title, $"Новая цена:\n{ txt} { cost} ({oldItem.Cost})", location, refer);
                                        // tbItems.Remove(oldItem);
                                        // tbItems.Insert(1, tbIt);
                                        tbItems.First(x => x.Id == oldItem.Id).Cost = cost;
                                    }


                                }
                                if (oldItem.Txt != txt)
                                {
                                    Console.WriteLine($"Новое описание:\n{ txt} { cost} {refer} ");
                                    if (sendTmMessage)
                                        SendMessage(id, title, $"Новое описание:\n{ txt} { cost}", location, refer);
                                    tbItems.First(x => x.Id == oldItem.Id).Txt = txt;
                                    //  tbItems.Remove(oldItem);
                                    //  tbItems.Insert(1, tbIt);

                                    //newItemConteint = true;
                                }

                            }

                        }
                    }
                }
                else
                {

                }

            }
        }

        static void SendMessage(string id, string title, string message, string location, string refer)
        {
            Telegram.Bot.Types.Message sendetMessage;
            lock (ForegraundService.obj)
            {
                // const string TOKEN = "1123919290:AAFtjId-l9-_9uBepk9jLnWJD4m9Ms0qROU";
                //  TelegramBotClient bot = new TelegramBotClient(TOKEN);
                // _ = bot.SendTextMessageAsync(id, message);

                var reply = $"<b>{title}</b>\n"
             + $"<code>{message}</code>\n"
             + $"{refer}\n"
             + $"{location}\n";
                sendetMessage = ForegraundService.bot.SendTextMessageAsync(id, reply, parseMode: ParseMode.Html).Result;
            }

            if (title == "Последняя проверка: ")
            {
                var msg = new EntityFrameworkWithXamarin.Core.Message();
                msg.ChatId = sendetMessage.Chat.Id;
                msg.MessageId = sendetMessage.MessageId;
                AddBotMessages(msg);
            }

        }
        public static void RecordEntry(string fileEvent, Reguest filePath)
        {
            lock (ForegraundService.obj)
            {
                // XmlSerializer formatter = new XmlSerializer(typeof(List<TableItem>));
                if (fileEvent == ds)
                {
                    tbItems = filePath.TdItems; //(List<TableItem>)formatter.Deserialize(writer);
                    Console.WriteLine($"Объект {filePath.Name} десериализован " + DateTime.Now.ToString());

                }
                if (fileEvent == "s")
                {
                    using (var db = new ApplicationContext(ForegraundService.dbFullPath))
                    {
                        _ = db.Database.MigrateAsync();


                        var tbit = db.Reguests.FirstOrDefault(x => x.Id == filePath.Id);
                        // var tt = db.TableItems.Where(x => x.ReguestId == filePath.Id).ToList();
                        // tt.Clear();
                        // tt = tbItems;
                        if (tbit != null)
                        {
                            tbit.TdItems = tbItems;
                        }
                        // db.Reguests.First(x => x.Id == filePath.Id).TdItems = tbItems;
                        db.SaveChanges();
                        Console.WriteLine($"Объект {filePath.Name} сериализован " + DateTime.Now.ToString());
                    }
                }
                if (fileEvent == "dell")
                {
                    /*  FileInfo fileInf = new FileInfo(filePath);
                      if (fileInf.Exists)
                      {
                          fileInf.Delete();

                      }
                    */
                }
                if (fileEvent == "rename")
                {
                    /* string[] str = filePath.Split("!");
                     string fileOld = str[0] + "!" + str[1] + ".xml";
                     string fileNew = str[0] + "!" + str[2];
                     FileInfo fileInf = new FileInfo(fileOld);
                     if (fileInf.Exists)
                     {
                         fileInf.MoveTo(fileNew);
                     }
                    */
                }
            }
        }
        static void DellMessage()
        {

            EntityFrameworkWithXamarin.Core.Message[] messagesFromBot;
            using (var db = new ApplicationContext(ForegraundService.dbFullPath))
            {
                _ = db.Database.MigrateAsync();

                messagesFromBot = db.Messages.ToArray();
                db.SaveChanges();
            }
            foreach (var message in messagesFromBot)
            {
                var adsasd = ForegraundService.bot.DeleteMessageAsync(message.ChatId, (int)message.MessageId);
            }

        }
        static List<Reguest> GetEntryReguests()
        {
            lock (ForegraundService.obj)
            {
                using (var db = new ApplicationContext(ForegraundService.dbFullPath))
                {
                    _ = db.Database.MigrateAsync();
                    List<Reguest> reguests = db.Reguests.Include(x => x.TdItems).ToList();
                    return reguests;
                }
            }

        }
        static List<EntityFrameworkWithXamarin.Core.Message> GetBotMessages()
        {
            lock (ForegraundService.obj)
            {
                using (var db = new ApplicationContext(ForegraundService.dbFullPath))
                {
                    _ = db.Database.MigrateAsync();
                    List<EntityFrameworkWithXamarin.Core.Message> messages = db.Messages.ToList();
                    return messages;
                }
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
    }
}