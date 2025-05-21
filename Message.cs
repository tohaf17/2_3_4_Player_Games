using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Threading; // Додайте цей using для роботи з потоками
using System; // Для Exception

namespace k
{
    public static class Messanger // Може бути статичним класом або синглтоном
    {

        public static void ShowMessage(string message, string title)
        {

            // Створюємо новий потік для вікна повідомлення
            Thread messageThread = new Thread(() =>
            {
                RenderWindow messageWindow = null;
                try
                {
                    messageWindow = new RenderWindow(new VideoMode(400, 200), title, Styles.Titlebar | Styles.Close);
                    Text text = new Text(message, new Font(k.Constants.Font), 20); // Використовуємо ініціалізований шрифт

                    FloatRect textRect = text.GetLocalBounds();
                    text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
                    text.Position = new Vector2f(messageWindow.Size.X / 2f, messageWindow.Size.Y / 2f);
                    text.FillColor = Color.Black;

                    messageWindow.Closed += (sender, e) => messageWindow.Close();
                    messageWindow.SetFramerateLimit(60);

                    while (messageWindow.IsOpen)
                    {
                        messageWindow.DispatchEvents();
                        messageWindow.Clear(new Color(240, 240, 240));
                        messageWindow.Draw(text);
                        messageWindow.Display();
                    }
                }
                catch (Exception ex)
                {
                    // Обробка помилок у самому потоці повідомлень, якщо вони виникнуть
                    Console.WriteLine($"Error in message window thread: {ex.Message}");
                }
                finally
                {
                    // Важливо звільнити ресурси SFML, якщо вікно було створено
                    messageWindow?.Dispose();
                }
            });

            // Запускаємо потік як фоновий, щоб він автоматично завершився, якщо основна програма закриється
            messageThread.IsBackground = true;
            messageThread.Start();
        }
    }
}