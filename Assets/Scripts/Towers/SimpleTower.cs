using UnityEngine;
using ObjectPool;
using System.Collections;

public class SimpleTower : BaseTower {
    private const float SPAWN_HEIGHT = 1.5f;

    protected override IEnumerator ShootingCoroutine() {
        while(true) {
            if(CanShoot() && IsEnemyInRange()) {
                Shoot();
                m_lastShotTime = Time.time;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected override void Shoot() {
        Vector3 spawnPosition = transform.position + Vector3.up * SPAWN_HEIGHT;
        GameObject projectile = ObjectPoolManager.SpawnObject(m_projectilePrefab, spawnPosition, Quaternion.identity, PoolType.Projectiles);

        if(projectile.TryGetComponent(out GuidedProjectile projectileBeh)) {
            projectileBeh.Target = m_currentTarget;
        } else {
            Debug.LogWarning($"SimpleTower: {gameObject.name}: GuidedProjectile component not found on projectile!");
        }
    }
}
