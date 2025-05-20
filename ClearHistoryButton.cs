using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace k
{
    public class ClearHistoryButton : Drawable
    {
        private RectangleShape shape;
        private Text label;
        private Color normalColor = new Color(200, 80, 80); // М'який червоний
        private Color hoverColor = new Color(230, 100, 100); // Світліший червоний
        private Color activeColor = new Color(170, 60, 60); // Темніший червоний при натисканні
        private Color textColor = Color.White;

        public event EventHandler Clicked;

        public ClearHistoryButton(string text, Font font, Vector2f position, Vector2f size)
        {
            shape = new RectangleShape(size)
            {
                Position = position,
                FillColor = normalColor,
                OutlineThickness = 2,
                OutlineColor = new Color(150, 50, 50)
            };

            label = new Text(text, font, 24)
            {
                FillColor = textColor
            };

            // Центруємо текст на кнопці
            FloatRect textRect = label.GetLocalBounds();
            label.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
            label.Position = new Vector2f(position.X + size.X / 2f, position.Y + size.Y / 2f);
        }

        public bool IsClicked(Vector2i mousePos)
        {
            return shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);
        }

        public void OnClick()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Vector2i mousePos = Mouse.GetPosition((Window)target); // Передаємо Window для отримання позиції миші
            bool isHovered = IsClicked(mousePos);
            bool isPressed = Mouse.IsButtonPressed(Mouse.Button.Left) && isHovered;

            if (isPressed)
            {
                shape.FillColor = activeColor;
            }
            else if (isHovered)
            {
                shape.FillColor = hoverColor;
            }
            else
            {
                shape.FillColor = normalColor;
            }

            target.Draw(shape, states);
            target.Draw(label, states);
        }
    }
}