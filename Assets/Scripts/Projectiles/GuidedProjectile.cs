using UnityEngine;
using ObjectPool;

public class GuidedProjectile : BaseProjectile {
	
	private Transform m_target;
	public Transform Target {
		get => m_target;
		set => m_target = value;
	}

	protected override void Update() {
		if(m_target == null || !m_target.gameObject.activeSelf) {
			ReturnToPool();
			return;
		}
		base.Update();
	}

	protected override void Move() {
		Vector3 direction = (m_target.position - transform.position).normalized;
		transform.Translate(direction * m_speed * Time.deltaTime);
	}
}
