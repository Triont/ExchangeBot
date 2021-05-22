using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using System.Linq;
using ExchangeSharp;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestTaskBot
{
    class Program
    {
        public static string Data_Bin_Kraken { get; set; }
        public static string GlobalSymbol { get; set; }
        public static string CandlesOrTrades { get; set; }
        private static TelegramBotClient client;
        static  void Main(string[] args)
        {
          
         

            client = new TelegramBotClient("1899731038:AAF31rQVRhdUwMGIfHzuiiKwCRB9KG9K6cg");

            client.OnMessage += BotOnMessageReceived;
           // client.OnInlineResultChosen += Client_OnInlineResultChosen;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();

            Console.ReadLine();
            client.StopReceiving();

        }

        //private static void Client_OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        //{

        //    e.ChosenInlineResult.ToString();


        //}

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard =
         new KeyboardButton[][]
         {
        new KeyboardButton[]
        {
            new KeyboardButton("Binance"),
            new KeyboardButton("Kraken")
        },

         };
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                if (message.Text == "Binance" || message.Text == "Kraken")
                {
                    switch (message.Text)
                    {
                        case "Binance":

                            Data_Bin_Kraken = "Binance";

                            var ds = ExchangeAPI.GetExchangeAPI<ExchangeBinanceAPI>();
                            var l = await ds.GetMarketSymbolsAsync();

                            List<string> list = new List<string>();
                            for (int i = 0; i < l.ToList().Count; i++)
                            {
                                var rs = await ds.ExchangeMarketSymbolToGlobalMarketSymbolAsync(l.ToList()[i]);

                                list.Add(rs);

                            }
                            var r = list.ToArray();
                            //var c=  list.Split(list.Count / 10).ToList();
                            //  var __arr = new List<string[]>();
                            //  List<string[]> vs1 = new List<string[]>();
                            //  for(int i=0; i<c.Count;i++)
                            //  {
                            //     vs1.Add(c[i].ToArray());
                            //  }
                            //  var qq = vs1.ToArray();
                            //  var lst_sym = l.ToList();
                            //  Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();

                            //  for(int i=0;i<list.Count();i++)
                            //  {
                            //      var __tmp = list[i].Split(new char[] { '-' });
                            //      if (__tmp.Length==2)
                            //      {

                            //          if (!keyValuePairs.ContainsKey(__tmp[0]))
                            //          {
                            //              keyValuePairs.Add(__tmp[0], new List<string>() { __tmp[1] });
                            //          }
                            //          else
                            //          {
                            //              keyValuePairs[__tmp[0]].Add(__tmp[1]);
                            //          }
                            //      }
                            // }

                            //  ClientWebSocket clientWebSocket = new ClientWebSocket();
                            //  clientWebSocket.Start();


                            //  var arr = l.ToArray();
                            //  KeyboardButton[] keyboardButtons = new KeyboardButton[arr.Length];
                            //  var reply = new ReplyKeyboardMarkup();

                            //  List<string[]> vs = new List<string[]>();
                            //  List<string> vs2 = new List<string>();
                            //  for(int i=0;i<keyValuePairs.Count;i++)
                            //  {
                            //      if(i%10==0)
                            //      {
                            //          vs.Add(vs2.ToArray());
                            //          vs2 = new List<string>();

                            //      }

                            //      vs2.Add(keyValuePairs.Keys.ElementAt(i));


                            //  }
                            //  var _arr = vs.ToArray();
                            //  List<KeyboardButton[]> buttons = new List<KeyboardButton[]>() { new KeyboardButton[11] };
                            //  for(int i=0; i<qq.Length;i++)
                            //  {
                            //      List<KeyboardButton> keyboards = new List<KeyboardButton>();
                            //      for(int j=0;j<qq[i].Length;j++)
                            //      {

                            //          keyboards.Add(new KeyboardButton(qq[i][j]));


                            //      }
                            //     buttons.Add( keyboards.ToArray());
                            //  }
                            var fd = new KeyboardButton[r.Length];
                            for (int i = 0; i < r.Length; i++)
                            {
                                fd[i] = new KeyboardButton(r[i]);
                            }

                            ReplyKeyboardMarkup reply = new ReplyKeyboardMarkup();
                            reply.Keyboard = new KeyboardButton[][]
                            {
                         fd
                            };

                            if (reply.Keyboard.ElementAt(0).ElementAt(0) != null)
                            {

                                await client.SendTextMessageAsync(message.Chat.Id, message.Text, replyMarkup: reply);
                                await Task.Delay(5000);
                            }



                            break;

                        case "Kraken":
                            Data_Bin_Kraken = "Kraken";

                            var kraken = ExchangeAPI.GetExchangeAPI<ExchangeKrakenAPI>();
                            var kr_symb = await kraken.GetMarketSymbolsAsync();

                            List<string> list_ = new List<string>();
                            for (int i = 0; i < kr_symb.ToList().Count; i++)
                            {
                                var rs = await kraken.ExchangeMarketSymbolToGlobalMarketSymbolAsync(kr_symb.ToList()[i]);

                                list_.Add(rs);

                            }
                            var jr = list_.ToArray();
                            //var c=  list.Split(list.Count / 10).ToList();
                            //  var __arr = new List<string[]>();
                            //  List<string[]> vs1 = new List<string[]>();
                            //  for(int i=0; i<c.Count;i++)
                            //  {
                            //     vs1.Add(c[i].ToArray());
                            //  }
                            //  var qq = vs1.ToArray();
                            //  var lst_sym = l.ToList();
                            //  Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();

                            //  for(int i=0;i<list.Count();i++)
                            //  {
                            //      var __tmp = list[i].Split(new char[] { '-' });
                            //      if (__tmp.Length==2)
                            //      {

                            //          if (!keyValuePairs.ContainsKey(__tmp[0]))
                            //          {
                            //              keyValuePairs.Add(__tmp[0], new List<string>() { __tmp[1] });
                            //          }
                            //          else
                            //          {
                            //              keyValuePairs[__tmp[0]].Add(__tmp[1]);
                            //          }
                            //      }
                            // }

                            //  ClientWebSocket clientWebSocket = new ClientWebSocket();
                            //  clientWebSocket.Start();


                            //  var arr = l.ToArray();
                            //  KeyboardButton[] keyboardButtons = new KeyboardButton[arr.Length];
                            //  var reply = new ReplyKeyboardMarkup();

                            //  List<string[]> vs = new List<string[]>();
                            //  List<string> vs2 = new List<string>();
                            //  for(int i=0;i<keyValuePairs.Count;i++)
                            //  {
                            //      if(i%10==0)
                            //      {
                            //          vs.Add(vs2.ToArray());
                            //          vs2 = new List<string>();

                            //      }

                            //      vs2.Add(keyValuePairs.Keys.ElementAt(i));


                            //  }
                            //  var _arr = vs.ToArray();
                            //  List<KeyboardButton[]> buttons = new List<KeyboardButton[]>() { new KeyboardButton[11] };
                            //  for(int i=0; i<qq.Length;i++)
                            //  {
                            //      List<KeyboardButton> keyboards = new List<KeyboardButton>();
                            //      for(int j=0;j<qq[i].Length;j++)
                            //      {

                            //          keyboards.Add(new KeyboardButton(qq[i][j]));


                            //      }
                            //     buttons.Add( keyboards.ToArray());
                            //  }
                            var _fd = new KeyboardButton[jr.Length];
                            for (int i = 0; i < jr.Length; i++)
                            {
                                _fd[i] = new KeyboardButton(jr[i]);
                            }

                            ReplyKeyboardMarkup _reply = new ReplyKeyboardMarkup();
                            _reply.Keyboard = new KeyboardButton[][]
                            {
                         _fd
                            };

                            if (_reply.Keyboard.ElementAt(0).ElementAt(0) != null)
                            {

                                await client.SendTextMessageAsync(message.Chat.Id, message.Text, replyMarkup: _reply);
                                await Task.Delay(5000);
                            }



                            break;












                    }
                }

                else if (message.Text == "Cradles" || message.Text == "Trades")
                {
                    var recieve = messageEventArgs.Message;
                    CandlesOrTrades = recieve.Text;
                    IExchangeAPI exchangeAPI;
                    switch (Data_Bin_Kraken)
                    {
                        case "Kraken":
                            exchangeAPI = ExchangeAPI.GetExchangeAPI<ExchangeKrakenAPI>();
                            switch (CandlesOrTrades)
                            {
                                case "Cradles":
                                    var res = await exchangeAPI.GetCandlesAsync(GlobalSymbol, 1800,

                   CryptoUtility.UtcNow.AddDays(-12),
                   CryptoUtility.UtcNow);


                                    await client.SendTextMessageAsync(message.Chat.Id, $"{CandlesOrTrades}  {GlobalSymbol} {Data_Bin_Kraken}" +
                                        $"Low price {res.Last().LowPrice} High price {res.Last().HighPrice} Base Volume {res.Last().BaseCurrencyVolume}" +
                                        $" Quote volume {res.Last().QuoteCurrencyVolume} \n Enter /New to start new data pull");
                                    await Task.Delay(5000);
                                    break;

                                case "Trades":
                                    ExchangeTrade exchangeTrade = new ExchangeTrade();


                                    //ClientWebSocket socket = new ClientWebSocket();
                                    

                                    var __res = await exchangeAPI.GetTradesWebSocketAsync(async (i) =>
                                    {

                                        await Task.FromResult(GlobalSymbol);
                                    });

                                    var aw = await exchangeAPI.GetRecentTradesAsync(GlobalSymbol);


                                    await client.SendTextMessageAsync(message.Chat.Id, $"Price{aw.Last().Price}Amount {aw.Last().Amount} Was bought {aw.Last().IsBuy} {aw.Last().Timestamp}" +
                                        $"\n Enter /New to start new data pull");
                                    await Task.Delay(5000);
                                    break;
                            }
                            break;


                        case "Binance":
                            exchangeAPI = ExchangeAPI.GetExchangeAPI<ExchangeBinanceAPI>();
                            switch (CandlesOrTrades)
                            {
                                case "Cradles":
                                    var res = await exchangeAPI.GetCandlesAsync(GlobalSymbol, 1800,

                   CryptoUtility.UtcNow.AddDays(-12),
                   CryptoUtility.UtcNow);


                                    await client.SendTextMessageAsync(message.Chat.Id, $"{CandlesOrTrades}  {GlobalSymbol} {Data_Bin_Kraken}" +
                                        $"Low price {res.Last().LowPrice} High price {res.Last().HighPrice} Base Volume {res.Last().BaseCurrencyVolume}" +
                                        $" Quote volume {res.Last().QuoteCurrencyVolume}\n Enter /New to start new data pull");
                                    await Task.Delay(5000);
                                    break;

                                case "Trades":
                                    ExchangeTrade exchangeTrade = new ExchangeTrade();


                                    ClientWebSocket socket = new ClientWebSocket();

                                    var __res = await exchangeAPI.GetTradesWebSocketAsync(async (i) =>
                                    {

                                        await Task.FromResult(GlobalSymbol);
                                    });
                                    IEnumerable<ExchangeTrade> aw = null;
                                    try
                                    {

                                         aw = await exchangeAPI.GetRecentTradesAsync(GlobalSymbol);
                                    }catch(APIException ap)
                                    {
                                        Console.WriteLine($"{ap.Data} {ap.Message}");
                                    }


                                    await client.SendTextMessageAsync(message.Chat.Id, $"Price{aw.Last().Price}Amount {aw.Last().Amount} Was bought {aw.Last().IsBuy} {aw.Last().Timestamp}\n Enter /New to start new data pull");
                                    
                                    await Task.Delay(5000);
                                    break;
                            }
                            break;
                    }

                }
                else if (messageEventArgs.Message.Text != "Kraken" && messageEventArgs.Message.Text != "Binance"
                    && messageEventArgs.Message.Text != "Cradles" && messageEventArgs.Message.Text != "Trades" && messageEventArgs.Message.Text!="/New")
                {
                    

                    GlobalSymbol = messageEventArgs.Message.Text;

                    ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup();
                    replyKeyboardMarkup.Keyboard = new KeyboardButton[][]
                    {
                            new KeyboardButton[]
                            {
                                "Cradles", "Trades"
                            }
                    };
                    await client.SendTextMessageAsync(message.Chat.Id, "Select cradle/trade", replyMarkup: replyKeyboardMarkup);
                    await Task.Delay(5000);

                }


                else if(messageEventArgs.Message.Text=="/New")
                {
                    await Task.Delay(5000);
                    await client.SendTextMessageAsync(message.Chat.Id, "\n New", replyMarkup: rkm);
                    await Task.Delay(10000);
                }

            }
        }
      

        protected static async Task<string[]> ValidateMarketSymbolsAsync(IExchangeAPI api, string[] marketSymbols, bool isWebSocket = false)
        {
            var apiSymbols = (await api.GetMarketSymbolsAsync(isWebSocket)).ToArray();

            if (marketSymbols is null || marketSymbols.Length == 0)
            {
                return apiSymbols;
            }

            return ValidateMarketSymbolsInternal(api, marketSymbols, apiSymbols)
                .ToArray();
        }

        private static IEnumerable<string> ValidateMarketSymbolsInternal(
            IExchangeAPI api,
            string[] marketSymbols,
            string[] apiSymbols
        )
        {
            foreach (var marketSymbol in marketSymbols)
            {
                var apiSymbol = apiSymbols.FirstOrDefault(
                    a => a.Equals(marketSymbol, StringComparison.OrdinalIgnoreCase)
                );

                if (!string.IsNullOrWhiteSpace(apiSymbol))
                {
                    //return this for proper casing
                    yield return apiSymbol;
                    continue;
                }

                var validSymbols = string.Join(",", apiSymbols.OrderBy(s => s));

                throw new ArgumentException(
                    $"Symbol {marketSymbol} does not exist in API {api.Name}.\n"
                    + $"Valid symbols: {validSymbols}"
                );
            }
        }

        
    }

    static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            return list.Select((item, index) => new { index, item })
                       .GroupBy(x => x.index % parts)
                       .Select(x => x.Select(y => y.item));
        }
    }
}
