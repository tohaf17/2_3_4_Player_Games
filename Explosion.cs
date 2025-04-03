using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Game
{
    public class Explosion
    {
        private Texture2D texture;
        private Vector2 position;
        private float timer;
        private float duration = 0.5f; // Тривалість вибуху в секундах
        private bool isActive = true;

        public Explosion(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (!isActive) return;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= duration)
            {
                isActive = false;
            }
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
}