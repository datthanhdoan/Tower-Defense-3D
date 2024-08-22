using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerDataSO", order = 1)]
public class TowerDataSO : ScriptableObject
{
    [SerializeField] private List<TowerData> towerDataList = new List<TowerData>();

    public TowerData GetTowerData(TowerName name)
    {
        return towerDataList.Find(towerData => towerData.Name == name);
    }



}

[System.Serializable]
public class TowerData
{
    [field: SerializeField] public TowerName Name { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
public enum TowerName
{
    Archer,
    Ballista,
    Cannon,
    Poison,
    Wizard,
}