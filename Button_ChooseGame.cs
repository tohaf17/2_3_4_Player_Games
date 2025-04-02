using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button_ChooseGame
{
    public Rectangle Bounds { get; private set; }

    private Texture2D texture; // Фон кнопки
    private Texture2D imageTexture; // Зображення на кнопці
    private bool isHovered;
    public bool IsSelected { get; private set; }

    public Button_ChooseGame(Vector2 position, Texture2D texture, Texture2D imageTexture)
    {
        Bounds = new Rectangle((int)position.X, (int)position.Y, 100, 100); // Розмір кнопки 100x100
        this.texture = texture;
        this.imageTexture = imageTexture;
    }

    public void Update(MouseState mouseState, MouseState previousMouseState)
    {
        isHovered = Bounds.Contains(mouseState.Position);

        // Перевіряємо, чи кнопка була натиснута в цьому кадрі, але не в попередньому
        if (isHovered && mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
        {
            IsSelected = true;
        }
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color buttonColor = isHovered ? Color.LightGray : Color.White;
        if (IsSelected)
        {
            buttonColor = Color.Yellow;
        }

        spriteBatch.Draw(texture, Bounds, buttonColor);

        // Малюємо зображення в центрі кнопки
        Rectangle imageBounds = new Rectangle(
            Bounds.X + (Bounds.Width - 64) / 2, // Центруємо по горизонталі
            Bounds.Y + (Bounds.Height - 64) / 2, // Центруємо по вертикалі
            64, 64 // Розмір зображення 64x64
        );
        spriteBatch.Draw(imageTexture, imageBounds, Color.White);
    }
}