using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Configs")]
    private float _moveSpeed = 5f;

	private void Update()
	{
		transform.Translate(Vector2.up * _moveSpeed * Time.deltaTime);
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			// TODO: Player Hit
			Debug.Log("Player Hit!");
		}

		Destroy(gameObject);
	}
}
