using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace My_Game
{
    public enum GameState
    {
        MainMenu,
        Tank,
        Chicken,
        Choose_Game
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private Texture2D _buttonTexture;
        private ButtonManager_MainMenu _buttonManagerMainMenu;
        private ButtonManager_ChooseGame _buttonManagerChooseGame;

        private GameState _gameState;
        private Tank_Map _gamePlay;
        private int _selectedPlayers = 0;

        private Texture2D tank;
        private Texture2D chicken;

        public Game1() //конструктор ігри основної
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _gameState = GameState.MainMenu;
            
            
        }

        protected override void Initialize() //записуємо парметри екрану
        {
            _graphics.IsFullScreen = false; 
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("myfont");

            _buttonTexture = new Texture2D(GraphicsDevice, 1, 1);
            _buttonTexture.SetData(new[] { Color.White });

            tank = Content.Load<Texture2D>("red_tank");
            chicken = Content.Load<Texture2D>("blue_tank"); // ПЕРЕРОБИТИ НА ЧІКЕН

            _buttonManagerMainMenu = new ButtonManager_MainMenu(_font, _buttonTexture, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _buttonManagerChooseGame = new ButtonManager_ChooseGame(_buttonTexture, new Texture2D[] { tank, chicken }, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // Підписуємося на події зміни стану
            _buttonManagerMainMenu.PlayersSelected += OnPlayersSelected;
            _buttonManagerChooseGame.GameSelected += OnGameSelected;
        }

        private void OnPlayersSelected(int players)
        {
            _selectedPlayers = players;
            _gameState = GameState.Choose_Game;
            _buttonManagerChooseGame?.ResetSelectedGame();
        }

        private void OnGameSelected(int selectedGame)
        {
            switch (selectedGame)
            {
                case 0: // Tank
                    try
                    {
                        _gamePlay = new Tank_Map(this, _selectedPlayers);
                        _gameState = GameState.Tank;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка при створенні Tank_Map: {ex.Message}");
                        _gameState = GameState.MainMenu;
                    }
                    break;
                case 1: // Chicken
                    // try
                    // {
                    //     //_gamePlay = new Chicken(this, _selectedPlayers);
                    //     _gameState = GameState.Chicken;
                    // }
                    // catch (Exception ex)
                    // {
                    //     Console.WriteLine($"Помилка при створенні Chicken: {ex.Message}");
                    //     _gameState = GameState.MainMenu;
                    // }
                    break;
                default:
                    _gameState = GameState.MainMenu;
                    break;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            switch (_gameState)
            {
                case GameState.MainMenu:
                    _buttonManagerMainMenu.Update(mouseState);
                    break;
                case GameState.Choose_Game:
                    _buttonManagerChooseGame?.Update(mouseState);
                    break;
                case GameState.Tank:
                    _gamePlay?.Update(gameTime);
                    break;
                // case GameState.Chicken:
                //     _gamePlay?.Update(gameTime);
                //     break;
                default:
                    _gameState = GameState.MainMenu;
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_gameState)
            {
                case GameState.MainMenu:
                    _buttonManagerMainMenu.Draw(_spriteBatch);
                    break;
                case GameState.Choose_Game:
                    _buttonManagerChooseGame.Draw(_spriteBatch);
                    break;
                case GameState.Tank:
                    _gamePlay?.Draw(_spriteBatch);
                    break;
                // case GameState.Chicken:
                //     _gamePlay?.Draw(_spriteBatch);
                //     break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
