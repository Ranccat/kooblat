using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Config")]
    [SerializeField] private float _attackDistance = 1.25f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _knockbackForce = 1f;

    [Header("Others")]
    [SerializeField] private Transform _attackOrigin;

	private Animator _anim;

    private bool _facingLeft = true;

    protected override void Awake()
    {
        base.Awake();

        _chasePlayer
            .SetStoppingDistance(_attackDistance)
            .SetOnDistanceReached(() => Attack());

        _anim = GetComponent<Animator>();
    }

    private void Attack()
    {
        _chasePlayer.enabled = false;

        _anim.SetTrigger("Attack");
    }

    private void Update()
    {
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

	public override void OnDamage(Damage damage)
    {
		_hp -= damage.HitPoints;
		if (_hp <= 0)
		{
			OnDeath();
		}
	}

    public void OnAttackEnd()
    {
        _chasePlayer.enabled = true;
    }

    public void OnAttackHit()
    {
        Vector2 startPos = _attackOrigin.position;
        Vector2 direction = ((Vector2)Game.Player.transform.position - startPos).normalized;
        Vector2 endPos = startPos + (direction * _attackDistance);

        RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.OnDamage(new Damage(DamageType.Melee, _damage, direction, _knockbackForce));
            }
        }

        Debug.DrawLine(startPos, endPos, Color.green, .5f);
	}
}
