using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Game
{
    public class Tank_Movement
    {
        private Texture2D texture;
        private Vector2 startPosition;
        private float speed = 3.5f;
        private float rotationSpeed = 3.5f;
        private float rotation = 0f; // Кут повороту
        private Vector2 origin;
        private Vector2 barrelTip = new Vector2(0, -10);
        private Keys key;

        // Межі руху танка
        private readonly int minX = 564 + 32;
        private readonly int maxX = 1460 - 32;
        private readonly int minY = 314 + 32;
        private readonly int maxY = 762 - 32;

        public Tank_Movement(Texture2D texture, Vector2 startPosition, Keys key)
        {
            this.texture = texture;
            this.startPosition = startPosition;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.key = key;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 barrel = startPosition + RotateVector(barrelTip, rotation);

            if (state.IsKeyDown(key))
            {
                Vector2 direction = barrel - startPosition;
                direction.Normalize();
                Vector2 newPosition = startPosition - direction * speed;

                // Перевірка меж та ковзання по стіні
                if (newPosition.X < minX)
                {
                    newPosition.X = minX;
                }
                if (newPosition.X > maxX)
                {
                    newPosition.X = maxX;
                }
                if (newPosition.Y < minY)
                {
                    newPosition.Y = minY;
                }
                if (newPosition.Y > maxY)
                {
                    newPosition.Y = maxY;
                }

                startPosition = newPosition;
            }
            else
            {
                rotation += MathHelper.ToRadians(rotationSpeed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, startPosition, null, Color.White, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
        }

        private Vector2 RotateVector(Vector2 point, float angle)
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
