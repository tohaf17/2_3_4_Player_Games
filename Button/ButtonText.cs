using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace k;
public class ButtonText
{
    private RectangleShape shape;
    private Text label;

    public bool IsHovered { get; private set; }
    public bool IsSelected { get; private set; }

    public ButtonText(Vector2f position, Vector2f size, string text, Font font)
    {
        shape = new RectangleShape(size)
        {
            Position = position,
            FillColor = Color.White
        };

        label = new Text(text, font, 24)
        {
            Position = position + new Vector2f(10, 10),
            FillColor = Color.Black
        };
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
        shape.FillColor = IsSelected ? Color.Yellow : (IsHovered ? new Color(211, 211, 211) // RGB для LightGray
: Color.White);
        window.Draw(shape);
        window.Draw(label);
    }

    public string GetText() => label.DisplayedString;
}
