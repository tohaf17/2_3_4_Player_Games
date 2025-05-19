using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace k
{
    class MiniTank : GameEntity, IBox
    {
        private float _scaleFactor = 0.5f; // Коефіцієнт зменшення (наприклад, вдвічі менший)

        public Transformable boxObject { get; set; } // Більше не зберігаємо спрайт
        public Clock Timer { get; set; } = new Clock();
        public bool InUse { get; set; } = false;
        private Sprite _tankSprite; // Посилання на спрайт танка

        // Конструктор приймає спрайт танка
        public MiniTank(Sprite tankSprite)
        {
            _tankSprite = tankSprite;
        }

        public bool IsExpired() => Timer.ElapsedTime.AsSeconds() >= 10f;

        public override void Draw(RenderWindow window)
        {
            // Нічого не малюємо, ефект застосовується до спрайта танка
        }

        void IBox.Draw(RenderWindow window)
        {
            Draw(window);
        }

        public override void Update(Time time, List<GameEntity> list, Vector2f offset)
        {
            // Нічого не оновлюємо, ефект залежить від стану InUse
        }

        // Метод для застосування ефекту зменшення
        public void ApplyEffect()
        {
            if (_tankSprite != null)
            {
                _tankSprite.Scale = new Vector2f(64f / _tankSprite.Texture.Size.X * _scaleFactor, 64f / _tankSprite.Texture.Size.Y * _scaleFactor);
            }
        }

        // Метод для скасування ефекту зменшення
        public void RevertEffect()
        {
            if (_tankSprite != null)
            {
                _tankSprite.Scale = new Vector2f(64f / _tankSprite.Texture.Size.X, 64f / _tankSprite.Texture.Size.Y);
            }
        }
    }
}