
using k;
public abstract class GameEntity
{
	public abstract void Update(SFML.System.Time time, List<GameEntity> list,SFML.System.Vector2f offset);
	public abstract void Draw(SFML.Graphics.RenderWindow window);
}
