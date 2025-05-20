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
        window.SetFramerateLimit(60); // Додаємо обмеження кадрів для основного вікна

        assetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";

        font = new Font(Path.Combine(assetsPath, "Lato-Regular.ttf"));
        clock = new Clock();

        mainMenu = new ButtonManagerMainMenu(font, window.Size.X, window.Size.Y);

        window.MouseButtonPressed += OnMousePressed;

        while (window.IsOpen)
        {
            window.DispatchEvents();
            var dt = clock.Restart();
            // Змінюємо очищення фону тут, щоб він відповідало стилю меню
            window.Clear(new Color(40, 40, 40)); // Темно-сірий фон

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
                // Тут нічого не потрібно робити, оскільки ShowGameHistory блокує виконання
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

        // Центруємо текст у вікні повідомлення
        FloatRect textRect = text.GetLocalBounds();
        text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
        text.Position = new Vector2f(messageWindow.Size.X / 2f, messageWindow.Size.Y / 2f);
        text.FillColor = Color.Black; // Змінюємо колір тексту на чорний

        messageWindow.Closed += (sender, e) => messageWindow.Close();
        messageWindow.SetFramerateLimit(60); // Обмеження кадрів

        while (messageWindow.IsOpen)
        {
            messageWindow.DispatchEvents();
            messageWindow.Clear(new Color(240, 240, 240)); // Світло-сірий фон для повідомлення
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

            // Якщо даних немає, LoadGameHistoryData вже викличе ShowMessage, тому просто виходимо
            if (historyData == null || historyData.Count == 0) return;

            // Вибираємо тільки ті ключі, які ми хочемо відобразити, і в потрібному порядку
            var keys = new List<string> { "Red", "Blue", "Green", "Yellow", "EndTime" };

            // Динамічно визначаємо ширину вікна залежно від кількості колонок
            float effectiveColumnSpacing = 180; // Збільшена відстань між колонками
            float desiredWindowWidth = 80 + keys.Count * effectiveColumnSpacing; // 40px відступи зліва і справа + (кількість колонок * відстань)
            if (desiredWindowWidth < 900) desiredWindowWidth = 900; // Мінімальна ширина

            // Динамічно визначаємо висоту вікна
            float headerHeight = 80;
            float dataAreaHeight = historyData.Count * 40; // 40 - приблизна висота рядка даних
            float buttonAreaHeight = 80; // Для кнопки очищення
            float desiredWindowHeight = headerHeight + dataAreaHeight + buttonAreaHeight;
            if (desiredWindowHeight < 600) desiredWindowHeight = 600; // Мінімальна висота

            var historyWindow = new RenderWindow(new VideoMode((uint)desiredWindowWidth, (uint)desiredWindowHeight), "Game history", Styles.Titlebar | Styles.Close);
            historyWindow.SetFramerateLimit(60); // Обмеження кадрів

            float rowHeight = 40;
            float startY = 100; // Змінено для більшого відступу зверху
            float startX = 40;  // Відступ зліва

            float buttonWidth = 200; // Ширша кнопка
            float buttonHeight = 50; // Вища кнопка
            float buttonX = (historyWindow.Size.X - buttonWidth) / 2; // Центруємо кнопку по горизонталі
            float buttonY = startY + (historyData.Count + 1) * rowHeight + 30; // Розміщуємо кнопку під сіткою з більшим відступом

            ClearHistoryButton clearButton = new ClearHistoryButton("Delete history", font, new Vector2f(buttonX, buttonY), new Vector2f(buttonWidth, buttonHeight));
            clearButton.Clicked += (sender, args) =>
            {
                try
                {
                    File.WriteAllText(filePath, "[]"); // Очищуємо вміст файлу
                    historyData.Clear(); // Очищаємо дані в пам'яті
                    ShowMessage("Історія успішно очищена!", "Очищення");
                    historyWindow.Close(); // Закриваємо вікно історії після очищення
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
                historyWindow.Clear(new Color(50, 50, 60)); // Темно-сірий фон, як у вікні результатів

                // Заголовок вікна історії
                Text historyTitle = new Text("Game history", font, 36)
                {
                    FillColor = new Color(255, 220, 100) // Золотий колір
                };
                FloatRect titleRect = historyTitle.GetLocalBounds();
                historyTitle.Origin = new Vector2f(titleRect.Left + titleRect.Width / 2f, titleRect.Top + titleRect.Height / 2f);
                historyTitle.Position = new Vector2f(historyWindow.Size.X / 2f, 40);
                historyWindow.Draw(historyTitle);


                // Заголовки колонок
                for (int i = 0; i < keys.Count; i++)
                {
                    var header = new Text(keys[i], font, 24) // Більший шрифт для заголовків
                    {
                        Position = new Vector2f(startX + i * effectiveColumnSpacing, startY - 40), // Трохи вище
                        FillColor = new Color(170, 200, 255) // Світліший синій для заголовків
                    };
                    historyWindow.Draw(header);
                }

                // Рядки даних
                for (int i = 0; i < historyData.Count; i++)
                {
                    var row = historyData[i];
                    for (int j = 0; j < keys.Count; j++)
                    {
                        if (row.TryGetValue(keys[j], out var valueObj))
                        {
                            var value = valueObj?.ToString();
                            Color cellColor = Color.White; // За замовчуванням білий текст

                            // Змінюємо колір для гравців
                            switch (keys[j].ToLower())
                            {
                                case "red":
                                    cellColor = new Color(255, 150, 150);
                                    break;
                                case "blue":
                                    cellColor = new Color(150, 200, 255);
                                    break;
                                case "green":
                                    cellColor = new Color(150, 255, 150);
                                    break;
                                case "yellow":
                                    cellColor = new Color(255, 255, 150);
                                    break;
                                case "endtime":
                                    cellColor = new Color(200, 200, 200); // Сірий для дати
                                    break;
                            }

                            var cell = new Text(value ?? "", font, 20) // Збільшуємо шрифт даних
                            {
                                Position = new Vector2f(startX + j * effectiveColumnSpacing, startY + i * rowHeight),
                                FillColor = cellColor
                            };
                            historyWindow.Draw(cell);
                        }
                    }
                }

                // Лінії сітки
                DrawGrid(historyWindow, startX, startY - 50, rowHeight, effectiveColumnSpacing, historyData.Count + 1, keys.Count); // Малюємо сітку з урахуванням заголовків

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
            return new List<Dictionary<string, object>>(); // Повертаємо порожній список, щоб уникнути NullReferenceException
        }

        string jsonString = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(jsonString) || jsonString == "[]")
        {
            ShowMessage("Історія ігор порожня.", "Історія Ігор");
            return new List<Dictionary<string, object>>();
        }
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
        // Вертикальні лінії
        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startY), new Color(100, 100, 100)); // Темно-сірі лінії
            line[1] = new Vertex(new Vector2f(x, startY + rows * rowHeight), new Color(100, 100, 100));
            target.Draw(line);
        }

        // Горизонтальні лінії
        for (int i = 0; i <= rows; i++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float y = startY + i * rowHeight;
            line[0] = new Vertex(new Vector2f(startX, y), new Color(100, 100, 100));
            line[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, y), new Color(100, 100, 100));
            target.Draw(line);
        }
    }
}