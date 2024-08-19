using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(TowerDetection))]
[RequireComponent(typeof(TowerShooting))]
public class Tower : MonoBehaviour
{
    // Script này sẽ là phần setting, tìm những enemy có máu cao nhất or gần nhất or gần nhất đến đích và truyền vào TowerShooting
    // Xoay tower về phía enemy


    /// <summary>
    /// Events
    /// </summary>
    public event Action UpdateEnemies;

    /// <summary>
    /// List of enemies
    /// </summary>
    private HashSet<EnemeHealth> enemies = new HashSet<EnemeHealth>();
    public HashSet<EnemeHealth> Enemies => enemies;

    public void AddEnemy(EnemeHealth enemy)
    {
        enemies.Add(enemy);
        UpdateEnemies?.Invoke();
    }

    public void RemoveEnemy(EnemeHealth enemy)
    {
        enemies.Remove(enemy);
        UpdateEnemies?.Invoke();
    }

}