using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
public class Button_MainMenu
{
    public Rectangle Bounds { get; private set; }
    public string Text { get; private set; }
    
    private SpriteFont _font;
    private Texture2D _texture;
    private bool _isHovered;
    public bool IsSelected { get; private set; } // Додаємо змінну для вибору

    public Button_MainMenu(Rectangle bounds, string text, SpriteFont font, Texture2D texture)
    {
        Bounds = bounds;
        Text = text;
        _font = font;
        _texture = texture;
    }

    public void Update(MouseState mouseState)
    {
        _isHovered = Bounds.Contains(mouseState.Position);
        
        if (_isHovered && mouseState.LeftButton == ButtonState.Pressed)
        {
            IsSelected = true; // Кнопка вибрана
        }
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color buttonColor = _isHovered ? Color.LightGray : Color.White;
        if (IsSelected)
        {
            buttonColor = Color.Yellow; // Виділена кнопка стає жовтою
        }

        spriteBatch.Draw(_texture, Bounds, buttonColor);

        Vector2 textSize = _font.MeasureString(Text);
        Vector2 textPosition = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(_font, Text, textPosition, Color.Black);
    }
}