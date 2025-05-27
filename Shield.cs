using k.Interfaces;
using SFML.Graphics;
using static k.Constants;
using SFML.System;

namespace k
{
    class Shield : ICollectible,IVisualEffect
    {
        private Color circleColor = new Color(0, 191, 255);
        private CircleShape shield;

        public Transformable CollectibleObject { get; set; }
        public Clock Timer { get; set; } = new Clock();
        public bool InUse { get; set; } = false;

        public Shield()
        {
            shield = new CircleShape(CircleRadius)
            {
                FillColor = Color.Transparent,
                OutlineThickness = CircleThickness,
                OutlineColor = circleColor,
                Origin = new Vector2f(CircleRadius, CircleRadius),
            };
            CollectibleObject = shield;
        }
        public bool IsExpired() => Timer.ElapsedTime.AsSeconds() >= 7f;

        public void Draw(RenderWindow window)
        {
            if (InUse)
            {
                window.Draw((Shape)CollectibleObject);
            }

        }
    }
}
