using UnityEngine;
using ObjectPool;

public abstract class BaseProjectile : MonoBehaviour {
    
    [SerializeField] protected float m_speed = 0.2f;
    [SerializeField] protected int m_damage = 10;

    protected virtual void Update() {
        Move();
    }

    protected virtual void Move(){}

    protected virtual void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null)
            return;

        damageable.TakeDamage(m_damage);
        ReturnToPool();
    }

    protected void ReturnToPool() {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
