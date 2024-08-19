using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public enum ClassifyBullet
{
    Arrow,
    CannonBall,
}

[System.Serializable]
public class BulletStorage
{
    public Bullet bulletPrefab;
    public ObjectPool<Bullet> pool;

}
public class BulletPoolManager : GenericSingleton<BulletPoolManager>
{
    [SerializeField] List<BulletStorage> bulletStorages;
    [SerializeField] int poolCapacity;
    [SerializeField] int poolSize;
    private Dictionary<ClassifyBullet, ObjectPool<Bullet>> _bulletPool = new Dictionary<ClassifyBullet, ObjectPool<Bullet>>();

    private void Start()
    {
        foreach (var bulletStorage in bulletStorages)
        {
            CreatePool(bulletStorage);
        }
    }

    public void CreatePool(BulletStorage bulletStorage)
    {
        ClassifyBullet classifyBullet = bulletStorage.bulletPrefab.ClassifyBullet;

        _bulletPool.Add(classifyBullet, new ObjectPool<Bullet>(() =>
        {
            return Instantiate(bulletStorage.bulletPrefab);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, poolCapacity, poolSize));

    }

    public Bullet GetBullet(ClassifyBullet classifyBullet)
    {
        return _bulletPool[classifyBullet].Get();
    }

    public void ReturnBullet(ClassifyBullet classifyBullet, Bullet bullet)
    {
        _bulletPool[classifyBullet].Release(bullet);
    }

}
