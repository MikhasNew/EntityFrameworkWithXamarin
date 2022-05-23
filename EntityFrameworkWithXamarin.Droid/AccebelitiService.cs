using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;

using Xamarin.Essentials;



using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Android.Media;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using Android.Util;
using Android.Text;
using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
//using Nancy.Helpers;
using System.Xml.Serialization;
using EntityFrameworkWithXamarin.Core;
using Telegram.Bot;
using Nancy.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkWithXamarin.Droid
{
   // [Service(Label = "MyAccessibilityService", Permission = Manifest.Permission.BindAccessibilityService)]
   // [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
   // [MetaData("android.accessibilityservice.AccessibilityService", Resource = "@xml/accessibility_service_config")]
    /*
     class AccebelitiService : AccessibilityService
    {
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


        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            Main();

        }
        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {


        }


        public override void OnInterrupt()
        {
            // throw new NotImplementedException();
        }

        static void Main()
        {
            Console.WriteLine("Сервис запущен!");
            /// var logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(Logger.Start));
            loggerThread.Start();
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
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
               }
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
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
                                    var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    if (mess[0] == "")
                                        name = $"{url[1].Replace("&ot", "")}";
                                    else
                                        name = $"{mess[0].Split(' ')[0]} {mess[0].Split(' ')[1].Trim(',')}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    // RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("onliner.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split('=');
                                    var name = $"{url[1].Replace("&f", "")}";
                                    AddEntry(message.Chat.Id, name, $"https://{mess[1]}");
                                    //RecordEntry("great", $"./Fillters/{message.Chat.Id}!{name}.xml");
                                }
                                if (mess[1].Contains("av.by"))
                                {
                                    var url = HttpUtility.UrlDecode(mess[1]).Split(".by/");
                                    var name = $"{url[1].Replace("&f", "").Replace("&", " ").Replace("/", " ").Replace("?", "").Replace("[0]", "").Split(" ")[0]}";
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

        }

        static void AddBotMessages(EntityFrameworkWithXamarin.Core.Message msg)
        {
            lock (obj)
            {
                using (var db = new ApplicationContext(dbFullPath))
                {
                    _= db.Database.MigrateAsync();
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
        static void AddEntry(long id, string reguest, string Rgst)
        {
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
                        db.Reguests.Add(new Reguest { User = newuser, Name = reguest, RGSt = Rgst });
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
                        db.Reguests.Add(new Reguest { User = user, Name = reguest, RGSt = Rgst });
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
    */
}