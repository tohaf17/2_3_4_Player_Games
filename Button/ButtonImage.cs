using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace k;
public class ButtonImage
{
    private RectangleShape background;
    private Sprite icon;

    public bool IsHovered { get; private set; }
    public bool IsSelected { get; private set; }

    public ButtonImage(Vector2f position, Vector2f size, Texture iconTexture)
    {
        background = new RectangleShape(size)
        {
            Position = position,
            FillColor = Color.White
        };

        icon = new Sprite(iconTexture)
        {
            Position = position + new Vector2f((size.X - 64) / 2, (size.Y - 64) / 2),
            Scale = new Vector2f(64f / iconTexture.Size.X, 64f / iconTexture.Size.Y)
        };
    }

    public void Update(Vector2i mousePos, bool isClicked)
    {
        IsHovered = background.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);
        if (IsHovered && isClicked)
            IsSelected = true;
    }

    public void Deselect() => IsSelected = false;

    public void Draw(RenderWindow window)
    {
        background.FillColor = IsSelected ? Color.Yellow : (IsHovered ? new Color(211, 211, 211) : Color.White);
        window.Draw(background);
        window.Draw(icon);
    }
}
