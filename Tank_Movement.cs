using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace My_Game
{
<<<<<<< HEAD
    public class Tank:Player,IMovement
=======
    public class Tank : Player
>>>>>>> 45861ca (Bomb -update 1)
    {
        private Texture2D originalTexture;
        private Texture2D texture;
        private Texture2D grey;
        private Texture2D bombTexture;

        private Vector2 startPosition;
        private Vector2 position;
        private float speed = 3.5f;
        private float rotationSpeed = 3.5f;
        private float rotation = 0f; 
        private Vector2 origin;

        private Keys key;
        private Color[] textureData;
<<<<<<< HEAD
        
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
=======

        private float bombCooldown = 0f;
        private const float BombDelay = 3f;

        public Bomb bomb { get; private set; }

        private bool isDestroyed = false;

        public string color;

        public Tank(Texture2D texture, Vector2 startPosition, Keys key, Texture2D greyTexture, Texture2D bombTexture)
        {
<<<<<<< HEAD
            this.texture = texture ?? throw new ArgumentNullException(nameof(texture));
            this.grey = greyTexture ?? throw new ArgumentNullException(nameof(greyTexture));
            this.bombTexture = bombTexture ?? throw new ArgumentNullException(nameof(bombTexture));
>>>>>>> 45861ca (Bomb -update 1)
=======
            this.originalTexture = texture;
            this.texture = texture;
            this.grey = greyTexture;
            this.bombTexture = bombTexture;
>>>>>>> c133d8d (Bombs.Update2)
            this.key = key;
            this.startPosition = startPosition;
            this.position = startPosition;

            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
        }
<<<<<<< HEAD
        
        public void Movement(Player[] tanks, int[,] map, int tileSize,Game1 game)
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 new_position = position;
            
            dict = new Dictionary<Tank, Bomb>();
=======

        public void Update(Tank[] tanks, int[,] map, int tileSize, Game1 game, GameTime gameTime)
        {
            if (isDestroyed) return;

            bombCooldown = Math.Max(0f, bombCooldown - (float)gameTime.ElapsedGameTime.TotalSeconds);
            bomb?.Update(map, tileSize, tanks);
            Movement(tanks, map, tileSize);
        }

        private void Movement(Tank[] tanks, int[,] map, int tileSize)
        {
            KeyboardState state = Keyboard.GetState();
>>>>>>> 45861ca (Bomb -update 1)


            if (state.IsKeyDown(key))
            {
                Vector2 direction = Move(new Vector2(0, -1), rotation);
                Vector2 newPosition = position - direction * speed;

                foreach (var otherTank in tanks)
                {
<<<<<<< HEAD
                    if (otherTank != null && otherTank != this && Intersects((Tank)otherTank, new_position))
                    {
                        return; 
                    }
                }

                position = new_position;
                CollidesWithMap(map, tileSize);
=======
                    if (otherTank != null && otherTank != this && Intersects(otherTank, newPosition))
                        return;
                }

                position = newPosition;
                ClampToMap();

                if ((bomb == null || !bomb.IsActive) && bombCooldown <= 0f)
                {
                    Vector2 shootDirection = -Move(new Vector2(0, -1), rotation);
                    float spawnOffset = (texture.Height / 2f) * (64f / texture.Width);
                    Vector2 bombSpawn = position + shootDirection * spawnOffset;

                    bomb = new Bomb(bombTexture, bombSpawn, shootDirection, rotation, this);
                    bombCooldown = BombDelay;
                }
<<<<<<< HEAD
>>>>>>> 45861ca (Bomb -update 1)

=======
>>>>>>> c133d8d (Bombs.Update2)
            }
            else
            {
                rotation += MathHelper.ToRadians(rotationSpeed);
            }
        }
<<<<<<< HEAD
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
=======

        private void ClampToMap()
        {
            position.X = MathHelper.Clamp(position.X, 596, 1428);
            position.Y = MathHelper.Clamp(position.Y, 346, 730);
>>>>>>> 45861ca (Bomb -update 1)
        }


        public bool Intersects(Tank other, Vector2 newPos)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            int scaledWidth = (int)(64f / texture.Width * texture.Width)-25;
            int scaledHeight = (int)(64f / texture.Height * texture.Height)-25;

            Rectangle rectA = new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), scaledWidth, scaledHeight);
            Rectangle rectB = new Rectangle((int)(other.position.X - other.origin.X), (int)(other.position.Y - other.origin.Y), scaledWidth, scaledHeight);
=======
            int scaledSize = 64;
            Rectangle rectA = new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), scaledSize, scaledSize);
            Rectangle rectB = new Rectangle((int)(other.position.X - other.origin.X), (int)(other.position.Y - other.origin.Y), scaledSize, scaledSize);
>>>>>>> 45861ca (Bomb -update 1)

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
                        return true;
                }
            }
            return false;
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

        private bool IsInsideTexture(Tank tank, Vector2 localPos)
        {
            return localPos.X >= 0 && localPos.X < tank.texture.Width &&
                   localPos.Y >= 0 && localPos.Y < tank.texture.Height;
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
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
            bomb?.Draw(spriteBatch);
=======
            Rectangle rectA = new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), 64, 64);
            Rectangle rectB = new Rectangle((int)(other.Position.X - origin.X), (int)(other.Position.Y - origin.Y), 64, 64);
            return rectA.Intersects(rectB);
>>>>>>> c133d8d (Bombs.Update2)
        }

        private Vector2 Move(Vector2 point, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vector2(
                cos * point.X - sin * point.Y,
                sin * point.X + cos * point.Y
            );
        }
        
        

<<<<<<< HEAD
=======
        public void SetDestroyed()
        {
            texture = grey;
            isDestroyed = true;
        }

        public void Reset()
        {
            texture = originalTexture;
            isDestroyed = false;
            position = startPosition;
            rotation = 0f;
            bombCooldown = 0f;
            bomb = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
            bomb?.Draw(spriteBatch);
        }

        public Vector2 Position => position;
        public Texture2D Texture => texture;
        public bool IsDestroyed => isDestroyed;
>>>>>>> c133d8d (Bombs.Update2)
    }
}
