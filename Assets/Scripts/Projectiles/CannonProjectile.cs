using UnityEngine;
using ObjectPool;

public class CannonProjectile : BaseProjectile {

    [SerializeField] private float m_lifeTime = 5f;
    private float m_enableTime = 0;

    private void OnEnable() {
        m_enableTime = Time.time;
    }

    protected override void Update() {
        base.Update();
        if(m_enableTime + m_lifeTime < Time.time) {
            ReturnToPool();
        }
    }
}
