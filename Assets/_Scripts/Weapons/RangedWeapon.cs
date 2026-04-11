using UnityEngine;

/// <summary>
/// Оружие дальнего боя, атакующее врага с помощью RayCast.
/// </summary>
public class RangedWeapon : WeaponBase
{
    [Header("Raycast параметры")]
    [Tooltip("Максимальная дальность выстрела.")]
    public float maxRange = 30f;
    [Tooltip("Слой, по которому производится RayCast (например, только враги).")]
    public LayerMask hitMask;
    [Tooltip("Префаб эффекта попадания (опционально).")]
    public GameObject hitEffectPrefab;

    [Tooltip("Точка, из которой производится RayCast (например, дуло оружия или камера).")]
    public Transform firePoint;

    public override void Attack()
    {
        if (!CanAttack())
            return;

        StartAttackCooldown();

        if (firePoint == null)
        {
            Debug.LogWarning($"{name}: Не назначен firePoint для RayCast!", this);
            return;
        }

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRange, hitMask))
        {
            // Попадание по врагу
            var enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(Damage);
            }

            // Визуальный эффект попадания (если задан)
            if (hitEffectPrefab != null)
            {
                GameObject effect = GameObject.Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                GameObject.Destroy(effect, 1.5f);
            }
        }

        // Можно добавить звук выстрела, анимацию и т.д.
        Debug.Log($"{name}: Выстрел произведён.");
    }
}