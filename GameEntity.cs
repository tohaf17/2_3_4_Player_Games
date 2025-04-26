
public abstract class GameEntity
{
	public abstract void Update(SFML.System.Time time, List<GameEntity>? list, int[,] map);
	public abstract void Draw(SFML.Graphics.RenderWindow window);
}
