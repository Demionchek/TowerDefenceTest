using UnityEngine;
using ObjectPool;
using System.Collections;

public class Spawner : MonoBehaviour {
	[SerializeField] private float m_interval = 1f;
	[SerializeField] private Transform m_moveTarget;

    public static LayerMask m_enemyLayerMask;
    private GameObject m_enemy;

    private const string ENEMY_LAYER_S = "Enemy";
    private bool m_isSpawning = false;

    private void Start() {
        SetupEnemy();
        StartSpawning();
    }

    private void StartSpawning() {
        if (!m_isSpawning) {
            m_isSpawning = true;
            StartCoroutine(SpawnCoroutine());
        }
    }

    private void StopSpawning() {
        m_isSpawning = false;
    }

    private IEnumerator SpawnCoroutine() {
        while (m_isSpawning) {
            SpawnEnemy();
            yield return new WaitForSeconds(m_interval);
        }
    }

    private void SpawnEnemy() {
        if (m_enemy != null) {
            GameObject newMonster = ObjectPoolManager.SpawnObject(m_enemy, transform.position, Quaternion.identity, PoolType.Enemies);
            if (newMonster.TryGetComponent(out Monster monsterBeh)) {
                monsterBeh.SetMoveTarget(m_moveTarget);
            } else {
                Debug.LogWarning("Spawner.SpawnEnemy: Monster component not found on spawned enemy!");
            }
        } else {
            Debug.LogWarning("Spawner.SpawnEnemy: enemy prefab is not set!");
        }
    }

    private void SetupEnemy() {
        if(m_moveTarget == null) {
            Debug.LogError("Spawner.SetupEnemy: m_moveTarget is not set!");
            return;
        }

        GameObject newMonster = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        newMonster.transform.position = transform.position;

        int layer = LayerMask.NameToLayer(ENEMY_LAYER_S);
        newMonster.layer = layer;
        m_enemyLayerMask = 1 << layer;

        var r = newMonster.AddComponent<Rigidbody>();
        r.useGravity = false;
        r.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Monster monsterBeh = newMonster.AddComponent<Monster>();
        monsterBeh.SetMoveTarget(m_moveTarget);

        m_enemy = newMonster;
        newMonster.SetActive(false);
    }
}
