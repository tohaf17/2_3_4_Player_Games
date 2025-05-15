

// === MAIN PROGRAM ===
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
namespace k;
class Program
{
    public enum GameState
    {
        MainMenu,
        ChooseGame,
        Tank
    }

    public static int[,] Map1 =
    {
        { 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,4, 4, 1 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1 }

    };
    public static readonly int[,] Map2 = Map1; // тимчасово дублюємо
    public static readonly int[,] Map3 = Map1; // можна задати іншу

    static RenderWindow window;
    static GameState currentState = GameState.MainMenu;
    static Font font;
    static ButtonManagerMainMenu mainMenu;
    static ButtonManagerChooseGame chooseGameMenu;
    static Clock clock;

    static void Main(string[] args)
    {
        window = new RenderWindow(VideoMode.DesktopMode, "SFML Game", Styles.Titlebar | Styles.Close);
        window.SetFramerateLimit(60);
        window.Closed += (_, __) => window.Close();

        // шлях до Content
        string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Content");

        font = new Font(Path.Combine(assetsPath, "Lato-Regular.ttf"));
        clock = new Clock();

        mainMenu = new ButtonManagerMainMenu(font, window.Size.X, window.Size.Y);
        var tankTex = new Texture(Path.Combine(assetsPath, "red_tank.png"));
        var chickenTex = new Texture(Path.Combine(assetsPath, "chicken.png"));
        chooseGameMenu = new ButtonManagerChooseGame(new[] { tankTex, chickenTex }, window.Size.X, window.Size.Y);

        window.MouseButtonPressed += OnMousePressed;

        while (window.IsOpen)
        {
            var dt = clock.Restart();
            window.DispatchEvents();
            window.Clear(Color.Black);

            var mouse = Mouse.GetPosition(window);
            if (currentState == GameState.MainMenu)
            {
                mainMenu.Update(mouse, false);
                mainMenu.Draw(window);
            }
            else if (currentState == GameState.ChooseGame)
            {
                chooseGameMenu.Update(mouse, false);
                chooseGameMenu.Draw(window);
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
            currentState = GameState.ChooseGame;
        }
        else if (currentState == GameState.ChooseGame && chooseGameMenu.Update(mouse, true))
        {
            if (chooseGameMenu.SelectedGame == 0)
            {
                var levels = new List<Level>
                    {
                        new Level(Map1),
                        new Level(Map2),
                        new Level(Map3)
                    };
                var selector = new RandomMapSelector(levels);
                var tournament = new Tournament(selector,
                                    "C: \\Users\\ADMIN\\OneDrive\\Desktop\\Course_Work\\bin\\Content",
                                    mainMenu.SelectedPlayers);
                tournament.Start(window);
            }
            currentState = GameState.MainMenu;
        }
    }
}

