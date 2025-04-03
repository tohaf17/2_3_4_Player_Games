using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        private Texture2D greyTexture;
        
        private Tank red_tank;
        private Tank blue_tank;
        private Tank green_tank;
        private Tank yellow_tank;
        

        private Texture2D yellowBombTexture;
        private Texture2D greenBombTexture;
        private Texture2D blueBombTexture;
        private Texture2D redBombTexture;
        
        private Game1 _game;

        private int _numPlayers; 

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
            _game = game;
            gray_block = game.Content.Load<Texture2D>("gray_block");
            dark_block = game.Content.Load<Texture2D>("dark_block");
            red = game.Content.Load<Texture2D>("red_tank");
            blue = game.Content.Load<Texture2D>("blue_tank");
            green = game.Content.Load<Texture2D>("green_tank");
            yellow = game.Content.Load<Texture2D>("yellow_tank");

            greyTexture = _game.Content.Load<Texture2D>("gray_tank");

            // Завантаження текстур бомб
            redBombTexture = _game.Content.Load<Texture2D>("red_bomb");
            blueBombTexture = _game.Content.Load<Texture2D>("blue_bomb");
            greenBombTexture = _game.Content.Load<Texture2D>("green");
            yellowBombTexture = _game.Content.Load<Texture2D>("yellow_bomb");
            StartGame(); 
        }

        private void StartGame()
        {
            

            if (_numPlayers >= 2)
            {
                red_tank = new Tank(red, new Vector2(628, 378), Keys.Q, greyTexture, redBombTexture); // Передача текстури бомби
                Console.WriteLine($"Red bomb texture is null: {redBombTexture == null}");
                blue_tank = new Tank(blue, new Vector2(1396, 698), Keys.M, greyTexture, blueBombTexture); // Передача текстури бомби
            }

            if (_numPlayers >= 3)
            {
                green_tank = new Tank(green, new Vector2(1396, 378), Keys.NumPad9, greyTexture, greenBombTexture); // Передача текстури бомби
            }

            if (_numPlayers == 4)
            {
                yellow_tank = new Tank(yellow, new Vector2(628, 698), Keys.V, greyTexture, yellowBombTexture); // Передача текстури бомби
            }
        }

        public void Update(GameTime gameTime)
        {
            Tank[] tanks = { red_tank, blue_tank, green_tank, yellow_tank };

            foreach (var tank in tanks)
            {
                if (tank != null)
                {
                    tank.Update(tanks, map, tile_size, _game, gameTime);
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Texture2D texture = map[y, x] == 1 ? dark_block : gray_block;
                    spriteBatch.Draw(texture, new Vector2(x * tile_size + 500, y * tile_size + 250), Color.White);
                }
            }

            red_tank?.Draw(spriteBatch);
            blue_tank?.Draw(spriteBatch);
            green_tank?.Draw(spriteBatch);
            yellow_tank?.Draw(spriteBatch);
            
            red_tank?.bomb?.Draw(spriteBatch); 
            blue_tank?.bomb?.Draw(spriteBatch);
            green_tank?.bomb?.Draw(spriteBatch);
            yellow_tank?.bomb?.Draw(spriteBatch);
        }
    }
}
