using k.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k
{
    class Shield : GameEntity, IBox
    {

        private const float circleRadius = 32f;
        private const float circleThickness = 2f;
        private Color circleColor = new Color(0, 191, 255);

        private CircleShape _boxObject;
        public Transformable boxObject
        {
            get { return _boxObject; }
            set { _boxObject = (CircleShape)value; }
        }

        public Clock Timer { get; set; } = new Clock();
        public bool InUse { get; set; } = false;

        public Shield()
        {
            _boxObject = new CircleShape(circleRadius)
            {
                FillColor = Color.Transparent,
                OutlineThickness = circleThickness,
                OutlineColor = circleColor,
                Origin = new Vector2f(circleRadius, circleRadius),
            };
        }
        public bool IsExpired() => Timer.ElapsedTime.AsSeconds() >= 10f;

        public override void Draw(RenderWindow window)
        {
            if (InUse)
            {
                window.Draw(_boxObject);
            }

        }
        void IBox.Draw(RenderWindow window)
        {
            Draw(window);
        }
        public override void Update(Time time, List<GameEntity> list, Vector2f offset)
        {

        }
    }
}
