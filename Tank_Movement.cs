using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace My_Game
{
    public class Tank:Player,IMovement
    {
        private Texture2D texture;
        public Texture2D Texture { get; set; }
        private Vector2 position;
        public Vector2 Position { get; set; }
        private float speed = 3.5f;
        private float rotationSpeed = 3.5f;
        private float rotation = 0f; 
        private Vector2 origin;
        private Vector2 barrelTip = new Vector2(0, -10);
        private Keys key;
        private Color[] textureData;
        
        private Dictionary<Tank, Bomb> dict;
        
        private Texture2D yellow;
        private Texture2D green;
        private Texture2D blue;
        private Texture2D red;

        private readonly int minX=  596;
        private readonly int maxX = 1428;
        private readonly int minY = 346;
        private readonly int maxY = 730;

        public Tank(Texture2D texture, Vector2 startPosition, Keys key)
        {
            this.texture = texture;
            position = startPosition;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.key = key;
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
        }
        
        public void Movement(Player[] tanks, int[,] map, int tileSize,Game1 game)
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 new_position = position;
            
            dict = new Dictionary<Tank, Bomb>();


            if (state.IsKeyDown(key))
            {
                Vector2 direction = Move(new Vector2(0, -1), rotation);
                new_position = position - direction * speed;

                foreach (var otherTank in tanks)
                {
                    if (otherTank != null && otherTank != this && Intersects((Tank)otherTank, new_position))
                    {
                        return; 
                    }
                }

                position = new_position;
                CollidesWithMap(map, tileSize);

            }
            else
            {
                rotation += MathHelper.ToRadians(rotationSpeed);
            }
        }
        public void Update(Tank[] tanks, int[,] map, int tileSize,Game1 game)
        {
            Movement(tanks, map, tileSize, game);
        }


        private void CollidesWithMap(int[,] map, int tileSize)
        {
            if (position.X < minX)
            {
                position .X = minX;
            }
            if (position .X > maxX)
            {
                position .X = maxX;
            }
            if (position .Y < minY)
            {
                position .Y = minY;
            }
            if (position .Y > maxY)
            {
                position .Y = maxY;
            }
        }


        public bool Intersects(Tank other, Vector2 newPos)
        {
            int scaledWidth = (int)(64f / texture.Width * texture.Width)-25;
            int scaledHeight = (int)(64f / texture.Height * texture.Height)-25;

            Rectangle rectA = new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), scaledWidth, scaledHeight);
            Rectangle rectB = new Rectangle((int)(other.position.X - other.origin.X), (int)(other.position.Y - other.origin.Y), scaledWidth, scaledHeight);

            if (!rectA.Intersects(rectB))
                return false;

            return PixelCollision(this, other, newPos);
        }

        private bool PixelCollision(Tank a, Tank b, Vector2 newPosA)
        {
            int top = Math.Max((int)newPosA.Y, (int)b.position.Y);
            int bottom = Math.Min((int)newPosA.Y + a.texture.Height, (int)b.position.Y + b.texture.Height);
            int left = Math.Max((int)newPosA.X, (int)b.position.X);
            int  right = Math.Min((int)newPosA.X + a.texture.Width, (int)b.position.X + b.texture.Width);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Vector2 localA = GlobalToLocal(a, new Vector2(x, y), newPosA);
                    Vector2 localB = GlobalToLocal(b, new Vector2(x, y), b.position);

                    if (!IsInsideTexture(a, localA) || !IsInsideTexture(b, localB))
                        continue;

                    Color colorA = a.GetPixelAt((int)localA.X, (int)localA.Y);
                    Color colorB = b.GetPixelAt((int)localB.X, (int)localB.Y);

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInsideTexture(Tank tank, Vector2 localPos)
        {
            return localPos.X >= 0 && localPos.X < tank.texture.Width &&
                   localPos.Y >= 0 && localPos.Y < tank.texture.Height;
        }

        private Vector2 GlobalToLocal(Tank tank, Vector2 globalPos, Vector2 tankPos)
        {
            Vector2 relative = globalPos - tankPos;
            float cos = (float)Math.Cos(-tank.rotation);
            float sin = (float)Math.Sin(-tank.rotation);

            return new Vector2(
                cos * relative.X - sin * relative.Y + tank.origin.X,
                sin * relative.X + cos * relative.Y + tank.origin.Y
            );
        }

        private Color GetPixelAt(int x, int y)
        {
            if (x < 0 || x >= texture.Width || y < 0 || y >= texture.Height)
                return Color.Transparent; // Якщо координати виходять за межі

            int index = y * texture.Width + x;
            return textureData[index];
        }

        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            spriteBatch.Draw(texture, position, null, color, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
        }

        private Vector2 Move(Vector2 point, float angle)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            return new Vector2(
                cos * point.X - sin * point.Y,
                sin * point.X + cos * point.Y
            );
        }
        
        

    }
}
