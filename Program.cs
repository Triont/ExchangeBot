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


  
    public class NewExchangeAPI
    {
    //    IExchangeAPI Exchange;
        public static IExchangeAPI stExchange;

        public static string ExchangeName { get; set; }

        public IExchangeAPI API { get; private set; }
        public NewExchangeAPI(IExchangeAPI exchangeAPI)
        {
            this.API = exchangeAPI;
        }
        public void Save()
        {
            stExchange = API;
        }
       
        public static string GlobalSymbol { get; set; }
        public static string CandlesOrTrades { get; set; }
    }

    class Program
    {
  
        private static TelegramBotClient client;
        static void Main(string[] args)
        {




            client = new TelegramBotClient("1899731038:AAF31rQVRhdUwMGIfHzuiiKwCRB9KG9K6cg");

            client.OnMessage += BotOnMessageReceived;
        
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();

            Console.ReadLine();
            client.StopReceiving();

        }


        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {

            NewExchangeAPI newExchangeAPI = null;
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



                    newExchangeAPI = new NewExchangeAPI(ExchangeAPI.GetExchangeAPI(message.Text));
                    newExchangeAPI.Save();
                    var Symbols = await newExchangeAPI.API.GetMarketSymbolsAsync();
                    List<string> list = new List<string>();
                    for (int i = 0; i < Symbols.ToList().Count; i++)
                    {
                        var rs = await newExchangeAPI.API.ExchangeMarketSymbolToGlobalMarketSymbolAsync(Symbols.ToList()[i]);

                        list.Add(rs);

                    }
                    var r = list.ToArray();


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






                }


                else if (message.Text == "Cradles" || message.Text == "Trades")
                {
                    var recieve = messageEventArgs.Message;
                
                    NewExchangeAPI.CandlesOrTrades = message.Text;
                    NewExchangeAPI.CandlesOrTrades = recieve.Text;
                    newExchangeAPI = new NewExchangeAPI(NewExchangeAPI.stExchange);


                    if (NewExchangeAPI.GlobalSymbol != null)
                    {

                        if (NewExchangeAPI.CandlesOrTrades == "Cradles")
                        {
                            var _res = await newExchangeAPI?.API?.GetCandlesAsync(NewExchangeAPI.GlobalSymbol, 1800, CryptoUtility.UtcNow.AddDays(-12),
                                   CryptoUtility.UtcNow);
                            await client.SendTextMessageAsync(message.Chat.Id, $" {NewExchangeAPI.CandlesOrTrades}  {NewExchangeAPI.GlobalSymbol} {newExchangeAPI.API.Name}" +
                                              $"Low price {_res.Last().LowPrice} High price {_res.Last().HighPrice} Base Volume {_res.Last().BaseCurrencyVolume}" +
                                              $" Quote volume {_res.Last().QuoteCurrencyVolume} \n Enter /New to start new data pull");
                            await Task.Delay(5000);
                        }
                        else
                        {

                        
                            KeyValuePair<string, ExchangeTrade> keyValuePair = new KeyValuePair<string, ExchangeTrade>(NewExchangeAPI.GlobalSymbol, new ExchangeTrade());
                      
                            var tmp__ = await newExchangeAPI?.API?.GetTradesWebSocketAsync(i => Task.FromResult(keyValuePair));

                            var aw = await newExchangeAPI?.API?.GetRecentTradesAsync(NewExchangeAPI.GlobalSymbol);


                            await client.SendTextMessageAsync(message.Chat.Id, $"Price{aw.Last().Price}Amount {aw.Last().Amount} Was bought {aw.Last().IsBuy} {aw.Last().Timestamp}" +
                                $"\n Enter /New to start new data pull");
                            await Task.Delay(6000);

                        }
                    }


                }
                else if (messageEventArgs.Message.Text != "Kraken" && messageEventArgs.Message.Text != "Binance"
                    && messageEventArgs.Message.Text != "Cradles" && messageEventArgs.Message.Text != "Trades" && messageEventArgs.Message.Text != "/New")
                {



                    newExchangeAPI = new NewExchangeAPI(NewExchangeAPI.stExchange);

                    NewExchangeAPI.GlobalSymbol = message.Text;
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


                else if (messageEventArgs.Message.Text == "/New")
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

  

}
