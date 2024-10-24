using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    private Damage _damage;
    private HashSet<Collider2D> _damagedEntities;
    private SpriteRenderer _renderer;

    public void Init(HashSet<Collider2D> damagedEntities)
    {
        _damagedEntities = damagedEntities;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_damagedEntities.Contains(collision))
            return;

        if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.OnDamage(_damage);
            _damagedEntities.Add(collision);
        }
    }
}
