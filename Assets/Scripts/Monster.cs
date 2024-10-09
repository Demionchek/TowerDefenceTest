using UnityEngine;
using ObjectPool;

public class Monster : MonoBehaviour, IDamageable {

	[SerializeField] private float m_speed = 5f;
	[SerializeField] private int m_maxHP = 30;
	[SerializeField] private float m_reachDistance = 0.3f;

	private Transform m_moveTarget;
	private Rigidbody m_rigidbody;
	private int m_hp;

	private void OnEnable() {
		m_hp = m_maxHP;
	}

	private void Awake() {
		m_rigidbody = GetComponent<Rigidbody>();
		if(m_rigidbody == null) {
			Debug.LogError($"Monster: {gameObject.name}: Rigidbody component not found!");
		}
	}

	public void TakeDamage(int damage) {
		m_hp -= damage;
		if (m_hp <= 0) {
			Die();
		}
	}

	public void SetMoveTarget(Transform target) {
		m_moveTarget = target;
	}

	private void FixedUpdate() {
		if(m_rigidbody == null) return;

		if(m_moveTarget == null) {
			Debug.LogWarning($"Monster: {gameObject.name}: m_moveTarget is not set!");
			return;
		}

		if(Vector3.Distance(transform.position, m_moveTarget.position) <= m_reachDistance) {
			Die();
			return;
		}

		Vector3 direction = (m_moveTarget.position - transform.position).normalized;
		m_rigidbody.velocity = direction * m_speed;
	}

	private void Die() {
		ObjectPoolManager.ReturnObjectToPool(gameObject);
	}
}
