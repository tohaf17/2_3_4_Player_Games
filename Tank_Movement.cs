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
        private bool isMoving=false;
        private Vector2 origin;
        private Vector2 barrelTip=new Vector2(0,-10);

        public Tank_Movement(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.startPosition = startPosition;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            Vector2 barrel = startPosition+RotateVector(barrelTip, rotation);
            // Якщо натиснута клавіша C, починаємо рух
            if (state.IsKeyDown(Keys.C))
            {
                // Рух вперед у напрямку дула
                Vector2 direction = barrel - startPosition;
                direction.Normalize();
                startPosition += -direction * speed;
            }
            else
            {
                // Обертання навколо центру
                rotation += MathHelper.ToRadians(rotationSpeed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, startPosition, null, Color.White, rotation, origin, 64f / texture.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, barrelTip, null, Color.Red, 0f, origin, 0.1f, SpriteEffects.None, 0f);
        }
        // Функція обертання точки навколо центру
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