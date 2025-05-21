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
    static MapRenderer mapRenderer; // Якщо MapRenderer не використовується, його можна видалити
    private const string ResultsFileName = "tournament_results.json";

    static void Main(string[] args)
    {
        window = new RenderWindow(VideoMode.DesktopMode, "SFML Game", Styles.Titlebar | Styles.Close);
        window.Closed += (_, __) => window.Close();
        window.SetFramerateLimit(60);

        assetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";

        font = new Font(Path.Combine(assetsPath, "Lato-Regular.ttf"));
        clock = new Clock();

        mainMenu = new ButtonManagerMainMenu(font, window.Size.X, window.Size.Y);

        window.MouseButtonPressed += OnMousePressed;

        while (window.IsOpen)
        {
            window.DispatchEvents();
            var dt = clock.Restart();
            window.Clear(new Color(40, 40, 40));

            var mouse = Mouse.GetPosition(window);
            if (currentState == GameState.MainMenu)
            {
                mainMenu.Update(mouse, false);
                mainMenu.Draw(window);
            }
            // GameState.Tank logic would go here if needed
            // else if (currentState == GameState.Tank && mapRenderer != null)
            // {
            //     mapRenderer.Draw(window);
            // }
            // GameState.ViewHistory is handled by its own blocking window, so nothing here.

            window.Display();
        }
    }

    static void OnMousePressed(object s, MouseButtonEventArgs e)
    {
        if (currentState != GameState.MainMenu)
        {
            return;
        }

        if (e.Button != Mouse.Button.Left) return;
        var mouse = Mouse.GetPosition(window);

        if (mainMenu.Update(mouse, true))
        {
            if (mainMenu.ViewHistoryClicked)
            {
                ShowGameHistory();
                mainMenu.ViewHistoryClicked = false;
            }
            else if (mainMenu.SelectedPlayers > 0)
            {
                currentState = GameState.Tank;
                // Припустимо, що клас Tournament існує і працює коректно
                var tournament = new Tournament(assetsPath, mainMenu.SelectedPlayers);
                tournament.Start(window);

                currentState = GameState.MainMenu;
                mainMenu.SelectedPlayers = 0;
            }
        }
    }

    static void ShowMessage(string message, string title)
    {
        var messageWindow = new RenderWindow(new VideoMode(400, 200), title, Styles.Titlebar | Styles.Close);
        var text = new Text(message, font, 20);

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

    static void ShowGameHistory()
    {
        // Змінні для прокрутки
        float scrollOffset = 0f; // Поточне зміщення контенту по Y
        float maxScrollOffset = 0f; // Максимально можливе зміщення
        const float scrollSpeedMouse = 30f; // Швидкість прокрутки колесом миші
        const float scrollSpeedKey = 80f; // Швидкість прокрутки клавішами PageUp/Down

        // Змінні для скролбара
        RectangleShape scrollbarTrack; // Фон скролбара
        RectangleShape scrollbarThumb; // "Повзунок" скролбара
        bool isDraggingThumb = false; // Чи тягне користувач повзунок
        Vector2f dragStartMousePos = new Vector2f(); // Додайте ініціалізацію
        float dragStartScrollOffset = 0f; // Додайте ініціалізацію

        try
        {
            string filePath = Path.Combine(assetsPath, ResultsFileName);
            var historyData = LoadGameHistoryData(filePath);

            if (historyData == null || historyData.Count == 0)
            {
                // Якщо немає даних, просто закриваємо вікно історії або показуємо повідомлення
                ShowMessage("Історія ігор порожня.", "Історія Ігор");
                return;
            }

            // ОНОВЛЕНО: Додано "Completed" до списку ключів
            var keys = new List<string> { "Red", "Blue", "Green", "Yellow", "EndTime", "Completed" };
            float headerRowHeight = 50; // Висота для заголовків колонок
            float dataRowHeight = 40; // Висота для кожної строчки даних
            float startYHeader = 100; // Y-координата для заголовків колонок
            float startYData = startYHeader + headerRowHeight; // Y-координата, з якої починається відображення даних (і сітки даних)
            float startX = 40;
            // ОНОВЛЕНО: Збільшено effectiveColumnSpacing, оскільки додано новий стовпець
            float effectiveColumnSpacing = 160;

            float buttonWidth = 200;
            float buttonHeight = 50;
            float buttonMarginBottom = 30; // Відступ кнопки від низу вікна

            // Розрахунок розмірів вікна
            float desiredWindowWidth = 80 + keys.Count * effectiveColumnSpacing + 40; // Додатковий відступ для скролбара
            if (desiredWindowWidth < 900) desiredWindowWidth = 900;

            // Висота видимої області для прокрутки даних
            // Це висота "внутрішнього" прямокутника, де відображаються дані
            float scrollableAreaHeight = VideoMode.DesktopMode.Height * 0.7f - startYData - buttonHeight - buttonMarginBottom - 20; // -20 для додаткового відступу
            if (scrollableAreaHeight < 200) scrollableAreaHeight = 200; // Мінімальна висота для прокрутки

            float totalContentHeight = historyData.Count * dataRowHeight;
            maxScrollOffset = Math.Max(0, totalContentHeight - scrollableAreaHeight);

            // Висота вікна буде адаптивною, але не більшою за 70% екрану, щоб не вилазила за межі
            float maxWindowHeight = VideoMode.DesktopMode.Height * 0.7f;
            float desiredWindowHeight = startYData + Math.Min(totalContentHeight, scrollableAreaHeight) + buttonHeight + buttonMarginBottom + 20; // +20 для додаткового відступу
            if (desiredWindowHeight < 600) desiredWindowHeight = 600;
            if (desiredWindowHeight > maxWindowHeight) desiredWindowHeight = maxWindowHeight;


            var historyWindow = new RenderWindow(new VideoMode((uint)desiredWindowWidth, (uint)desiredWindowHeight), "Game history", Styles.Titlebar | Styles.Close);
            historyWindow.SetFramerateLimit(60);

            // Ініціалізація скролбара
            // Розмір треку скролбара відповідає висоті прокручуваної області
            scrollbarTrack = new RectangleShape(new Vector2f(20, scrollableAreaHeight))
            {
                FillColor = new Color(70, 70, 80),
                Position = new Vector2f(historyWindow.Size.X - 20 - 10, startYData) // Позиція треку скролбара співпадає з початком області даних
            };
            scrollbarThumb = new RectangleShape()
            {
                FillColor = new Color(150, 150, 160)
            };

            // Ініціалізація кнопки "Видалити історію"
            ClearHistoryButton clearButton = new ClearHistoryButton("Delete history", font,
                new Vector2f((historyWindow.Size.X - buttonWidth) / 2, historyWindow.Size.Y - buttonHeight - buttonMarginBottom),
                new Vector2f(buttonWidth, buttonHeight));

            clearButton.Clicked += (sender, args) =>
            {
                try
                {
                    File.WriteAllText(filePath, "[]");
                    historyData.Clear(); // Очищаємо дані в пам'яті
                    scrollOffset = 0; // Скидаємо прокрутку
                    maxScrollOffset = 0; // Оновлюємо maxScrollOffset
                    ShowMessage("History was removed", "Removing");
                    // historyWindow.Close(); // Можна закрити, а можна залишити порожнє вікно
                }
                catch (Exception ex)
                {
                    ShowMessage($"Помилка при очищенні історії: {ex.Message}", "Помилка");
                }
            };

            // Обробники подій для вікна історії
            historyWindow.Closed += (_, __) => historyWindow.Close();
            historyWindow.MouseWheelScrolled += (sender, e) =>
            {
                if (maxScrollOffset > 0)
                {
                    scrollOffset += e.Delta * scrollSpeedMouse;
                    ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                }
            };
            historyWindow.KeyPressed += (sender, e) =>
            {
                if (maxScrollOffset > 0)
                {
                    if (e.Code == Keyboard.Key.PageUp)
                    {
                        scrollOffset += scrollSpeedKey;
                    }
                    else if (e.Code == Keyboard.Key.PageDown)
                    {
                        scrollOffset -= scrollSpeedKey;
                    }
                    ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                }
            };
            historyWindow.MouseButtonPressed += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Left)
                {
                    var mousePos = Mouse.GetPosition(historyWindow);

                    // Перевірка кліку на скролбар
                    if (maxScrollOffset > 0 && scrollbarThumb.GetGlobalBounds().Contains(e.X, e.Y))
                    {
                        isDraggingThumb = true;
                        dragStartMousePos = new Vector2f(e.X, e.Y);
                        dragStartScrollOffset = scrollOffset;
                    }
                    else if (maxScrollOffset > 0 && scrollbarTrack.GetGlobalBounds().Contains(e.X, e.Y))
                    {
                        float clickY = e.Y;
                        float thumbCenterY = scrollbarThumb.Position.Y + scrollbarThumb.Size.Y / 2f;

                        if (clickY < thumbCenterY)
                        {
                            // Клік над повзунком, прокручуємо вгору (зменшуємо scrollOffset)
                            scrollOffset += scrollableAreaHeight * 0.8f;
                        }
                        else
                        {
                            // Клік під повзунком, прокручуємо вниз (збільшуємо scrollOffset, але це від'ємне значення)
                            scrollOffset -= scrollableAreaHeight * 0.8f;
                        }
                        ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                    }
                    // Перевірка кліку на кнопку очищення
                    else if (clearButton.IsClicked(mousePos))
                    {
                        clearButton.OnClick();
                    }
                }
            };
            historyWindow.MouseButtonReleased += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Left)
                {
                    isDraggingThumb = false;
                }
            };
            historyWindow.MouseMoved += (sender, e) =>
            {
                if (isDraggingThumb)
                {
                    float currentMouseY = e.Y;
                    float deltaY = currentMouseY - dragStartMousePos.Y;

                    // Розраховуємо нове зміщення прокрутки пропорційно переміщенню повзунка
                    if (scrollbarTrack.Size.Y - scrollbarThumb.Size.Y > 0)
                    {
                        float scrollbarMovementRatio = deltaY / (scrollbarTrack.Size.Y - scrollbarThumb.Size.Y);
                        // Знак змінено на '+' для коректної прокрутки:
                        // якщо ми тягнемо повзунок вниз (deltaY > 0), scrollOffset має зменшуватися (бути більш від'ємним)
                        scrollOffset = dragStartScrollOffset - (scrollbarMovementRatio * maxScrollOffset);
                    }
                    ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                }
            };

            while (historyWindow.IsOpen)
            {
                historyWindow.DispatchEvents();
                historyWindow.Clear(new Color(50, 50, 60)); // Фон вікна

                // 1. Малюємо заголовки колонок
                for (int i = 0; i < keys.Count; i++)
                {
                    var header = new Text(keys[i], font, 24)
                    {
                        Position = new Vector2f(startX + i * effectiveColumnSpacing, startYHeader),
                        FillColor = new Color(170, 200, 255)
                    };
                    historyWindow.Draw(header);
                }

                // 2. Малюємо сітку для заголовків та для області даних.
                // Сітка заголовків (одна лінія під заголовками)
                DrawGridHeader(historyWindow, startX, startYHeader, effectiveColumnSpacing, keys.Count, headerRowHeight);
                // Сітка даних (вертикальні лінії по всій висоті таблиці, горизонтальні тільки в scrollableArea)
                DrawGridData(historyWindow, startX, startYData, dataRowHeight, effectiveColumnSpacing, historyData.Count, keys.Count, scrollOffset, scrollableAreaHeight);


                // 3. Малюємо рядки даних (з прокруткою)
                for (int i = 0; i < historyData.Count; i++)
                {
                    float currentY = startYData + i * dataRowHeight + scrollOffset;

                    // Малюємо лише ті рядки, які знаходяться у видимій області прокрутки
                    // Перевіряємо, чи перетинається рядок з видимою областю (startYData до startYData + scrollableAreaHeight)
                    if (currentY + dataRowHeight > startYData && currentY < startYData + scrollableAreaHeight)
                    {
                        var row = historyData[i];
                        for (int j = 0; j < keys.Count; j++)
                        {
                            string displayValue = "";
                            Color cellColor = Color.White;

                            if (keys[j] == "Completed")
                            {
                                if (row.TryGetValue(keys[j], out var completedObj) && completedObj is JsonElement completedElement)
                                {
                                    if (completedElement.ValueKind == JsonValueKind.True)
                                    {
                                        displayValue = "Yes";
                                        cellColor = new Color(100, 200, 100); // Зелений для "Yes"
                                    }
                                    else if (completedElement.ValueKind == JsonValueKind.False)
                                    {
                                        displayValue = "No";
                                        cellColor = new Color(200, 100, 100); // Червоний для "No"
                                    }
                                    else
                                    {
                                        displayValue = "N/A";
                                        cellColor = new Color(150, 150, 150); // Сірий для невизначених
                                    }
                                }
                                else
                                {
                                    displayValue = "N/A";
                                    cellColor = new Color(150, 150, 150); // Сірий для відсутніх
                                }
                            }
                            else if (row.TryGetValue(keys[j], out var valueObj))
                            {
                                displayValue = valueObj?.ToString();
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
                                        cellColor = new Color(200, 200, 200);
                                        break;
                                }
                            }

                            var cell = new Text(displayValue, font, 20)
                            {
                                Position = new Vector2f(startX + j * effectiveColumnSpacing, currentY),
                                FillColor = cellColor
                            };
                            historyWindow.Draw(cell);
                        }
                    }
                }

                // 4. Заголовок вікна історії (малюється поверх всього, не прокручується)
                Text historyTitle = new Text("Game history", font, 36)
                {
                    FillColor = new Color(255, 220, 100)
                };
                FloatRect titleRect = historyTitle.GetLocalBounds();
                historyTitle.Origin = new Vector2f(titleRect.Left + titleRect.Width / 2f, titleRect.Top + titleRect.Height / 2f);
                historyTitle.Position = new Vector2f(historyWindow.Size.X / 2f, 40);
                historyWindow.Draw(historyTitle);


                // 5. Оновлення та малювання скролбара (поверх даних)
                UpdateScrollbarThumbPosition(scrollbarThumb, scrollbarTrack, scrollOffset, maxScrollOffset, totalContentHeight, scrollableAreaHeight);

                if (maxScrollOffset > 0)
                {
                    historyWindow.Draw(scrollbarTrack);
                    historyWindow.Draw(scrollbarThumb);
                }

                // 6. Кнопка "Очистити дані" (малюється поверх всього)
                clearButton.Draw(historyWindow, RenderStates.Default);

                historyWindow.Display();
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Помилка при читанні історії: {ex.Message}", "Помилка");
        }
    }

    // Допоміжна функція для оновлення позиції повзунка скролбара
    static void UpdateScrollbarThumbPosition(RectangleShape thumb, RectangleShape track, float currentScrollOffset, float maxScroll, float totalContentHeight, float scrollableAreaHeight)
    {
        if (maxScroll <= 0)
        {
            thumb.Size = new Vector2f(track.Size.X, track.Size.Y);
            thumb.Position = track.Position;
            return;
        }

        // Розраховуємо співвідношення видимої висоти до загальної висоти контенту
        float visibleHeightRatio = scrollableAreaHeight / totalContentHeight;
        if (visibleHeightRatio > 1f) visibleHeightRatio = 1f; // Не може бути більше 1

        // Висота повзунка
        float thumbHeight = track.Size.Y * visibleHeightRatio;
        thumbHeight = Math.Max(30f, thumbHeight); // Мінімальна висота повзунка

        thumb.Size = new Vector2f(track.Size.X, thumbHeight);

        // Розраховуємо позицію повзунка
        float thumbYPosition = track.Position.Y;
        // Прокрутка від'ємна, тому віднімаємо її, щоб отримати позицію від верхнього краю треку
        thumbYPosition += (-currentScrollOffset / maxScroll) * (track.Size.Y - thumbHeight);

        thumb.Position = new Vector2f(track.Position.X, thumbYPosition);
    }

    // Допоміжна функція для обмеження прокрутки
    static void ApplyScrollLimits(ref float scrollOffset, float maxScrollOffset)
    {
        if (scrollOffset > 0) scrollOffset = 0; // Не можна прокрутити вище початку
        if (scrollOffset < -maxScrollOffset) scrollOffset = -maxScrollOffset; // Не можна прокрутити нижче кінця
    }

    static List<Dictionary<string, object>> LoadGameHistoryData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // Не показуємо повідомлення, якщо файл просто відсутній.
            // Його буде створено автоматично з порожнім масивом.
            File.WriteAllText(filePath, "[]"); // Створюємо порожній файл JSON
            return new List<Dictionary<string, object>>();
        }

        string jsonString = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(jsonString) || jsonString == "[]")
        {
            // Якщо файл порожній або містить лише порожній масив, повертаємо порожній список.
            // Повідомлення про порожню історію краще показати один раз, коли вікно відкривається.
            return new List<Dictionary<string, object>>();
        }
        try
        {
            // Використовуємо JsonElement для більш гнучкої десеріалізації, оскільки Dictionary<string, object>
            // може не завжди коректно обробляти булеві значення без додаткових налаштувань.
            // Ми будемо перевіряти JsonValueKind.
            var historyData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonString);

            // Конвертуємо JsonElement назад в object, якщо потрібно для сумісності з поточним кодом.
            // Або змінюємо тип historyData на List<Dictionary<string, JsonElement>> і відповідно оновлюємо логіку відображення.
            // Для цієї задачі простіше оновити логіку відображення.

            // Якщо historyData може бути null (наприклад, якщо файл JSON не містить масиву), 
            // повертаємо новий порожній список.
            return historyData?.Select(dict => dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value)).ToList()
                   ?? new List<Dictionary<string, object>>();
        }
        catch (JsonException ex)
        {
            ShowMessage($"Помилка при десеріалізації історії: {ex.Message}\nПереконайтеся, що файл 'tournament_results.json' містить коректний JSON.", "Помилка");
            // Можливо, варто видалити або перейменувати пошкоджений файл тут
            return new List<Dictionary<string, object>>();
        }
    }

    // Нова функція для малювання сітки заголовків
    static void DrawGridHeader(RenderTarget target, float startX, float startYHeader, float columnSpacing, int cols, float headerRowHeight)
    {
        Color lineColor = new Color(100, 100, 100);

        // Вертикальні лінії для заголовків
        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startYHeader), lineColor);
            line[1] = new Vertex(new Vector2f(x, startYHeader + headerRowHeight), lineColor);
            target.Draw(line);
        }

        // Горизонтальні лінії для заголовків
        // Верхня межа заголовків
        var topLine = new VertexArray(PrimitiveType.Lines, 2);
        topLine[0] = new Vertex(new Vector2f(startX, startYHeader), lineColor);
        topLine[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, startYHeader), lineColor);
        target.Draw(topLine);

        // Нижня межа заголовків
        var bottomLine = new VertexArray(PrimitiveType.Lines, 2);
        bottomLine[0] = new Vertex(new Vector2f(startX, startYHeader + headerRowHeight), lineColor);
        bottomLine[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, startYHeader + headerRowHeight), lineColor);
        target.Draw(bottomLine);
    }

    // Нова функція для малювання сітки даних
    static void DrawGridData(RenderTarget target, float startX, float startYData, float rowHeight, float columnSpacing, int totalRows, int cols, float scrollOffset, float scrollableAreaHeight)
    {
        Color lineColor = new Color(100, 100, 100);

        // Вертикальні лінії для даних (від початку області даних до кінця прокручуваної області)
        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startYData), lineColor);
            line[1] = new Vertex(new Vector2f(x, startYData + scrollableAreaHeight), lineColor);
            target.Draw(line);
        }

        // Горизонтальні лінії для даних (прокручуються разом з даними)
        // Малюємо лише ті лінії, які потрапляють у видиму область
        for (int i = 0; i <= totalRows; i++) // totalRows + 1 для нижньої лінії останнього рядка
        {
            float y = startYData + i * rowHeight + scrollOffset;

            // Малюємо лінію, якщо вона знаходиться у видимій області даних
            if (y >= startYData && y <= startYData + scrollableAreaHeight)
            {
                var line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(new Vector2f(startX, y), lineColor);
                line[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, y), lineColor);
                target.Draw(line);
            }
        }
    }
}
