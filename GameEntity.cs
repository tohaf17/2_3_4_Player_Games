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

        Vector2f currentPosition = sprite.Position;


        currentPosition.X = (currentPosition.X + screenWidth) % screenWidth;
        currentPosition.Y = (currentPosition.Y + screenHeight) % screenHeight;

        if (currentPosition.X < 0)
            currentPosition.X += screenWidth;
        if (currentPosition.Y < 0)
            currentPosition.Y += screenHeight;

        sprite.Position = currentPosition;
    }
}
