using k.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace k
{
    class Shield : ICollectible,IVisualEffect
    {

        private const float circleRadius = 32f;
        private const float circleThickness = 2f;
        private Color circleColor = new Color(0, 191, 255);

        private CircleShape shield;
        public Transformable CollectibleObject { get; set; }

        public Clock Timer { get; set; } = new Clock();
        public bool InUse { get; set; } = false;

        public Shield()
        {
            shield = new CircleShape(circleRadius)
            {
                FillColor = Color.Transparent,
                OutlineThickness = circleThickness,
                OutlineColor = circleColor,
                Origin = new Vector2f(circleRadius, circleRadius),
            };
            CollectibleObject = shield;
        }
        public bool IsExpired() => Timer.ElapsedTime.AsSeconds() >= 10f;

        public void Draw(RenderWindow window)
        {
            if (InUse)
            {
                window.Draw((Shape)CollectibleObject);
            }

        }
    }
}
