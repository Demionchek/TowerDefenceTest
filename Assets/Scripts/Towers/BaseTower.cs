using UnityEngine;
using System.Collections;

public abstract class BaseTower : MonoBehaviour {
    [SerializeField] protected float m_shootInterval = 0.5f;
    [SerializeField] protected float m_range = 10f;
    [SerializeField] protected GameObject m_projectilePrefab;

    protected float m_lastShotTime;
    protected Coroutine m_shootingCoroutine;
    protected Transform m_currentTarget;

    protected virtual void Start() {
        if(m_projectilePrefab == null) {
            Debug.LogError($"{GetType().Name}: {gameObject.name}: m_projectilePrefab is not set!");
            return;
        }

        StartShooting();
    }

    protected virtual void StartShooting() {
        m_shootingCoroutine = StartCoroutine(ShootingCoroutine());
    }

    protected virtual void StopShooting() {
        if(m_shootingCoroutine != null) {
            StopCoroutine(m_shootingCoroutine);
            m_shootingCoroutine = null;
        }
    }

    protected virtual bool CanShoot() {
        return Time.time >= m_lastShotTime + m_shootInterval;
    }


    protected abstract IEnumerator ShootingCoroutine();

    protected virtual bool IsEnemyInRange() {
        if(m_currentTarget != null && Vector3.Distance(transform.position, m_currentTarget.position) <= m_range) {
            return true;
        }

        Collider[] enemiesColliders = Physics.OverlapSphere(transform.position, m_range, Spawner.m_enemyLayerMask, QueryTriggerInteraction.UseGlobal);
        
        if(enemiesColliders.Length > 0) {
            GetClosestEnemy(enemiesColliders);
        } else {
            m_currentTarget = null;
        }
        return m_currentTarget != null;
    }

    private void GetClosestEnemy(Collider[] enemiesColliders) {
        float closestEnemyDistance = Mathf.Infinity;
        foreach(Collider enemyCollider in enemiesColliders) {
            if(Vector3.Distance(transform.position, enemyCollider.transform.position) < closestEnemyDistance) {
                closestEnemyDistance = Vector3.Distance(transform.position, enemyCollider.transform.position);
                m_currentTarget = enemyCollider.transform;
            }
        }
    }

    protected abstract void Shoot();
}
