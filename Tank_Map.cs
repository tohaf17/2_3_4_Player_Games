using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
<<<<<<< HEAD
=======
using System;
using System.Collections.Generic;
using System.Linq;
>>>>>>> c133d8d (Bombs.Update2)

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
<<<<<<< HEAD
        
        private Tank red_tank;
        private Tank blue_tank;
        private Tank green_tank;
        private Tank yellow_tank;
        
        private Game1 _game;
=======
        private Texture2D greyTexture;

        private Texture2D yellowBombTexture;
        private Texture2D greenBombTexture;
        private Texture2D blueBombTexture;
        private Texture2D redBombTexture;
>>>>>>> c133d8d (Bombs.Update2)

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
        private int gameCount = 0;
        private const int maxGames = 5;

        private List<Tank> tanks = new();

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
            greyTexture = game.Content.Load<Texture2D>("gray_tank");

<<<<<<< HEAD
            StartGame(); 
=======
            redBombTexture = game.Content.Load<Texture2D>("red_bomb");
            blueBombTexture = game.Content.Load<Texture2D>("blue_bomb");
            greenBombTexture = game.Content.Load<Texture2D>("green");
            yellowBombTexture = game.Content.Load<Texture2D>("yellow_bomb");

            StartGame();
>>>>>>> c133d8d (Bombs.Update2)
        }

        private void StartGame()
        {
<<<<<<< HEAD
            if (_numPlayers >= 2)
            {
                red_tank = new Tank(red, new Vector2(628, 378), Keys.Q);
                blue_tank = new Tank(blue, new Vector2(1396, 698), Keys.M);
                
=======
            tanks.Clear();

            if (_numPlayers >= 2)
            {
                var redTank = new Tank(red, new Vector2(628, 378), Keys.Q, greyTexture, redBombTexture) { color = "Red" };
                var blueTank = new Tank(blue, new Vector2(1396, 698), Keys.M, greyTexture, blueBombTexture) { color = "Blue" };
                tanks.Add(redTank);
                tanks.Add(blueTank);
>>>>>>> c133d8d (Bombs.Update2)
            }

            if (_numPlayers >= 3)
            {
<<<<<<< HEAD
                green_tank = new Tank(green, new Vector2(1396, 378), Keys.NumPad9);
=======
                var greenTank = new Tank(green, new Vector2(1396, 378), Keys.NumPad9, greyTexture, greenBombTexture) { color = "Green" };
                tanks.Add(greenTank);
>>>>>>> c133d8d (Bombs.Update2)
            }

            if (_numPlayers == 4)
            {
<<<<<<< HEAD
                yellow_tank = new Tank(yellow, new Vector2(628, 698), Keys.V);
=======
                var yellowTank = new Tank(yellow, new Vector2(628, 698), Keys.V, greyTexture, yellowBombTexture) { color = "Yellow" };
                tanks.Add(yellowTank);
>>>>>>> c133d8d (Bombs.Update2)
            }
        }

        public void Update(GameTime gameTime)
        {
            if (gameCount >= maxGames)
                return;

            foreach (var tank in tanks)
            {
                tank.Update(tanks.ToArray(), map, tile_size, _game, gameTime);
            }

            int alive = tanks.Count(t => !t.IsDestroyed);

            if (alive == 1)
            {
                Tank winner = tanks.First(t => !t.IsDestroyed);
                winner.Score++;
                gameCount++;

                Console.WriteLine($"🏆 Переможець раунду {gameCount}: {winner.color}");
                if (gameCount >= maxGames)
                {
<<<<<<< HEAD
                    // Перевіряємо колізії з іншими танками та картою
                    tank.Update(tanks, map, tile_size,_game);
=======
                    Console.WriteLine("\n📊 Кінцеві очки:");
                    foreach (var t in tanks)
                        Console.WriteLine($"{t.color} — {t.Score} очок");
                }
                else
                {
                    RestartRound();
>>>>>>> c133d8d (Bombs.Update2)
                }
            }
        }

        private void RestartRound()
        {
            foreach (var tank in tanks)
            {
                tank.Reset();
            }
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

<<<<<<< HEAD
            // Малювання танків (якщо вони є)
            red_tank?.Draw(spriteBatch);
            blue_tank?.Draw(spriteBatch);
            green_tank?.Draw(spriteBatch);
            yellow_tank?.Draw(spriteBatch);
=======
            foreach (var tank in tanks)
                tank.Draw(spriteBatch);
>>>>>>> c133d8d (Bombs.Update2)
        }
    }
}
