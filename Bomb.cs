using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace  My_Game;
public class Bomb
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 direction;
    private float speed = 5f; // Швидкість польоту бомби
    private bool isActive = true; // Чи активна бомба

    public Bomb(Texture2D texture, Vector2 startPosition, float rotation)
    {
        this.texture = texture;
        this.position = startPosition;
        this.direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
    }

    public void Update(int[,] map, int tileSize, Tank[] tanks)
    {
        if (!isActive) return;

        position += direction * speed;

        // Перевіряємо зіткнення з картою
        if (CollidesWithMap(map, tileSize))
        {
            isActive = false; // Бомба зникає
        }

        // Перевіряємо зіткнення з танками
        foreach (var tank in tanks)
        {
            if (Intersects(tank))
            {
                tank.SetDestroyed(); // Танк вибухає
                isActive = false;
                break;
            }
        }
    }

    private bool CollidesWithMap(int[,] map, int tileSize)
    {
        int tileX = (int)position.X / tileSize;
        int tileY = (int)position.Y / tileSize;

        return map[tileY, tileX] != 0; // Якщо осередок не порожній – зіткнення
    }

    private bool Intersects(Tank tank)
    {
        Rectangle bombRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        Rectangle tankRect = new Rectangle((int)tank.Position.X, (int)tank.Position.Y, tank.Texture.Width, tank.Texture.Height);

        return bombRect.Intersects(tankRect);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (isActive)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }

    public bool IsActive => isActive;
}