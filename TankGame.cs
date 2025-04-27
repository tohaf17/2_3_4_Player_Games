using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.IO;

namespace k
{
    public class TankGame
    {
        private List<GameEntity> entities = new();
        
        private int[,] map;
        private const int tileSize = 64;
        private Vector2f offset ;

        private Texture darkBlock;
        private Texture grayBlock;
        private Texture leftDarkBlock;


        public TankGame(string pathContent, int players)
        {
            map = new int[,]
            {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // ТЕКСТУРИ
            darkBlock = new Texture(Path.Combine(pathContent, "dark_block.png"));
            grayBlock = new Texture(Path.Combine(pathContent, "gray_block.png"));
            leftDarkBlock = new Texture(Path.Combine(pathContent, "left_side_block.png"));

            var redTankTexture = new Texture(Path.Combine(pathContent, "red_tank.png"));
            var blueTankTexture = new Texture(Path.Combine(pathContent, "blue_tank.png"));
            var greenTankTexture = new Texture(Path.Combine(pathContent, "green_tank.png"));
            var yellowTankTexture = new Texture(Path.Combine(pathContent, "yellow_tank.png"));
            var destroyedTexture = new Texture(Path.Combine(pathContent, "gray_tank.png"));

            var redBombTexture = new Texture(Path.Combine(pathContent, "red_bomb.png"));
            var blueBombTexture = new Texture(Path.Combine(pathContent, "blue_bomb.png"));
            var greenBombTexture = new Texture(Path.Combine(pathContent, "green.png"));
            var yellowBombTexture = new Texture(Path.Combine(pathContent, "yellow_bomb.png"));

            // ВІДРАЗУ ОБЧИСЛЮЄМО OFFSET
            float mapWidth = tileSize * map.GetLength(1);
            float mapHeight = tileSize * map.GetLength(0);
            offset = new Vector2f(
                (1280 - mapWidth) / 2f,   // Наприклад якщо вікно 1280x720
                (720 - mapHeight) / 2f
            );

            // ТЕПЕР СТВОРЮЄМО ТАНКИ І ПЕРЕДАЄМО OFFSET
            if (players >= 2)
            {
                entities.Add(new Tank(redTankTexture, new Vector2f(628, 378), Keyboard.Key.Q, destroyedTexture, redBombTexture, map, tileSize, offset));
                entities.Add(new Tank(blueTankTexture, new Vector2f(1396, 698), Keyboard.Key.M, destroyedTexture, blueBombTexture, map, tileSize, offset));
            }
            if (players >= 3)
            {
                entities.Add(new Tank(greenTankTexture, new Vector2f(1396, 378), Keyboard.Key.Num9, destroyedTexture, greenBombTexture, map, tileSize, offset));
            }
            if (players == 4)
            {
                entities.Add(new Tank(yellowTankTexture, new Vector2f(628, 698), Keyboard.Key.V, destroyedTexture, yellowBombTexture, map, tileSize, offset));
            }
        }


        public void Update(Time deltaTime, RenderWindow window)
        {
            // 1) Оновлюємо offset по центру:
            float mapW = tileSize * map.GetLength(1);
            float mapH = tileSize * map.GetLength(0);
            offset = new Vector2f(
                (window.Size.X - mapW) / 2f,
                (window.Size.Y - mapH) / 2f
            );

            // 2) Для кожного танка викликаємо Update із актуальним offset
            foreach (var t in entities.OfType<Tank>())
                t.Update(deltaTime, entities, map, offset);
        }

        public void Draw(RenderWindow window)
        {
            // РОЗРАХУНОК НОВОГО OFFSET ПО ЦЕНТРУ
            var mapWidthInPixels = tileSize * map.GetLength(1);
            var mapHeightInPixels = tileSize * map.GetLength(0);

            offset = new Vector2f(
                (window.Size.X - mapWidthInPixels) / 2f,
                (window.Size.Y - mapHeightInPixels) / 2f
            );

            // МАЛЮЄМО ФОН
            for (int y = 0; y < window.Size.Y; y += tileSize)
            {
                for (int x = 0; x < window.Size.X; x += tileSize)
                {
                    Sprite backgroundTile = new Sprite(darkBlock)
                    {
                        Position = new Vector2f(x, y),
                        Scale = new Vector2f(tileSize / (float)darkBlock.Size.X, tileSize / (float)darkBlock.Size.Y)
                    };
                    window.Draw(backgroundTile);
                }
            }

            // МАЛЮЄМО КАРТУ
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Texture texture = map[y, x] switch
                    {
                        1 => darkBlock,
                        0 => grayBlock,
                        _ => grayBlock
                    };

                    Sprite tile = new Sprite(texture)
                    {
                        Position = new Vector2f(x * tileSize + offset.X, y * tileSize + offset.Y),
                        Scale = new Vector2f(tileSize / (float)texture.Size.X, tileSize / (float)texture.Size.Y)
                    };
                    window.Draw(tile);
                }
            }

            foreach (var entity in entities)
                entity.Draw(window);
        }

    }
}
