using UnityEngine;

[CreateAssetMenu(fileName = "FireConfig", menuName = "Configs/FireConfig")]
public class FireConfig : ScriptableObject
{
    [Header("Основные параметры")]
    [Tooltip("Время горения без подкидывания топлива (секунды)")]
    public float baseBurnTime = 12f;
    [Tooltip("Максимальное время горения(секунды)")]
    public float maxBurnTime = 60f;

    [Tooltip("Базовая мощность огня (HeatPower)")]
    public int baseHeatPower = 1;

    [Tooltip("Скорость уменьшения жара (единиц в секунду)")]
    public float heatDecayRate = 0.1f;

}
