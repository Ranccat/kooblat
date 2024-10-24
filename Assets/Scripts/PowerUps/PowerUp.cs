using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))] // TO BE DISCUSSED
public abstract class PowerUp : MonoBehaviour
{
    protected abstract void ApplyPowerUp();

	// On Pickup
	private void OnCollisionEnter2D(Collision2D coll)
	{
		ApplyPowerUp();
		Destroy(gameObject);
	}
}
