using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace k
{
    public class Tank : GameEntity, IControllable
    {
        private Sprite sprite;
        private float rotationSpeed = 200f;
        private float speed = 300f;
        private Keyboard.Key fireKey;
        private Texture destroyedTexture;
        private Texture bombTexture;
        private Bomb bomb;
        private float cooldown = 0f;
        private const float BombDelay = 3f;
        private PlayerData data = new();

        private Vector2f velocity = new Vector2f(0, 0);
        private const float dAcc = 250f;
        private const float damping = 0.80f;

        private int[,] map;
        private int tileSize;
        private Vector2f offset; // ДОДАНО offset

        public Tank(Texture texture, Vector2f position, Keyboard.Key key, Texture grey, Texture bombTex, int[,] map, int tileSize, Vector2f offset)
        {
            sprite = new Sprite(texture)
            {
                Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
                Position = position,
                Scale = new Vector2f(64f / texture.Size.X, 64f / texture.Size.Y)
            };
            fireKey = key;
            destroyedTexture = grey;
            bombTexture = bombTex;
            this.map = map;
            this.tileSize = tileSize;
            this.offset = offset; // зберігаємо offset
        }

        public override void Update(Time deltaTime, List<GameEntity> entities, int[,] map)
        {
            float delta = deltaTime.AsSeconds();
            bomb?.Update(deltaTime, entities, map);

            if (cooldown > 0)
                cooldown -= delta;

            HandleInput(delta, entities);

            var newPosition = sprite.Position - velocity * delta;

            bool isBlocked = false;

            if (!CanMoveTo(newPosition))
                isBlocked = true;

            if (!isBlocked)
            {
                foreach (var entity in entities)
                {
                    if (entity is Tank otherTank && otherTank != this && Intersects(otherTank, newPosition))
                    {
                        isBlocked = true;
                        break;
                    }
                }
            }

            if (!isBlocked)
            {
                sprite.Position = newPosition;
                ClampToMap();
            }

            velocity *= damping;
        }

        public void HandleInput(float delta, List<GameEntity> entities)
        {
            if (!IsAlive) return;

            if ((bomb == null || !bomb.IsActive) && cooldown <= 0f && Keyboard.IsKeyPressed(fireKey))
            {
                var angleRad = sprite.Rotation * (float)Math.PI / 180f;
                var direction = new Vector2f(-(float)Math.Sin(angleRad), (float)Math.Cos(angleRad));
                var spawn = sprite.Position + direction * 32f;

                bomb = new Bomb(bombTexture, spawn, direction, sprite.Rotation, this);
                cooldown = BombDelay;
            }

            if (Keyboard.IsKeyPressed(fireKey))
            {
                var angleRad = (sprite.Rotation - 90f) * (float)Math.PI / 180f;
                var direction = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
                velocity += direction * dAcc * delta;
            }
            else
            {
                sprite.Rotation += rotationSpeed * delta;
            }
        }

        public override void Draw(RenderWindow window)
        {
            window.Draw(sprite);
            bomb?.Draw(window);
        }

        public Vector2f Position => sprite.Position;
        public bool IsAlive => sprite.Texture != destroyedTexture;
        public Bomb ActiveBomb => bomb;
        public PlayerData Data => data;

        public void TakeDamage()
        {
            sprite = new Sprite(destroyedTexture)
            {
                Origin = new Vector2f(destroyedTexture.Size.X / 2f, destroyedTexture.Size.Y / 2f),
                Position = Position,
                Scale = new Vector2f(64f / destroyedTexture.Size.X, 64f / destroyedTexture.Size.Y)
            };
        }

        public void ForceMove(Vector2f moveOffset)
        {
            sprite.Position += moveOffset;
            ClampToMap();
        }

        private void ClampToMap()
        {
            float mapWidth = tileSize * map.GetLength(1);
            float mapHeight = tileSize * map.GetLength(0);

            sprite.Position = new Vector2f(
                Math.Clamp(sprite.Position.X, offset.X, offset.X + mapWidth),
                Math.Clamp(sprite.Position.Y, offset.Y, offset.Y + mapHeight)
            );
        }

        public bool Intersects(Tank other, Vector2f newPos)
        {
            FloatRect rectA = new FloatRect(newPos.X - 24, newPos.Y - 24, 48, 48);
            FloatRect rectB = new FloatRect(other.Position.X - 24, other.Position.Y - 24, 48, 48);
            return rectA.Intersects(rectB);
        }

        private bool CanMoveTo(Vector2f position)
        {
            Vector2f halfSize = new Vector2f(24, 24);

            Vector2f[] corners = new Vector2f[]
            {
                position + new Vector2f(-halfSize.X, -halfSize.Y),
                position + new Vector2f(halfSize.X, -halfSize.Y),
                position + new Vector2f(-halfSize.X, halfSize.Y),
                position + new Vector2f(halfSize.X, halfSize.Y)
            };

            foreach (var corner in corners)
            {
                Vector2f relativeCorner = corner - offset;

                int tileX = (int)(relativeCorner.X / tileSize);
                int tileY = (int)(relativeCorner.Y / tileSize);

                if (tileX < 0 || tileY < 0 || tileY >= map.GetLength(0) || tileX >= map.GetLength(1))
                    return false;

                if (map[tileY, tileX] != 0)
                    return false;
            }

            return true;
        }
    }
}
