using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Game
{
    public class Tank_Map
    {
        private Texture2D gray_block;
        private Texture2D dark_block;
        
        private Texture2D yellow;
        private Texture2D green;
        private Texture2D blue;
        private Texture2D red;
        
        private Tank_Movement red_tank;
        private Tank_Movement blue_tank;
        private Tank_Movement green_tank;
        private Tank_Movement yellow_tank;

        private int _numPlayers; // Кількість гравців

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

        public Tank_Map(Game1 game, int numPlayers)
        {
            _numPlayers = numPlayers;

            gray_block = game.Content.Load<Texture2D>("gray_block");
            dark_block = game.Content.Load<Texture2D>("dark_block");
            red = game.Content.Load<Texture2D>("red_tank");
            blue = game.Content.Load<Texture2D>("blue_tank");
            green = game.Content.Load<Texture2D>("green_tank");
            yellow = game.Content.Load<Texture2D>("yellow_tank");

            StartGame(); 
        }

        private void StartGame()
        {
            if (_numPlayers >= 2)
            {
                red_tank = new Tank_Movement(red, new Vector2(628, 378), Keys.Q);
                blue_tank = new Tank_Movement(blue, new Vector2(1396, 698), Keys.M);
            }

            if (_numPlayers >= 3)
            {
                green_tank = new Tank_Movement(green, new Vector2(1396, 378), Keys.NumPad9);
            }

            if (_numPlayers == 4)
            {
                yellow_tank = new Tank_Movement(yellow, new Vector2(628, 698), Keys.V);
            }
        }

        public void Update(GameTime gameTime) // Додаємо параметр gameTime
        {
            red_tank?.Update();
            blue_tank?.Update();
            green_tank?.Update();
            yellow_tank?.Update();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // Малювання карти
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Texture2D texture = map[y, x] == 1 ? dark_block : gray_block;
                    spriteBatch.Draw(texture, new Vector2(x * tile_size + 500, y * tile_size + 250), Color.White);
                }
            }

            // Малювання танків (якщо вони є)
            red_tank?.Draw(spriteBatch);
            blue_tank?.Draw(spriteBatch);
            green_tank?.Draw(spriteBatch);
            yellow_tank?.Draw(spriteBatch);
        }
    }
}
