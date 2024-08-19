using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerDetection : MonoBehaviour
{

    [SerializeField] SphereCollider sphereCollider;
    // enemy tag
    [SerializeField] float detectionRadius;
    private Tower tower;
    private float defaultRadius = 18f;

    void Start()
    {
        Init();
    }

    void Reset()
    {
        detectionRadius = defaultRadius;
        Init();

    }


    void Init()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = detectionRadius;
        sphereCollider.isTrigger = true;

        if (tower == null)
        {
            tower = GetComponent<Tower>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemeHealth enemy = other.GetComponent<EnemeHealth>();
            if (!tower.Enemies.Contains(enemy))
            {
                tower.AddEnemy(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemeHealth enemy = other.GetComponent<EnemeHealth>();
            tower.RemoveEnemy(enemy);
        }
    }
}