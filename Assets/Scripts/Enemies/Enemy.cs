using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int _initHp = 5;

    // Score to be added to the player upon killing this enemy
    [SerializeField] private int _score = 10;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private Vector2 _offset;

    protected ChasePlayer _chasePlayer;

    protected int _hp;

    protected virtual void Awake()
    {
        _chasePlayer = GetComponent<ChasePlayer>()
            .SetSpeed(_speed)
            .SetOffset(_offset);
    }

    protected virtual void Start()
    {
        _hp = _initHp;
    }

    public abstract void OnDamage(Damage damage);

    protected virtual void OnDeath()
    {
        Game.AddScore(_score);
        Destroy(gameObject);
    }
}
