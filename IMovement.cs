namespace My_Game;

public interface IMovement
{
    void Movement(Player[] players, int[,] map, int tileSize,Game1 game);
    void Move();
}