using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace My_Game
{
    public class Tank : Player
    {
        private Texture2D texture;
        public Texture2D Texture { get; set; }
        private Vector2 position;
        public Vector2 Position { get; set; }
        private float speed = 3.5f;
        private float rotationSpeed = 3.5f;
        private float rotation = 0f;
        private Vector2 origin;
        private Keys key;
        private Color[] textureData;

        private Texture2D grey;
        private Texture2D bombTexture;
        public Bomb bomb { get; set; }

        private float bombCooldown = 0f;
        private const float BombDelay = 3f;

        public Tank(Texture2D texture, Vector2 startPosition, Keys key, Texture2D greyTexture, Texture2D bombTexture)
        {
            this.texture = texture ?? throw new ArgumentNullException(nameof(texture));
            this.grey = greyTexture ?? throw new ArgumentNullException(nameof(greyTexture));
            this.bombTexture = bombTexture ?? throw new ArgumentNullException(nameof(bombTexture));
            this.key = key;

            position = startPosition;
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
        }

        public void Update(Tank[] tanks, int[,] map, int tileSize, Game1 game, GameTime gameTime)
        {
            bombCooldown = Math.Max(0f, bombCooldown - (float)gameTime.ElapsedGameTime.TotalSeconds);

            bomb?.Update(map, tileSize, tanks);
            Movement(tanks, map, tileSize);
        }

        public void Movement(Tank[] tanks, int[,] map, int tileSize)
        {
            KeyboardState state = Keyboard.GetState();

            // Створення бомби — без змін
            if ((bomb == null || !bomb.IsActive) && bombCooldown <= 0f && state.IsKeyDown(key))
            {
                Vector2 shootDirection = -Move(new Vector2(0, -1), rotation);
                float spawnOffset = (texture.Height / 2f) * (64f / texture.Width);
                Vector2 bombSpawn = position + shootDirection * spawnOffset;

                bomb = new Bomb(bombTexture, bombSpawn, shootDirection, rotation, this);
                bombCooldown = BombDelay;
            }

            if (state.IsKeyDown(key))
            {
                Vector2 direction = Move(new Vector2(0, -1), rotation);
                Vector2 newPosition = position - direction * speed;

                bool isBlocked = false;

                foreach (var otherTank in tanks)
                {
                    if (otherTank != null && otherTank != this && Intersects(otherTank, newPosition))
                    {
                        // 💥 Спроба штовхнути іншого
                        Vector2 push = -direction * 2f; // на 2 пікселі назад
                        Vector2 pushedPosition = otherTank.Position + push;

                        // Перевірка, чи можна штовхнути інший танк
                        bool canPush = true;
                        foreach (var t in tanks)
                        {
                            if (t != null && t != this && t != otherTank && Intersects(t, pushedPosition))
                            {
                                canPush = false;
                                break;
                            }
                        }

                        if (canPush)
                        {
                            otherTank.ForceMove(push); // ⬅️ зміщення іншого
                        }
                        else
                        {
                            isBlocked = true; // 🤷‍♂️ не можемо штовхнути
                        }
                    }
                }

<<<<<<< HEAD:Tank_Movement.cs
                position = newPosition;
                CollidesWithMap();

                if ((bomb == null || !bomb.IsActive) && bombCooldown <= 0f)
=======
                if (!isBlocked)
>>>>>>> 6516c0a (Bombs.Update3):Tank.cs
                {
                    // ще раз перевіримо, що після штовхання перетину більше нема
                    bool collisionAfterPush = false;
                    foreach (var otherTank in tanks)
                    {
                        if (otherTank != null && otherTank != this && Intersects(otherTank, newPosition))
                        {
                            collisionAfterPush = true;
                            break;
                        }
                    }

<<<<<<< HEAD:Tank_Movement.cs
                    bomb = new Bomb(bombTexture, bombSpawn, shootDirection, rotation);
                    bombCooldown = BombDelay;
=======
                    if (!collisionAfterPush)
                    {
                        position = newPosition;
                        ClampToMap();
                    }
>>>>>>> 6516c0a (Bombs.Update3):Tank.cs
                }

            }
            else
            {
                rotation += MathHelper.ToRadians(rotationSpeed);
            }
        }

<<<<<<< HEAD:Tank_Movement.cs
        private void CollidesWithMap()
=======
        public void ForceMove(Vector2 offset)
        {
            position += offset;
            ClampToMap(); // Щоб не вилетів за карту
        }


        private void ClampToMap()
>>>>>>> 6516c0a (Bombs.Update3):Tank.cs
        {
            position.X = MathHelper.Clamp(position.X, 596, 1428);
            position.Y = MathHelper.Clamp(position.Y, 346, 730);
        }

        public bool Intersects(Tank other, Vector2 newPos)
        {
<<<<<<< HEAD:Tank_Movement.cs
            int scaledSize = 64;
            Rectangle rectA = new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), scaledSize, scaledSize);
            Rectangle rectB = new Rectangle((int)(other.position.X - other.origin.X), (int)(other.position.Y - other.origin.Y), scaledSize, scaledSize);

            if (!rectA.Intersects(rectB))
                return false;

            return PixelCollision(this, other, newPos);
        }

        private bool PixelCollision(Tank a, Tank b, Vector2 newPosA)
        {
            int top = Math.Max((int)newPosA.Y, (int)b.position.Y);
            int bottom = Math.Min((int)newPosA.Y + a.texture.Height, (int)b.position.Y + b.texture.Height);
            int left = Math.Max((int)newPosA.X, (int)b.position.X);
            int right = Math.Min((int)newPosA.X + a.texture.Width, (int)b.position.X + b.texture.Width);

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
                return Color.Transparent;

            int index = y * texture.Width + x;
            return textureData[index];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
            bomb?.Draw(spriteBatch);
=======
            Rectangle rectA = new Rectangle((int)(newPos.X - 24), (int)(newPos.Y - 24), 48, 48);
            Rectangle rectB = new Rectangle((int)(other.Position.X - 24), (int)(other.Position.Y - 24), 48, 48);
            return rectA.Intersects(rectB);
>>>>>>> 6516c0a (Bombs.Update3):Tank.cs
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

        public void SetDestroyed()
        {
            texture = grey;
        }
    }
}
