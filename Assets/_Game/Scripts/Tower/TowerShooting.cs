using System;
using System.Collections;
using UnityEngine;

public class TowerShooting : MonoBehaviour
{

    /// <summary>
    /// Events
    /// </summary>
    public event Action OnTargetChanged;

    /// <summary>
    /// Tower component
    /// </summary>
    private Tower tower;
    private EnemeHealth currentTarget;
    public EnemeHealth CurrentTarget => currentTarget;
    private EnemeHealth previousTarget;

    /// <summary>
    /// Bullet properties
    /// </summary>
    [Header("Bullet Properties")]
    [SerializeField] private float damage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ClassifyBullet classifyBullet;
    [SerializeField] private Transform bulletSpawnPoint;

    /// <summary>
    /// Tower settings
    /// </summary>
    [Header("Settings")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float coolDownTime = 3f;
    [SerializeField] private bool keepTarget = false;
    [SerializeField] private TargetingMode targetingMode;

    private BulletPoolManager bulletPoolManager;

    public enum TargetingMode
    {
        Closest,
        Farthest,
        MostHealth,
        LowestHealth
    }

    private void Start()
    {
        InitializeComponents();
        tower.UpdateEnemies += OnEnemiesUpdated;
    }

    private void InitializeComponents()
    {
        tower = GetComponent<Tower>();
        if (tower == null)
        {
            Debug.LogError("Tower component is missing.");
        }

        bulletPoolManager = BulletPoolManager.Instance;
        if (bulletPoolManager == null)
        {
            Debug.LogError("BulletPoolManager instance is missing.");
        }
    }

    private void OnEnemiesUpdated()
    {
        if (!keepTarget || currentTarget == null || !tower.Enemies.Contains(currentTarget))
        {
            UpdateTarget();
        }
        Attack();
    }

    public void Attack()
    {
        if (canShoot && currentTarget != null)
        {
            StartCoroutine(SpawnBulletCoroutine());
        }
    }

    private IEnumerator SpawnBulletCoroutine()
    {
        while (tower.Enemies.Contains(currentTarget) && currentTarget.CurrentHealth > 0)
        {
            SpawnBullet(currentTarget);
            yield return CooldownCoroutine();

            if (!keepTarget)
            {
                UpdateTarget();
            }
        }
    }

    private void UpdateTarget()
    {
        previousTarget = currentTarget;
        currentTarget = GetTarget(targetingMode);
        if (previousTarget != currentTarget)
        {
            OnTargetChanged?.Invoke();
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        canShoot = false;
        yield return new WaitForSeconds(coolDownTime);
        canShoot = true;
    }

    private void SpawnBullet(EnemeHealth enemy)
    {
        if (enemy == null || enemy.CurrentHealth <= 0)
        {
            return;
        }

        GameObject bullet = bulletPoolManager.GetBullet(classifyBullet).gameObject;
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.GetComponent<Bullet>().SetBullet(enemy, damage, bulletSpeed, bulletPoolManager);
    }

    private EnemeHealth GetTarget(TargetingMode mode)
    {
        if (tower.Enemies.Count == 0)
        {
            return null;
        }

        return mode switch
        {
            TargetingMode.Closest => GetClosestTarget(),
            TargetingMode.Farthest => GetFarthestTarget(),
            TargetingMode.MostHealth => GetMostHealthTarget(),
            TargetingMode.LowestHealth => GetLowestHealthTarget(),
            _ => null,
        };
    }

    private EnemeHealth GetClosestTarget()
    {
        EnemeHealth closestTarget = null;
        float minDistance = float.MaxValue;

        foreach (EnemeHealth enemy in tower.Enemies)
        {
            if (enemy.CurrentHealth <= 0)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = enemy;
            }
        }

        return closestTarget;
    }

    private EnemeHealth GetFarthestTarget()
    {
        EnemeHealth farthestTarget = null;
        float maxDistance = float.MinValue;

        foreach (EnemeHealth enemy in tower.Enemies)
        {
            if (enemy.CurrentHealth <= 0)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestTarget = enemy;
            }
        }

        return farthestTarget;
    }

    private EnemeHealth GetMostHealthTarget()
    {
        EnemeHealth mostHealthTarget = null;
        float maxHealth = float.MinValue;

        foreach (EnemeHealth enemy in tower.Enemies)
        {
            if (enemy.CurrentHealth <= 0)
            {
                continue;
            }
            if (enemy.CurrentHealth > maxHealth)
            {
                maxHealth = enemy.CurrentHealth;
                mostHealthTarget = enemy;
            }
        }

        return mostHealthTarget;
    }

    private EnemeHealth GetLowestHealthTarget()
    {
        EnemeHealth lowestHealthTarget = null;
        float minHealth = float.MaxValue;

        foreach (EnemeHealth enemy in tower.Enemies)
        {
            if (enemy.CurrentHealth <= 0)
            {
                continue;
            }
            if (enemy.CurrentHealth < minHealth)
            {
                minHealth = enemy.CurrentHealth;
                lowestHealthTarget = enemy;
            }
        }

        return lowestHealthTarget;
    }
}
