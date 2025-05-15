using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace k
{
    public class TankGame
    {
        private readonly List<GameEntity> entities = new();
        private readonly int[,] map;
        private const int tileSize = 64;
        private Vector2f offset;

        private readonly MapCollider mapCollider;
        private readonly MapRenderer renderer;

        public IEnumerable<GameEntity> Entities => entities;
        public bool IsGameOver() => entities.OfType<Tank>().Count(t => t.IsAlive) <= 1;

        public TankGame(int[,] map, string assetsPath, int playerCount)
        {
            this.map = map;

            // початковий Offset для 1280×720
            offset = new Vector2f(
                (1280 - tileSize * map.GetLength(1)) / 2f,
                (720 - tileSize * map.GetLength(0)) / 2f
            );

            renderer = new MapRenderer(map, tileSize, offset, assetsPath);
            mapCollider = new MapCollider(map, offset, tileSize);

            // завантажуємо текстури

            assetsPath = "C:\\Users\\ADMIN\\OneDrive\\Desktop\\Course_Work\\bin\\Content";
            var redTankTex = new Texture(Path.Combine(assetsPath, "red_tank.png"));
            var blueTankTex = new Texture(Path.Combine(assetsPath, "blue_tank.png"));
            var greenTankTex = new Texture(Path.Combine(assetsPath, "green_tank.png"));
            var yellowTankTex = new Texture(Path.Combine(assetsPath, "yellow_tank.png"));
            var destroyedTex = new Texture(Path.Combine(assetsPath, "gray_tank.png"));

            var redBombTex = new Texture(Path.Combine(assetsPath, "red_bomb.png"));
            var blueBombTex = new Texture(Path.Combine(assetsPath, "blue_bomb.png"));
            var greenBombTex = new Texture(Path.Combine(assetsPath, "green_bomb.png"));
            var yellowBombTex = new Texture(Path.Combine(assetsPath, "yellow_bomb.png"));

            if (playerCount >= 2)
            {
                GameEntity redTank = new Tank(redTankTex, new Vector2f(628, 378), Keyboard.Key.Q, destroyedTex, redBombTex, mapCollider);
                ((Tank)redTank).Data.Color = "Red";
                entities.Add(redTank);

                GameEntity blueTank = new Tank(blueTankTex, new Vector2f(1396, 698), Keyboard.Key.M, destroyedTex, blueBombTex, mapCollider);
                ((Tank)blueTank).Data.Color = "Blue";
                entities.Add(blueTank);
            }
            if (playerCount >= 3)
            {
                GameEntity greenTank = new Tank(greenTankTex, new Vector2f(1396, 378), Keyboard.Key.Numpad9, destroyedTex, greenBombTex, mapCollider);
                ((Tank)greenTank).Data.Color = "Green";
                entities.Add(greenTank);
            }
            if (playerCount >= 4)
            {
                GameEntity yellowTank = new Tank(yellowTankTex, new Vector2f(628, 698), Keyboard.Key.V, destroyedTex, yellowBombTex, mapCollider);
                ((Tank)yellowTank).Data.Color = "Yellow";
                entities.Add(yellowTank);
            }
        }

        public void Update(Time deltaTime, RenderWindow window)
        {
            // оновлюємо Offset під поточний розмір вікна
            offset = new Vector2f(
                (window.Size.X - tileSize * map.GetLength(1)) / 2f,
                (window.Size.Y - tileSize * map.GetLength(0)) / 2f
            );
            renderer.UpdateOffset(offset);
            mapCollider.Offset = offset;

            // апдейтимо танки
            foreach (var t in entities.OfType<Tank>())
                t.Update(deltaTime, entities, map, offset);
        }

        public void Draw(RenderWindow window)
        {
            renderer.Draw(window);
            foreach (var e in entities)
                e.Draw(window);
        }
    }
}
 