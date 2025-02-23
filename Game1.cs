using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Game;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Tank_Movement tank;
    private Texture2D gray_block;
    private Texture2D dark_block;
    private Texture2D delete;
    private Texture2D pixel;
    
    private Texture2D tankTexture;
    private Vector2 tankPosition;


    private int[,] map =
    {
        { 1, 1, 1, 1, 1, 1, 1 ,1,1,1,1,1,1,1,1,1},
        { 1, 0, 0, 0, 0, 0, 0 ,0,0,0,0,0,0,0,0,1},
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1 },
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1},
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1 },
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1 },
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1 },
        { 1, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0,0,1 },
        { 1, 1, 1, 1, 1, 1, 1 ,1,1,1,1,1,1,1,1,1}
    };

    private int tile_size = 64;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.IsFullScreen = false; 
        Window.IsBorderless = true;
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.ApplyChanges();
        
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        gray_block = Content.Load<Texture2D>("gray_block");
        dark_block = Content.Load<Texture2D>("dark_block");
        delete = Content.Load<Texture2D>("delete");
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Black });
        // TODO: use this.Content to load your game content here
        
        tankTexture = Content.Load<Texture2D>("yellow_tank");
        tank=new Tank_Movement(tankTexture, new Vector2(650, 350)); // Початкове розташування танка

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        

        base.Update(gameTime);
        tank.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(53, 53, 53));

        _spriteBatch.Begin();
        
    
        
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Texture2D texture = map[y, x] == 1 ? dark_block : gray_block;
                _spriteBatch.Draw(texture, new Vector2(x * tile_size+500, y * tile_size+250), Color.White);
            }
        }

        tank.Draw(_spriteBatch);

        _spriteBatch.End();
        

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}