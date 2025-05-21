using SFML.Graphics;

namespace k.Interfaces
{
    // ICollectible тепер успадковує ITimeLimited, оскільки колекційні об'єкти
    // зазвичай мають час життя або тривалість дії.
    public interface ICollectible : ITimeLimited
    {
        Transformable CollectibleObject { get; set; } // Об'єкт на карті, який можна зібрати
    }
}