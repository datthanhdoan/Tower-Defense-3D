using UnityEngine;

public class Bullet : MonoBehaviour
{
    enum BulletTargetStatus
    {
        NonTargetEnemy,
        AimtoEnemy
    }
    float _damage;
    float _bulletSpeed;
    BulletTargetStatus _bulletRotationStatus = BulletTargetStatus.NonTargetEnemy;
    EnemeHealth _target;
    BulletPoolManager _bulletPoolManager;
    [SerializeField] ClassifyBullet _classifyBullet;

    public ClassifyBullet ClassifyBullet => _classifyBullet;

    public void SetBullet(EnemeHealth target, float damage, float bulletSpeed, BulletPoolManager bulletPoolManager)
    {
        this._target = target;
        this._damage = damage;
        this._bulletSpeed = bulletSpeed;
        this._bulletPoolManager = bulletPoolManager;
    }


    private void Update()
    {
        if (_target == null)
        {
            ReleaseBullet();
            return;
        }

        if (_bulletRotationStatus == BulletTargetStatus.NonTargetEnemy)
        {
            RotateBullet();
            _bulletRotationStatus = BulletTargetStatus.AimtoEnemy;
        }

        MoveBullet();


    }

    private void MoveBullet()
    {
        Vector3 direction = _target.transform.position - transform.position;
        float distanceThisFrame = _bulletSpeed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    // Rotate bullet to face the target

    private void RotateBullet()
    {
        Vector3 direction = _target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        _target.TakeDamage(_damage);
        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        _bulletPoolManager.ReturnBullet(_classifyBullet, this);
    }

    // Reset bullet data
    private void OnDisable()
    {
        _target = null;
        _damage = 0;
        _bulletSpeed = 0;
        _bulletRotationStatus = BulletTargetStatus.NonTargetEnemy;
    }
}