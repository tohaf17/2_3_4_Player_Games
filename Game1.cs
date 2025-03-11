using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Game
{
    public enum GameState
    {
        MainMenu,
        Game
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private Texture2D _buttonTexture;
        private ButtonManager _buttonManager;

        private GameState _gameState;
        private Tank_Map _gamePlay;
        private int _selectedPlayers = 0; // Змінна для збереження кількості гравців

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _gameState = GameState.MainMenu;
        }

        protected override void Initialize()
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

            _buttonManager = new ButtonManager(_font, _buttonTexture, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (_gameState == GameState.MainMenu)
            {
                _buttonManager.Update(mouseState);
                
                if (_buttonManager.SelectedPlayers > 0) 
                {
                    _selectedPlayers = _buttonManager.SelectedPlayers; // Зберігаємо вибір
                    _gamePlay = new Tank_Map(this, _selectedPlayers);  // Передаємо в гру кількість гравців
                    _gameState = GameState.Game; // Перемикаємо стан гри
                }
            }
            else if (_gameState == GameState.Game)
            {
                _gamePlay.Update(gameTime); // Оновлюємо гру
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (_gameState == GameState.MainMenu)
            {
                _buttonManager.Draw(_spriteBatch);
            }
            else if (_gameState == GameState.Game)
            {
                _gamePlay.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
