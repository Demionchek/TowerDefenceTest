using UnityEngine;
using System.Collections;
using ObjectPool;

public class CannonTower : BaseTower {
    [SerializeField] private Transform m_shootPoint;
    [SerializeField] private Transform m_cannonHubY;
    [SerializeField] private Transform m_cannonHubX;

    [SerializeField] private float m_rotationSpeedY = 60f;
    [SerializeField] private float m_rotationSpeedX = 30f;
    [SerializeField] private float m_aimThreshold = 2f;
    [SerializeField] private float m_maxElevationAngle = 60f;
    [SerializeField] private float m_minElevationAngle = -60f;
    [SerializeField] private float m_projectileSpeed = 20f;
    [SerializeField] private float m_maxPredictionTime = 3f;

    private Vector3 m_targetDirection;
    private Vector3 m_targetVelocity;

    protected override void Start() {
        if(m_shootPoint == null || m_cannonHubY == null || m_cannonHubX == null) {
            Debug.LogError($"CannonTower: {gameObject.name}: Some required components are not set!");
            enabled = false;
            return;
        }
        base.Start();
    }

    protected override IEnumerator ShootingCoroutine() {
        while(true) {
            if(IsEnemyInRange()) {
                AimAtTarget();
                if(IsAimedAtTarget() && CanShoot()) {
                    Shoot();
                    m_lastShotTime = Time.time;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private Vector3 PredictTargetPosition() {
        if(m_currentTarget == null) return Vector3.zero;

        if(m_currentTarget.TryGetComponent(out Rigidbody targetRb)) {
            m_targetVelocity = targetRb.velocity;
        } else {
            Debug.LogWarning($"CannonTower: {gameObject.name}: Rigidbody component not found on target!");
            return Vector3.zero;
        }

        Vector3 targetPosition = m_currentTarget.position;
        Vector3 toTarget = targetPosition - m_shootPoint.position;
        float targetDistance = toTarget.magnitude;
        float timeToTarget = targetDistance / m_projectileSpeed;

        timeToTarget = Mathf.Min(timeToTarget, m_maxPredictionTime);

        return targetPosition + m_targetVelocity * timeToTarget;
    }

    private void AimAtTarget() {
        if(m_currentTarget != null) {
            Vector3 predictedPosition = PredictTargetPosition();
            m_targetDirection = predictedPosition - m_cannonHubY.position;

            // Y rotation
            Vector3 targetDirectionXZ = new Vector3(m_targetDirection.x, 0f, m_targetDirection.z);
            Quaternion targetRotationY = Quaternion.LookRotation(targetDirectionXZ);
            m_cannonHubY.rotation = Quaternion.RotateTowards(m_cannonHubY.rotation, targetRotationY, m_rotationSpeedY * Time.fixedDeltaTime);

            // X rotation
            float targetAngleX = Mathf.Clamp(Vector3.SignedAngle(targetDirectionXZ, m_targetDirection, m_cannonHubY.right), m_minElevationAngle, m_maxElevationAngle);
            Quaternion targetRotationX = Quaternion.Euler(targetAngleX, 0f, 0f);
            m_cannonHubX.localRotation = Quaternion.RotateTowards(m_cannonHubX.localRotation, targetRotationX, m_rotationSpeedX * Time.fixedDeltaTime);
        }
    }

    private bool IsAimedAtTarget() {
        if(m_currentTarget == null) return false;

        Vector3 aimDirection = m_shootPoint.forward;
        return Vector3.Angle(aimDirection, m_targetDirection) < m_aimThreshold;
    }

    protected override void Shoot() {
        GameObject projectile = ObjectPoolManager.SpawnObject(m_projectilePrefab, m_shootPoint.position, m_shootPoint.rotation, PoolType.Projectiles);
        if(projectile.TryGetComponent(out Rigidbody projectileRb)) {
            projectileRb.velocity = m_shootPoint.forward * m_projectileSpeed;
        }
    }
}
