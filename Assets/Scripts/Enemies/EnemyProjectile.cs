using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	[Header("Configs")]
	private float _moveSpeed = 5f;

	private Damage _damage;

	private void Update()
	{
		transform.Translate(Vector2.up * _moveSpeed * Time.deltaTime);
	}

	public void SetDamage(Damage damage)
	{
		_damage = damage;
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.TryGetComponent(out IDamageable damageable))
		{
			damageable.OnDamage(_damage);
		}

		Destroy(gameObject);
	}
}
