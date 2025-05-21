using SFML.Graphics; 
using SFML.System;   

namespace k;

public abstract class GameEntity
{
    protected Sprite sprite;
    protected Vector2u screenSize; 
    protected byte[] collisionMask;// Це поле тепер не буде впливати на частоту пострілів при затиснутій кнопці, лише на логіку обертання.

    public byte[] CollisionMask
    {
        get { return collisionMask; }
        set { collisionMask = value; }
    }
    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }
    public Vector2u ScreenSize
    {
        get { return screenSize; }
        set { screenSize = value; }
    }
    //ВИВЕСТИ У КОНСТАНТИ

    public abstract void Update(Time time, List<GameEntity> entities, Vector2f offset);
    public abstract void Draw(RenderWindow window);

    protected void ApplyScreenWrapping()
    {
        if (sprite == null)
        {
            Console.WriteLine("Помилка: Спрайт не ініціалізовано в GameEntity для обгортання екрану.");
            return;
        }
        float screenWidth = screenSize.X;
        float screenHeight = screenSize.Y;

        var bounds = sprite.GetGlobalBounds();
        float halfW = bounds.Width / 2f;
        float halfH = bounds.Height / 2f;

        Vector2f currentPosition = sprite.Position;

        if (currentPosition.X < -halfW)
        {
            sprite.Position = new Vector2f(screenWidth + halfW, currentPosition.Y);
        }
        else if (currentPosition.X > screenWidth + halfW)
        {
            sprite.Position = new Vector2f(-halfW, currentPosition.Y);
        }
        if (currentPosition.Y < -halfH)
        {
            sprite.Position = new Vector2f(currentPosition.X, screenHeight + halfH);
        }
        else if (currentPosition.Y > screenHeight + halfH)
        {
            sprite.Position = new Vector2f(currentPosition.X, -halfH);
        }
    }
}