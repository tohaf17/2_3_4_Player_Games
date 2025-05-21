using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace k
{
    public class ClearHistoryButton
    {
        private Text _text;
        private RectangleShape _background;
        public event EventHandler Clicked;

        public ClearHistoryButton(string text, Font font, Vector2f position, Vector2f size)
        {
            _background = new RectangleShape(size)
            {
                FillColor = new Color(200, 50, 50),
                Position = position
            };

            _text = new Text(text, font, 24)
            {
                FillColor = Color.White
            };

            // Центрування тексту
            FloatRect textRect = _text.GetLocalBounds();
            _text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
            _text.Position = new Vector2f(position.X + size.X / 2f, position.Y + size.Y / 2f);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_background, states);
            target.Draw(_text, states);
        }

        public bool IsClicked(Vector2i mousePos)
        {
            return _background.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);
        }

        public void OnClick()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}