using SFML.Graphics; 
using SFML.System;   

namespace k;

public abstract class GameEntity
{
    protected Sprite sprite;
    protected Vector2u screenSize; 
    protected byte[] collisionMask;
    protected MapCollider collider;

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
    public MapCollider Collider
    {
        get { return collider; }
        set { collider = value; }
    }

    public GameEntity(MapCollider collider, Vector2u screenSize)
    {
        this.collider = collider;
        this.screenSize = screenSize;
    }

    public abstract void Update(Time time, List<GameEntity> entities);
    public abstract void Draw(RenderWindow window);

    protected void ApplyScreenWrapping()
    {
        if (sprite == null)
        {
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