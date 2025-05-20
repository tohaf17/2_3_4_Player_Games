using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace k;

class Program
{
    public enum GameState
    {
        MainMenu,
        Tank,
        ViewHistory
    }

    static RenderWindow window;
    static string assetsPath;
    static GameState currentState = GameState.MainMenu;
    static Font font;
    static ButtonManagerMainMenu mainMenu;
    static Clock clock;
    static MapRenderer mapRenderer;
    private const string ResultsFileName = "tournament_results.json";

    static void Main(string[] args)
    {
        window = new RenderWindow(VideoMode.DesktopMode, "SFML Game", Styles.Titlebar | Styles.Close);
        window.Closed += (_, __) => window.Close();

        assetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";

        font = new Font(Path.Combine(assetsPath, "Lato-Regular.ttf"));
        clock = new Clock();

        mainMenu = new ButtonManagerMainMenu(font, window.Size.X, window.Size.Y);

        window.MouseButtonPressed += OnMousePressed;

        while (window.IsOpen)
        {
            window.DispatchEvents();
            var dt = clock.Restart();
            window.Clear(Color.Black);

            var mouse = Mouse.GetPosition(window);
            if (currentState == GameState.MainMenu)
            {
                mainMenu.Update(mouse, false);
                mainMenu.Draw(window);
            }
            else if (currentState == GameState.Tank && mapRenderer != null)
            {
                mapRenderer.Draw(window);
            }
            else if (currentState == GameState.ViewHistory)
            {
                // Відображення вікна історії відбувається в обробнику OnMousePressed
            }

            window.Display();
        }
    }

    static void OnMousePressed(object s, MouseButtonEventArgs e)
    {
        if (e.Button != Mouse.Button.Left) return;
        var mouse = Mouse.GetPosition(window);

        if (currentState == GameState.MainMenu && mainMenu.Update(mouse, true))
        {
            if (mainMenu.ViewHistoryClicked)
            {
                currentState = GameState.ViewHistory;
                ShowGameHistory();
                currentState = GameState.MainMenu; // Повертаємося до головного меню після перегляду історії
                mainMenu.ViewHistoryClicked = false; // Скидаємо прапорець
            }
            // ... inside Program.OnMousePressed
            else if (mainMenu.SelectedPlayers > 0)
            {
                var tournament = new Tournament(assetsPath, mainMenu.SelectedPlayers);
                tournament.Start(window);
                currentState = GameState.MainMenu; // This line ensures return to main menu
                mainMenu.SelectedPlayers = 0; // Reset player selection
            }
        }
    }



    static void ShowMessage(string message, string title)
    {
        var messageWindow = new RenderWindow(new VideoMode(400, 200), title, Styles.Titlebar | Styles.Close);
        var text = new Text(message, font, 20);
        text.Position = new Vector2f(20, 80);

        messageWindow.Closed += (sender, e) => messageWindow.Close();

        while (messageWindow.IsOpen)
        {
            messageWindow.DispatchEvents(); // Додано DispatchEvents для обробки події Closed
            messageWindow.Clear(Color.White);
            messageWindow.Draw(text);
            messageWindow.Display();
        }
    }

    static void ShowGameHistory()
    {
        try
        {
            string filePath = Path.Combine(assetsPath, ResultsFileName);

            var historyData = LoadGameHistoryData(filePath);

            if (historyData == null) return;

            var allKeys = historyData.SelectMany(dict => dict.Keys).Distinct().ToList();
            var keys = new List<string> { "Red", "Blue","Green","Yellow", "EndTime" };

            var historyWindow = new RenderWindow(new VideoMode(900, 900), "Історія Ігор", Styles.Titlebar | Styles.Close); // Збільшуємо висоту вікна
            float rowHeight = 40;
            float startY = 60;
            float startX = 20;
            float columnSpacing = 150;
            float padding = 5;
            float buttonWidth = 150;
            float buttonHeight = 40;
            float buttonX = startX;
            float buttonY = startY + historyData.Count * rowHeight + 20; // Розміщуємо кнопку під сіткою

            ClearHistoryButton clearButton = new ClearHistoryButton("Remove data", font, new Vector2f(buttonX, buttonY), new Vector2f(buttonWidth, buttonHeight));
            clearButton.Clicked += (sender, args) =>
            {
                try
                {
                    File.WriteAllText(filePath, "[]"); // Очищуємо вміст файлу
                    historyData = LoadGameHistoryData(filePath); // Перезавантажуємо дані
                }
                catch (Exception ex)
                {
                    ShowMessage($"Помилка при очищенні історії: {ex.Message}", "Помилка");
                }
            };

            historyWindow.MouseButtonPressed += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Left)
                {
                    var mousePos = Mouse.GetPosition(historyWindow);
                    if (clearButton.IsClicked(mousePos))
                    {
                        clearButton.OnClick();
                    }
                }
            };

            historyWindow.Closed += (_, __) => historyWindow.Close();

            while (historyWindow.IsOpen)
            {
                historyWindow.DispatchEvents();
                historyWindow.Clear(Color.White);

                // Заголовки
                for (int i = 0; i < keys.Count; i++)
                {
                    var header = new Text(keys[i], font, 20)
                    {
                        Position = new Vector2f(startX + i * columnSpacing + padding, 20),
                        FillColor = Color.Black
                    };
                    historyWindow.Draw(header);
                }

                // Рядки
                for (int i = 0; i < historyData.Count; i++)
                {
                    var row = historyData[i];
                    for (int j = 0; j < keys.Count; j++)
                    {
                        if (row.TryGetValue(keys[j], out var valueObj))
                        {
                            var value = valueObj?.ToString();
                            var cell = new Text(value ?? "", font, 18)
                            {
                                Position = new Vector2f(startX + j * columnSpacing + padding, startY + i * rowHeight),
                                FillColor = Color.Black
                            };
                            historyWindow.Draw(cell);
                        }
                    }
                }

                // Лінії сітки
                DrawGrid(historyWindow, startX, startY, rowHeight, columnSpacing, historyData.Count, keys.Count);

                // Кнопка "Очистити дані"
                clearButton.Draw(historyWindow, RenderStates.Default);

                historyWindow.Display();
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Помилка при читанні історії: {ex.Message}", "Помилка");
        }
    }

    static List<Dictionary<string, object>> LoadGameHistoryData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            ShowMessage("Файл історії не знайдено.", "Історія Ігор");
            return new List<Dictionary<string, object>>();
        }

        string jsonString = File.ReadAllText(filePath);
        try
        {
            var historyData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString);
            return historyData ?? new List<Dictionary<string, object>>();
        }
        catch (JsonException ex)
        {
            ShowMessage($"Помилка при десеріалізації історії: {ex.Message}", "Помилка");
            return new List<Dictionary<string, object>>();
        }
    }

    static void DrawGrid(RenderTarget target, float startX, float startY, float rowHeight, float columnSpacing, int rows, int cols)
    {
        for (int i = 0; i <= rows; i++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float y = startY + i * rowHeight;
            line[0] = new Vertex(new Vector2f(startX, y), Color.Black);
            line[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, y), Color.Black);
            target.Draw(line);
        }

        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startY), Color.Black);
            line[1] = new Vertex(new Vector2f(x, startY + rows * rowHeight), Color.Black);
            target.Draw(line);
        }
    }

}