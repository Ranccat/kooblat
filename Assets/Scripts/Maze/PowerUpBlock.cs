using System.Collections.Generic;
using UnityEngine;

public class PowerUpBlock : BreakableBlockBase
{
    [SerializeField] private List<PowerUp> _powerUps = new List<PowerUp>();
    private PowerUp _powerUp;

    public override void OnDamage(Damage damage)
    {
        if (damage.Type != DamageType.Ranged)
            GetDestroyed();
    }

	private void Awake()
	{
		int idx = Random.Range(0, _powerUps.Count);
		_powerUp = _powerUps[idx];
	}

	protected override void GetDestroyed()
    {
        base.GetDestroyed();

        Instantiate(_powerUp, transform.position, Quaternion.identity);
    }
}
