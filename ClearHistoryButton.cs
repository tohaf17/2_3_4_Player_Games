using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace k
{
    public class ClearHistoryButton
    {
        private RectangleShape background;
        private Text text;
        private Font font;
        private FloatRect bounds;

        public event EventHandler Clicked;

        public ClearHistoryButton(string buttonText, Font font, Vector2f position, Vector2f size)
        {
            this.font = font;
            background = new RectangleShape(size)
            {
                Position = position,
                FillColor = Color.Green,
                OutlineThickness = 1,
                OutlineColor = Color.Black
            };
            text = new Text(buttonText, font, 20)
            {
                Position = new Vector2f(position.X + (size.X - font.GetGlyph(buttonText[0], 20, false, 0).Advance * buttonText.Length) / 2,
                                        position.Y + (size.Y - font.GetLineSpacing(20)) / 2),
                FillColor = Color.Black
            };
            bounds = background.GetGlobalBounds();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(background, states);
            target.Draw(text, states);
        }

        public bool IsClicked(Vector2i mousePosition)
        {
            return bounds.Contains(mousePosition.X, mousePosition.Y);
        }

        public void OnClick()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}