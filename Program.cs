

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

    static RenderWindow window;
    static GameState currentState = GameState.MainMenu;
    static TankGame tankGame;
    static Clock clock;
    static Time deltaTime;
    static ButtonManagerMainMenu mainMenu;
    static ButtonManagerChooseGame chooseGameMenu;

    static Font font;
    static Texture tankTexture;
    static Texture chickenTexture;

    static string pathContent = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Content"));


    static void Main(string[] args)
    {
        VideoMode desktopMode = VideoMode.DesktopMode;
        window = new RenderWindow(desktopMode, "SFML Game", Styles.Titlebar | Styles.Close);



        window.SetFramerateLimit(60);
        window.Closed += (_, __) => window.Close();

        font = new Font(Path.Combine(pathContent,"Lato-Regular.ttf"));
        tankTexture = new Texture(Path.Combine(pathContent,"red_tank.png"));
        chickenTexture = new Texture(Path.Combine(pathContent, "elf.png"));
        clock=new Clock();
        mainMenu = new ButtonManagerMainMenu(font, window.Size.X, window.Size.Y);
        chooseGameMenu = new ButtonManagerChooseGame(new Texture[] { tankTexture, chickenTexture }, window.Size.X, window.Size.Y);

        window.MouseButtonPressed += OnMousePressed;

        while (window.IsOpen)
        {
            deltaTime = clock.Restart();
            window.DispatchEvents();
            window.Clear(Color.Black);

            Vector2i mouse = Mouse.GetPosition(window);

            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenu.Update(mouse, false);
                    mainMenu.Draw(window);
                    break;

                case GameState.ChooseGame:
                    chooseGameMenu.Update(mouse, false);
                    chooseGameMenu.Draw(window);
                    break;

                case GameState.Tank:
                    tankGame.Update(deltaTime,window);
                    tankGame.Draw(window);
                    break;
            }

            window.Display();
        }
    }

    static void OnMousePressed(object sender, MouseButtonEventArgs e)
    {
        if (e.Button != Mouse.Button.Left) return;

        Vector2i mouse = Mouse.GetPosition(window);

        if (currentState == GameState.MainMenu && mainMenu.Update(mouse, true))
        {
            currentState = GameState.ChooseGame;
        }
        else if (currentState == GameState.ChooseGame && chooseGameMenu.Update(mouse, true))
        {
            
            try
            {
                if (chooseGameMenu.SelectedGame == 0)
                {
                    tankGame = new TankGame(pathContent, mainMenu.SelectedPlayers);
                    currentState = GameState.Tank;// Launch tank game here
                }
                else
                {
                    currentState = GameState.MainMenu;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при створенні гри: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            
        }
    }
}
