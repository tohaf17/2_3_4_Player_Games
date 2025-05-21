using SFML.Graphics;
using static k.Constants;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace k
{
    public class TankGame
    {
        private readonly List<GameEntity> entities = new();
        private readonly Vector2u screenSize;

        private readonly MapRenderer renderer;
        private readonly MapCollider collider;
        private Random random = new();


        public TankGame((string,string) level, int playerCount, RenderWindow window)
        {
            screenSize = window.Size;
            renderer = new MapRenderer(level);
            collider = new MapCollider(renderer.SpritesWall,renderer.SpritesBox);

            // Завантаження текстур танків і бомб
            var redTankTex = new Texture(Path.Combine(AssetsPath, "red_tank.png"));
            var blueTankTex = new Texture(Path.Combine(AssetsPath, "blue_tank.png"));
            var greenTankTex = new Texture(Path.Combine(AssetsPath, "green_tank.png"));
            var yellowTankTex = new Texture(Path.Combine(AssetsPath, "yellow_tank.png"));
            var destroyedTex = new Texture(Path.Combine(AssetsPath, "gray_tank.png"));

            var redBombTex = new Texture(Path.Combine(AssetsPath, "red_bomb.png"));
            var blueBombTex = new Texture(Path.Combine(AssetsPath, "blue_bomb.png"));
            var greenBombTex = new Texture(Path.Combine(AssetsPath, "green_bomb.png"));
            var yellowBombTex = new Texture(Path.Combine(AssetsPath, "yellow_bomb.png"));

            // Створення танків (позиції довільні)
            if (playerCount >= 2)
            {
                var red = new Tank(collider,redTankTex, new Vector2f(random.Next(100,1800),random.Next(100,600)), Keyboard.Key.Q, destroyedTex, redBombTex,screenSize);
                var blue = new Tank(collider,blueTankTex, new Vector2f(random.Next(100, 1800), random.Next(100, 600)), Keyboard.Key.M, destroyedTex, blueBombTex, screenSize);
                red.Data.Color = "Red";
                blue.Data.Color = "Blue";
                entities.Add(red);
                entities.Add(blue);
            }
            if (playerCount >= 3)
            {
                var green = new Tank(collider, greenTankTex, new Vector2f(random.Next(100, 1800), random.Next(100, 600)), Keyboard.Key.Numpad9, destroyedTex, greenBombTex, screenSize);
                green.Data.Color = "Green";
                entities.Add(green);
            }
            if (playerCount >= 4)
            {
                var yellow = new Tank(collider, yellowTankTex, new Vector2f(random.Next(100, 1800), random.Next(100, 600)), Keyboard.Key.V, destroyedTex, yellowBombTex, screenSize);
                yellow.Data.Color = "Yellow";
                entities.Add(yellow);
            }
        }

        public void Update(Time deltaTime, RenderWindow window)
        {
            foreach (var entity in entities.OfType<Tank>())
                entity.Update(deltaTime, entities);
        }

        public void DrawMap(RenderWindow window)
        {
            renderer.Draw(window);
        }


        public void DrawEntities(RenderWindow window)
        {
            foreach (var entity in entities)
                entity.Draw(window);
        }

        public bool IsGameOver() => entities.OfType<Tank>().Count(t => t.IsAlive) <= 1;
        public IEnumerable<GameEntity> Entities => entities;
        public MapRenderer MapRenderer => renderer;
    }
}
