using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace k;

public class ButtonText
{
    private RectangleShape shape;
    private Text label;
    private Color normalColor = new Color(70, 130, 180);
    private Color hoverColor = new Color(100, 149, 237);
    private Color selectedColor = new Color(255, 215, 0);
    private Color textColor = Color.White;

    public bool IsHovered { get; set; }
    public bool IsSelected { get; set; }

    public ButtonText(Vector2f position, Vector2f size, string text, Font font)
    {
        shape = new RectangleShape(size)
        {
            Position = position,
            FillColor = normalColor,
            OutlineThickness = 2,
            OutlineColor = new Color(50, 90, 120)
        };

        label = new Text(text, font, 28)
        {
            FillColor = textColor
        };

        FloatRect textRect = label.GetLocalBounds();
        label.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
        label.Position = new Vector2f(position.X + size.X / 2f, position.Y + size.Y / 2f);
    }

    public void Update(Vector2i mousePos, bool isClicked)
    {
        IsHovered = shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);

        if (IsHovered && isClicked)
            IsSelected = true;
    }

    public void Deselect() => IsSelected = false;

    public void Draw(RenderWindow window)
    {
        if (IsSelected)
        {
            shape.FillColor = selectedColor;
            label.FillColor = Color.Black;
        }
        else if (IsHovered)
        {
            shape.FillColor = hoverColor;
            label.FillColor = textColor;
        }
        else
        {
            shape.FillColor = normalColor;
            label.FillColor = textColor;
        }

        window.Draw(shape);
        window.Draw(label);
    }

    public string GetText() => label.DisplayedString;
}