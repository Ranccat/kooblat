using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
	[Header("Configs")]
	private float _moveSpeed = 5f;

	private Damage _damage;

	public void SetPlayerProjectile(Damage damage, float speed)
	{
		_damage = damage;
		_moveSpeed = speed;
	}	

	private void Update()
	{
		transform.Translate(Vector2.up * _moveSpeed * Time.deltaTime);
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
