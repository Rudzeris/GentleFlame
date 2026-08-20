namespace Assets.Scripts.Core.Enums
{
    /// <summary>
    /// Виды топлива. Значения заданы явно: они попадают в сохранение,
    /// поэтому переупорядочивание списка не должно ломать старые сейвы.
    /// </summary>
    public enum FuelType
    {
        Straw = 0,      // Солома
        Twigs = 1,      // Хворост
        Cones = 2,      // Шишки
        Log = 3,        // Полено
        Resin = 4,      // Смола
        Coal = 5,       // Уголь
        Heartwood = 6,  // Сердце-древо
    }
}
