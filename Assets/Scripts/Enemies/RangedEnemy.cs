using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private EnemyProjectile _projectile;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _maxShootDistance = 5f;
    [SerializeField] private float _fireCooldown = 1f;
    [SerializeField] private float _knockbackForce = 0f;
    [SerializeField] private int _damage;

    private Animator _anim;

    private Transform _playerTr;

    private float _lastFiredTime;
    private bool _facingLeft = true;

    protected override void Awake()
    {
        base.Awake();

        _anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        _playerTr = Game.Player.transform;
    }

    private void Update()
    {
        // has line of sight
        if (Vector2.Distance(transform.position, _playerTr.position) <= _maxShootDistance
            && !Physics2D.Linecast(transform.position, _playerTr.position, LayerMask.GetMask("Block")))
        {
            _chasePlayer.enabled = false;

            // ready to fire, fire
            if (Time.time - _lastFiredTime >= _fireCooldown)
            {
                Fire();
            }
        }
        else // keep chasing
            _chasePlayer.enabled = true;

		Vector2 facingDir = _chasePlayer.GetDirection();

		if ((_facingLeft && facingDir.x > 0) || (!_facingLeft && facingDir.x < 0))
			FlipCharacter();
	}

	private void FlipCharacter()
	{
		_facingLeft = !_facingLeft;

		Vector3 scaler = transform.localScale;
		scaler.x *= -1;
		transform.localScale = scaler;
	}

	private void Fire()
    {
        _lastFiredTime = Time.time;

        _chasePlayer.enabled = false;

        _anim.SetTrigger("Attack");
    }

    public override void OnDamage(Damage damage)
    {
        _hp -= damage.HitPoints;
        if (_hp <= 0)
        {
			OnDeath();
		}
    }

    private void OnDrawGizmos()
    {
        if (_playerTr == null)
            return;

        if (Vector2.Distance(transform.position, _playerTr.position) <= _maxShootDistance
            && !Physics2D.Linecast(transform.position, _playerTr.position, LayerMask.GetMask("Block")))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _playerTr.position);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _maxShootDistance);
        }
    }

    public void OnAttackRelease()
    {
        Vector2 direction = (Game.Player.transform.position - transform.position).normalized;
		Vector2 shootPos = (Vector2)transform.position;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		angle -= 90f;
		Quaternion rotation = Quaternion.Euler(0, 0, angle);

		var projectile = Instantiate(_projectile, shootPos, rotation);
		projectile.SetDamage(new Damage(DamageType.Ranged, _damage, direction, _knockbackForce));
	}

    public void OnAttackEnd()
    {
        _chasePlayer.enabled = true;
    }
}
