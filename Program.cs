using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using DotEnv.Core;


namespace TelegramBotExperiments
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient(Environment.GetEnvironmentVariable("BOT_TOKEN"));

        static Dictionary<string, Command> commands = new Dictionary<string, Command>()
        {
            {"/start", new StartCommand() },
            {"/help", new HelpCommand() },
            {"/settings", new SettingsCommand() }
        };

        interface Command
        {
            Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        }

        class StartCommand : Command
        {
            public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Добро пожаловать на борт, добрый путник!");
            }
        }

        class HelpCommand : Command
        {
            public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Список команд: \n/start - начать работу с ботом \n/help - получить список команд \n/settings - настройки");
            }
        }

        class SettingsCommand : Command
        {
            public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Настройки");
            }
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            if (update.Type == UpdateType.Message)
            {
                if (commands.ContainsKey(update.Message.Text))
                {
                    await commands[update.Message.Text].ExecuteAsync(botClient, update, cancellationToken);
                }
            }
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            return Task.CompletedTask;
        }


        static void Main(string[] args)
        {
            new EnvLoader().AddEnvFile(".env").Load();

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync
            );
            Console.ReadLine();
        }
    }
}