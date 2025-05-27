using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Threading; 
namespace k
{
    public static class Messanger 
    {

        public static void ShowMessage(string message, string title)
        {
            
            Thread messageThread = new Thread(() =>
            {
                RenderWindow messageWindow = null;
                try
                {
                    messageWindow = new RenderWindow(new VideoMode(400, 200), title, Styles.Titlebar | Styles.Close);
                    Text text = new Text(message, new Font(k.Constants.Font), 20); 

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
                    Console.WriteLine($"Error in message window thread: {ex.Message}");
                }
                finally
                {
                    messageWindow?.Dispose();
                }
            });

            messageThread.IsBackground = true;
            messageThread.Start();
        }
    }
}