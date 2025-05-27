using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
using static k.Constants;
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
        ViewHistory,
        HowToPlay
    }

    static RenderWindow window;
    static GameState currentState = GameState.MainMenu;
    static Font font;
    static ButtonManagerMainMenu mainMenu;
    static Clock clock;

    static void Main()
    {
        window = new RenderWindow(VideoMode.DesktopMode, "SFML Game", Styles.Titlebar | Styles.Close);
        window.Closed += (_, __) => window.Close();
       

        font = new Font(k.Constants.Font);
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

        bool buttonWasActivated = mainMenu.Update(mouse, true);

        if (buttonWasActivated)
        {
            if (mainMenu.IsViewHistoryClickedAndReset())
            {
                ShowGameHistory();
                
            }
            else if (mainMenu.IsHowToPlayClickedAndReset())
            {
                ShowHowToPlay();
            }
            else if (mainMenu.SelectedPlayers > 0)
            {
                currentState = GameState.Tank; 
                var tournament = new Tournament(mainMenu.SelectedPlayers);
                tournament.Start(window); 

                currentState = GameState.MainMenu; 
                mainMenu.SelectedPlayers = 0; 
                foreach (var btn in mainMenu.Buttons)
                {
                    if (btn.GetText().Contains("Players"))
                    {
                        btn.Deselect();
                    }
                }
            }
        }
    }

    static void ShowHowToPlay()
    {
        float windowWidth = VideoMode.DesktopMode.Width * 0.6f;
        float windowHeight = VideoMode.DesktopMode.Height * 0.7f;

        var howToPlayWindow = new RenderWindow(new VideoMode((uint)windowWidth, (uint)windowHeight), "How To Play", Styles.Titlebar | Styles.Close);
        howToPlayWindow.SetFramerateLimit(60);

        howToPlayWindow.Closed += (_, __) => howToPlayWindow.Close();

        Text title = new Text("How To Play", font, 36)
        {
            FillColor = new Color(255, 220, 100)
        };
        FloatRect titleRect = title.GetLocalBounds();
        title.Origin = new Vector2f(titleRect.Left + titleRect.Width / 2f, titleRect.Top + titleRect.Height / 2f);
        title.Position = new Vector2f(howToPlayWindow.Size.X / 2f, 40);

        string instructionsTextContent =
            "Welcome to Tanks!\n\n" +
            "Objective: Destroy all enemy tanks to win the round. Be the last tank standing!\n\n" +
            "Controls:\n" +
            " - Red tank (Q): Move and shoot\n" +
            " - Blue tank (M): Move and shoot\n" +
            " - Green tank (NumPad9): Move and shoot\n" +
            " - Yellow tank (V): Move and shoot\n" +
            " - Bomb: Fire using the same dedicated button as for regular shots.\n\n" +
            "Power-ups (active for 10 seconds):\n" +
            " - Shield Pack: Creates a protective shield around your tank.\n" +
            " - Mini Boost: Temporarily shrinks your tank, making it harder to hit.\n\n" +
            "Tips:\n" +
            " - Use wall blocks for cover.\n" +
            " - Anticipate enemy movements.\n" +
            " - Collect power-ups to gain an advantage.\n\n" +
            "Good luck, Commander!";

        Text instructionsText = new Text(instructionsTextContent, font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(50, 120)
        };
        while (howToPlayWindow.IsOpen)
        {
            howToPlayWindow.DispatchEvents();
            howToPlayWindow.Clear(new Color(50, 50, 60));

            howToPlayWindow.Draw(title);
            howToPlayWindow.Draw(instructionsText);

            howToPlayWindow.Display();
        }
    }

    static void ShowGameHistory()
    {
        float scrollOffset = 0f;
        float maxScrollOffset = 0f;
        const float scrollSpeedMouse = 30f;
        const float scrollSpeedKey = 80f;

        RectangleShape scrollbarTrack;
        RectangleShape scrollbarThumb;
        bool isDraggingThumb = false;
        Vector2f dragStartMousePos = new Vector2f();
        float dragStartScrollOffset = 0f;

        try
        {
            string filePath = Path.Combine(AssetsPath, ResultsFileName);
            var historyData = LoadGameHistoryData(filePath);

            if (historyData == null || historyData.Count == 0)
            {
                Messanger.ShowMessage("History is empty", "History");
                return;
            }

            var keys = new List<string> { "Red", "Blue", "Green", "Yellow", "EndTime", "Completed" };
            float headerRowHeight = 50;
            float dataRowHeight = 40;
            float startYHeader = 100;
            float startYData = startYHeader + headerRowHeight;
            float startX = 40;

            float effectiveColumnSpacing = 160;

            float buttonWidth = 200;
            float buttonHeight = 50;
            float buttonMarginBottom = 30;

            float desiredWindowWidth = 80 + keys.Count * effectiveColumnSpacing + 40;
            if (desiredWindowWidth < 900) desiredWindowWidth = 900;

            float scrollableAreaHeight = VideoMode.DesktopMode.Height * 0.7f - startYData - buttonHeight - buttonMarginBottom - 20;
            if (scrollableAreaHeight < 200) scrollableAreaHeight = 200;

            float totalContentHeight = historyData.Count * dataRowHeight;
            maxScrollOffset = Math.Max(0, totalContentHeight - scrollableAreaHeight);

            float maxWindowHeight = VideoMode.DesktopMode.Height * 0.7f;
            float desiredWindowHeight = startYData + Math.Min(totalContentHeight, scrollableAreaHeight) + buttonHeight + buttonMarginBottom + 20;
            if (desiredWindowHeight < 600) desiredWindowHeight = 600;
            if (desiredWindowHeight > maxWindowHeight) desiredWindowHeight = maxWindowHeight;


            var historyWindow = new RenderWindow(new VideoMode((uint)desiredWindowWidth, (uint)desiredWindowHeight), "Game history", Styles.Titlebar | Styles.Close);
            historyWindow.SetFramerateLimit(60);

            scrollbarTrack = new RectangleShape(new Vector2f(20, scrollableAreaHeight))
            {
                FillColor = new Color(70, 70, 80),
                Position = new Vector2f(historyWindow.Size.X - 20 - 10, startYData)
            };
            scrollbarThumb = new RectangleShape()
            {
                FillColor = new Color(150, 150, 160)
            };

            ClearHistoryButton clearButton = new ClearHistoryButton("Delete history", font,
                new Vector2f((historyWindow.Size.X - buttonWidth) / 2, historyWindow.Size.Y - buttonHeight - buttonMarginBottom),
                new Vector2f(buttonWidth, buttonHeight));

            clearButton.Clicked += (sender, args) =>
            {
                try
                {
                    File.WriteAllText(filePath, "[]");
                    historyData.Clear();
                    scrollOffset = 0;
                    maxScrollOffset = 0;
                    Messanger.ShowMessage("History was removed", "Removing");

                }
                catch (Exception ex)
                {
                    Messanger.ShowMessage($"Some problem: {ex.Message}", "Помилка");
                }
            };

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
                            scrollOffset += scrollableAreaHeight * 0.8f;
                        }
                        else
                        {
                            scrollOffset -= scrollableAreaHeight * 0.8f;
                        }
                        ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                    }
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

                    if (scrollbarTrack.Size.Y - scrollbarThumb.Size.Y > 0)
                    {
                        float scrollbarMovementRatio = deltaY / (scrollbarTrack.Size.Y - scrollbarThumb.Size.Y);
                        scrollOffset = dragStartScrollOffset - (scrollbarMovementRatio * maxScrollOffset);
                    }
                    ApplyScrollLimits(ref scrollOffset, maxScrollOffset);
                }
            };

            while (historyWindow.IsOpen)
            {
                historyWindow.DispatchEvents();
                historyWindow.Clear(new Color(50, 50, 60));

                for (int i = 0; i < keys.Count; i++)
                {
                    var header = new Text(keys[i], font, 24)
                    {
                        Position = new Vector2f(startX + i * effectiveColumnSpacing, startYHeader),
                        FillColor = new Color(170, 200, 255)
                    };
                    historyWindow.Draw(header);
                }

                DrawGridHeader(historyWindow, startX, startYHeader, effectiveColumnSpacing, keys.Count, headerRowHeight);

                DrawGridData(historyWindow, startX, startYData, dataRowHeight, effectiveColumnSpacing, historyData.Count, keys.Count, scrollOffset, scrollableAreaHeight);


                for (int i = 0; i < historyData.Count; i++)
                {
                    float currentY = startYData + i * dataRowHeight + scrollOffset;


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
                                        cellColor = new Color(100, 200, 100);
                                    }
                                    else if (completedElement.ValueKind == JsonValueKind.False)
                                    {
                                        displayValue = "No";
                                        cellColor = new Color(200, 100, 100);
                                    }
                                    else
                                    {
                                        displayValue = "N/A";
                                        cellColor = new Color(150, 150, 150);
                                    }
                                }
                                else
                                {
                                    displayValue = "N/A";
                                    cellColor = new Color(150, 150, 150);
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


                Text historyTitle = new Text("Game history", font, 36)
                {
                    FillColor = new Color(255, 220, 100)
                };
                FloatRect titleRect = historyTitle.GetLocalBounds();
                historyTitle.Origin = new Vector2f(titleRect.Left + titleRect.Width / 2f, titleRect.Top + titleRect.Height / 2f);
                historyTitle.Position = new Vector2f(historyWindow.Size.X / 2f, 40);
                historyWindow.Draw(historyTitle);


                UpdateScrollbarThumbPosition(scrollbarThumb, scrollbarTrack, scrollOffset, maxScrollOffset, totalContentHeight, scrollableAreaHeight);

                if (maxScrollOffset > 0)
                {
                    historyWindow.Draw(scrollbarTrack);
                    historyWindow.Draw(scrollbarThumb);
                }
                clearButton.Draw(historyWindow, RenderStates.Default);

                historyWindow.Display();
            }
        }
        catch (Exception ex)
        {
            Messanger.ShowMessage($"Помилка при читанні історії: {ex.Message}", "Помилка");
        }
    }

    static void UpdateScrollbarThumbPosition(RectangleShape thumb, RectangleShape track, float currentScrollOffset, float maxScroll, float totalContentHeight, float scrollableAreaHeight)
    {
        if (maxScroll <= 0)
        {
            thumb.Size = new Vector2f(track.Size.X, track.Size.Y);
            thumb.Position = track.Position;
            return;
        }

        float visibleHeightRatio = scrollableAreaHeight / totalContentHeight;
        if (visibleHeightRatio > 1f) visibleHeightRatio = 1f;

        float thumbHeight = track.Size.Y * visibleHeightRatio;
        thumbHeight = Math.Max(30f, thumbHeight);

        thumb.Size = new Vector2f(track.Size.X, thumbHeight);

        float thumbYPosition = track.Position.Y;

        thumbYPosition += (-currentScrollOffset / maxScroll) * (track.Size.Y - thumbHeight);

        thumb.Position = new Vector2f(track.Position.X, thumbYPosition);
    }

    static void ApplyScrollLimits(ref float scrollOffset, float maxScrollOffset)
    {
        if (scrollOffset > 0) scrollOffset = 0;
        if (scrollOffset < -maxScrollOffset) scrollOffset = -maxScrollOffset;
    }

    static List<Dictionary<string, object>> LoadGameHistoryData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
            return new List<Dictionary<string, object>>();
        }

        string jsonString = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(jsonString) || jsonString == "[]")
        {
            return new List<Dictionary<string, object>>();
        }
        try
        {
            var historyData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonString);

            return historyData?.Select(dict => dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value)).ToList()
                             ?? new List<Dictionary<string, object>>();
        }
        catch (JsonException ex)
        {
            Messanger.ShowMessage($"{ex.Message}", "Error");

            return new List<Dictionary<string, object>>();
        }
    }

    static void DrawGridHeader(RenderTarget target, float startX, float startYHeader, float columnSpacing, int cols, float headerRowHeight)
    {
        Color lineColor = new Color(100, 100, 100);

        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startYHeader), lineColor);
            line[1] = new Vertex(new Vector2f(x, startYHeader + headerRowHeight), lineColor);
            target.Draw(line);
        }

        var topLine = new VertexArray(PrimitiveType.Lines, 2);
        topLine[0] = new Vertex(new Vector2f(startX, startYHeader), lineColor);
        topLine[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, startYHeader), lineColor);
        target.Draw(topLine);

        var bottomLine = new VertexArray(PrimitiveType.Lines, 2);
        bottomLine[0] = new Vertex(new Vector2f(startX, startYHeader + headerRowHeight), lineColor);
        bottomLine[1] = new Vertex(new Vector2f(startX + cols * columnSpacing, startYHeader + headerRowHeight), lineColor);
        target.Draw(bottomLine);
    }

    static void DrawGridData(RenderTarget target, float startX, float startYData, float rowHeight, float columnSpacing, int totalRows, int cols, float scrollOffset, float scrollableAreaHeight)
    {
        Color lineColor = new Color(100, 100, 100);

        for (int j = 0; j <= cols; j++)
        {
            var line = new VertexArray(PrimitiveType.Lines, 2);
            float x = startX + j * columnSpacing;
            line[0] = new Vertex(new Vector2f(x, startYData), lineColor);
            line[1] = new Vertex(new Vector2f(x, startYData + scrollableAreaHeight), lineColor);
            target.Draw(line);
        }
        for (int i = 0; i <= totalRows; i++)
        {
            float y = startYData + i * rowHeight + scrollOffset;

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