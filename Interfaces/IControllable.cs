namespace k.Interfaces;
interface IControllable
{
    void HandleInput(float delta, List<GameEntity> list);
}