using UnityEngine;

[CreateAssetMenu(
    fileName = "Player Data",
    menuName = "Game Data/Player Data",
    order = 0)]
/// <summary>
/// ScriptableObject с базовыми параметрами игрока.
/// Хранит стартовые значения здоровья, маны и настроек движения,
/// которые затем читают PlayerStats и PlayerController.
/// </summary>
public class PlayerData : ScriptableObject
{
    [Header("Основные характеристики")]
    [Min(1f)] public float maxHealth = 100f; //максимальное здоровье игрока
    [Min(0f)] public float maxMana = 0f; // скорость передвижения игрока

    [Header("Движение")]
    [Min(0f)] public float moveSpeed = 5f; //скорость передвижения игрока
    [Min(0f)] public float jumpForce = 5f; // сила прыжка игрока

    [Header("Дополнительные параметры движения")]
    [Min(0f)] public float acceleration = 10f; // ускорение при движении
    [Min(0f)] public float rotationSpeed = 720f; // скорость вращения игрока (градусы в секунду)
}