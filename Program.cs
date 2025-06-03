
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static async Task Main()
    {
        // Инициализация базы данных
        using var db = new CarpoolContext();
        await db.Database.EnsureCreatedAsync();

        var bot = new TelegramBotClient("7557499187:AAF_vBgOVLlCPZGJzK3vgD6Zth8YZ8fMbok");
        
        bot.StartReceiving(UpdateHandler, ErrorHandler);
        
        Console.WriteLine("Бот запущен. Нажмите Ctrl+C для выхода");
        await Task.Delay(-1); // Бесконечное ожидание
    }

    static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken stopCode)
    {
        if (update.Message is not { Text: { } text } message)
            return;

        using var db = new CarpoolContext();
        var chatId = message.Chat.Id;
        var cmd = text.Trim();

        try
        {
            switch (cmd)
            {
                case "/start":
                    await TelegramMenu(bot, chatId, stopCode);
                    break;

                case "🔎 Найти попутчика":
                    await bot.SendTextMessageAsync(chatId, "Введите маршрут в формате: Откуда, Куда", cancellationToken: stopCode);
                    break;

                case "📄 Добавить попутчика":
                    await bot.SendTextMessageAsync(chatId, "Введите данные в формате: Имя, Откуда, Куда, Дата (ГГГГ.ММ.ДД), Вознаграждение", cancellationToken: stopCode);
                    break;

                case "❌ Удалить попутчика":
                    var allCarpoolers = await db.Carpoolers.ToListAsync(stopCode);
                    if (!allCarpoolers.Any())
                    {
                        await bot.SendTextMessageAsync(chatId, "Нет попутчиков для удаления", cancellationToken: stopCode);
                        break;
                    }

                    var messageText = "Список попутчиков (укажите ID для удаления таким форматом: ID <номер>):\n";
                    foreach (var c in allCarpoolers)
                    {
                        messageText += $"\nID: {c.Id} - {c.Name} ({c.Origin} → {c.Destination})";
                    }
                    await bot.SendTextMessageAsync(chatId, messageText, cancellationToken: stopCode);
                    break;

                default:
                    if (cmd.Contains(','))
                    {
                        var parts = cmd.Split(',');
                        if (parts.Length == 2) // Ищем попутчика
                        {
                            var from = parts[0].Trim();
                            var to = parts[1].Trim();

                            var results = await db.Carpoolers
                                .Where(c => c.Origin.Contains(from) && c.Destination.Contains(to))
                                .ToListAsync(stopCode);

                            if (results.Any())
                            {
                                foreach (var c in results)
                                {
                                    await bot.SendTextMessageAsync(
                                        chatId,
                                        $"🚗 {c.Name}\n" +
                                        $"📍 {c.Origin} → {c.Destination}\n" +
                                        $"📅 {c.Date:yyyy.MM.dd}\n" +
                                        $"💰 {c.Reward} руб.",
                                        cancellationToken: stopCode);
                                }
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(chatId, "Попутчики не найдены", cancellationToken: stopCode);
                            }
                        }
                        else if (parts.Length == 5) // Добавляем попутчика
                        {
                            if (!DateTime.TryParseExact(parts[3].Trim(), "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out var date))
                            {
                                await bot.SendTextMessageAsync(chatId, "Неверный формат даты. Используйте ГГГГ.ММ.ДД", cancellationToken: stopCode);
                                return;
                            }

                            var carpooler = new Carpooler
                            {
                                Name = parts[0].Trim(),
                                Origin = parts[1].Trim(),
                                Destination = parts[2].Trim(),
                                Date = date,
                                Reward = decimal.Parse(parts[4].Trim())
                            };

                            db.Carpoolers.Add(carpooler);
                            await db.SaveChangesAsync(stopCode);

                            await bot.SendTextMessageAsync(chatId, "Попутчик добавлен!", cancellationToken: stopCode);
                        }
                    }
                    else if (cmd.StartsWith("ID ")) // Удаляем попутчика
                    {
                        if (int.TryParse(cmd.Substring("ID ".Length), out int id))
                        {
                            var carpooler = await db.Carpoolers.FindAsync([id], stopCode);
                            if (carpooler != null)
                            {
                                db.Carpoolers.Remove(carpooler);
                                await db.SaveChangesAsync(stopCode);
                                await bot.SendTextMessageAsync(chatId, $"Попутчик ID {id} удален", cancellationToken: stopCode);
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(chatId, "Попутчик с таким ID не найден", cancellationToken: stopCode);
                            }
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            await bot.SendTextMessageAsync(chatId, $"Ошибка: {ex.Message}", cancellationToken: stopCode);
        }
                        }

    static Task ErrorHandler(ITelegramBotClient bot, Exception error, CancellationToken stopCode)
    {
        Console.WriteLine($"Ошибка: {error.Message}");
        return Task.CompletedTask;
    }

    static async Task TelegramMenu(ITelegramBotClient bot, long chatId, CancellationToken stopCode)
    {
        var menu = new ReplyKeyboardMarkup(
        [
            [new KeyboardButton("🔎 Найти попутчика")],
            [new KeyboardButton("📄 Добавить попутчика")],
            [new KeyboardButton("❌ Удалить попутчика")]
        ])
        {
            ResizeKeyboard = true
        };
        
        await bot.SendTextMessageAsync(
            chatId,
            "Выберите действие:",
            replyMarkup: menu,
            cancellationToken: stopCode);
    }
}