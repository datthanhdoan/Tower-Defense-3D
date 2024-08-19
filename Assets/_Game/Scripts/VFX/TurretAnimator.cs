using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerShooting))]
public class TurretAnimator : MonoBehaviour
{
    private EnemeHealth _target;
    private TowerShooting _towerShooting;
    [SerializeField] bool _hasBase;
    [SerializeField] bool _hasTurret;
    [SerializeField] GameObject _base;
    [SerializeField] GameObject _turret;
    [SerializeField] float _rotationSpeed = 3f;

    private void Start()
    {
        _towerShooting = GetComponent<TowerShooting>();
        _towerShooting.OnTargetChanged += HandleEnemies;
    }

    private void HandleEnemies()
    {
        StartCoroutine(UpdateAim());
    }

    IEnumerator UpdateAim()
    {
        if (_towerShooting.CurrentTarget == null) yield return null;
        _target = _towerShooting.CurrentTarget;
        while (_target != null)
        {
            if (_hasBase) BaseAimtoEnemy(_target);
            if (_hasTurret) TurrentAimtoEnemy(_target);
            yield return null;
        }
    }

    private void TurrentAimtoEnemy(EnemeHealth target)
    {
        Vector3 direction = _target.transform.position - _turret.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(_turret.transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed).eulerAngles;
        _turret.transform.rotation = Quaternion.Euler(rotation);
    }

    private void BaseAimtoEnemy(EnemeHealth target)
    {
        Vector3 direction = _target.transform.position - _base.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(_base.transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed).eulerAngles;
        _base.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }




}
