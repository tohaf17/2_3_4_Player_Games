using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace k
{
    public class ClearHistoryButton
    {
        private Text text;
        private RectangleShape background;
        public event EventHandler Clicked;

        public ClearHistoryButton(string text, Font font, Vector2f position, Vector2f size)
        {
            background = new RectangleShape(size)
            {
                FillColor = new Color(200, 50, 50),
                Position = position
            };

            this.text = new Text(text, font, 24)
            {
                FillColor = Color.White
            };

            FloatRect textRect = this.text.GetLocalBounds();
            this.text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
            this.text.Position = new Vector2f(position.X + size.X / 2f, position.Y + size.Y / 2f);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(background, states);
            target.Draw(text, states);
        }

        public bool IsClicked(Vector2i mousePos)
        {
            return background.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);
        }

        public void OnClick()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}