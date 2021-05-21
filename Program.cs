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

namespace TestTaskBot
{
    class Program
    {
        private static TelegramBotClient client;
        static async Task Main(string[] args)
        {
            var ex = ExchangeAPI.GetExchangeAPI<ExchangeBinanceAPI>();
            var rew = ExchangeAPI.GetExchangeAPI<ExchangeKrakenAPI>();
          var _ex_s=await  ex.GetMarketSymbolsAsync();
            var tmp_n = await ex.GetMarketSymbolsAsync();

            var sym = await ValidateMarketSymbolsAsync(ex, tmp_n.ToArray());


            var eee =await rew.GetMarketSymbolsAsync();
           var l= eee.ToList();
            List<string> list = new List<string>();
            for(int i=0;i<l.Count;i++)
            {
                var rs = rew.ExchangeMarketSymbolToGlobalMarketSymbolAsync(l[i]);
                
                list.Add(await rs);
            }
       var tmp_=     list.Distinct().ToList();

            List<string> names = new List<string>();
            for(int i =0;i<tmp_.Count;i++)
            {
                var str = tmp_[i].Split(new char[] { '-' })[0];
                names.Add(str);
            }
            var qqq = names.Distinct().ToList();
       List<IEnumerable<MarketCandle>> list1 = new List<IEnumerable<MarketCandle>>();
            for(int i=0;i<sym.Length;i++)
            {
                list1.Add(await rew.GetCandlesAsync(sym[i], 1800,
                //TODO: Add interfaces for start and end date
                CryptoUtility.UtcNow.AddDays(-12),
                CryptoUtility.UtcNow));
            }

            InlineKeyboardButton inlineKeyboardButton = new InlineKeyboardButton();
            inlineKeyboardButton.Text = "j";

            client = new TelegramBotClient("1899731038:AAF31rQVRhdUwMGIfHzuiiKwCRB9KG9K6cg");
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            
            Console.ReadLine();
            client.StopReceiving();
            Console.WriteLine("Hello World!");
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard =
         new KeyboardButton[][]
         {
        new KeyboardButton[]
        {
            new KeyboardButton("item"),
            new KeyboardButton("item")
        },
          new KeyboardButton[]
        {
            new KeyboardButton("item")
        }
         };
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(message.Chat.Id, message.Text, replyMarkup:rkm);
                

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
