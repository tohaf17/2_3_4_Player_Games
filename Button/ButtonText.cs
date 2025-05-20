using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace k;

public class ButtonText
{
    private RectangleShape shape;
    private Text label;
    private Color normalColor = new Color(70, 130, 180); // SteelBlue for normal
    private Color hoverColor = new Color(100, 149, 237); // CornflowerBlue for hover
    private Color selectedColor = new Color(255, 215, 0); // Gold for selected
    private Color textColor = Color.White; // Text color

    public bool IsHovered { get; private set; }
    public bool IsSelected { get; private set; }

    public ButtonText(Vector2f position, Vector2f size, string text, Font font)
    {
        shape = new RectangleShape(size)
        {
            Position = position,
            FillColor = normalColor,
            OutlineThickness = 2, // Add a border
            OutlineColor = new Color(50, 90, 120) // Darker shade for border
        };

        label = new Text(text, font, 28) // Slightly larger font for impact
        {
            FillColor = textColor
        };

        // Center the text within the button
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
            label.FillColor = Color.Black; // Black text on selected
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