    using k.Interfaces;
using SFML.Graphics;
using SFML.System;
using static k.Constants;

namespace k
{
    class MiniTank : ICollectible
    {
        public Transformable CollectibleObject { get; set; } 
        public Clock Timer { get; set; } = new Clock();
        public bool InUse { get; set; } = false;

        public MiniTank(Transformable tankSprite)
        {
            CollectibleObject = tankSprite;
        }

        public bool IsExpired() => Timer.ElapsedTime.AsSeconds() >= 10f;

        
        public void ApplyEffect()
        {
            if (CollectibleObject is Sprite sprite) 
            {
                sprite.Scale = new Vector2f(64f / sprite.Texture.Size.X * ScaleNumber, 64f / sprite.Texture.Size.Y * ScaleNumber);
            }
        }

        public void RevertEffect()
        {
            if (CollectibleObject is Sprite tankSprite)
            {
                tankSprite.Scale = new Vector2f(64f / tankSprite.Texture.Size.X, 64f / tankSprite.Texture.Size.Y);
            }
        }
    }
}