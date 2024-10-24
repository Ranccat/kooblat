using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
	[Header("Player Configs")]
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private int _maxHp = 6;
	[SerializeField] private float _flashDuration = 0.25f;

	[Header("Melee Attack")]
	[SerializeField] private int _meleeHitPoints = 1;
	[SerializeField] private float _meleeKnockbackForce = 1f;
	[SerializeField] private float _meleeAttackRange = 1f;

	[Header("Ranged Attack")]
	[SerializeField] private float _projectileSpeed = 5f;
	[SerializeField] private PlayerProjectile _projectile;
	[SerializeField] private int _rangedDamage = 1;
	[SerializeField] private float _rangedForce = 0f;

	[Header("Components")]
	[SerializeField] private Sprite _meleeWeapon;
	[SerializeField] private Sprite _rangedWeapon;
	[SerializeField] private SpriteRenderer _weaponSprite;
	[SerializeField] private Transform _attackOrigin;

	public int MaxHp { get { return _maxHp; } }
	public int Hp { get { return _hp; } }

	private int _hp;
	private int _maskHp; // gas mask HP
	private bool _facingLeft = true;
	private bool _isDead = false;
	private Vector2 _inputVec;
	private Vector2 _facingVec;
	private PlayerState _state;
	private Weapon _weapon;

	private Animator _anim;
	private Rigidbody2D _rb;
	private Camera _camera;
	private ColorFlash _colorFlash;

	private enum PlayerState
	{
		Idle = 0,
		Moving = 1,
		Attacking = 2,
		Dead = 3,
	}

	private enum Weapon
	{
		Melee = 0,
		Ranged = 1,
		Explosive = 2,
	}

	#region Lifecycle

    private void Awake()
	{ 
		_anim = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody2D>();
		_camera = Camera.main;
		_colorFlash = GetComponent<ColorFlash>();
	}

	private void Start()
	{
		_hp = _maxHp;
		_state = PlayerState.Idle;
		_weapon = Weapon.Melee;
	}

	private void Update()
	{
		if (_isDead)
			return;

		if (_state == PlayerState.Attacking)
			return;

		if (Input.GetKeyDown(KeyCode.C))
        {
			Coords coords = Game.Maze.GetCoords(transform.position);

			print($"{coords.Row}, {coords.Col}");
        }

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Attack();
		}

		for (int i = 0; i < 3; i++)
		{
			if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + (i))))
			{
				ChangeWeapon(i);
				break;
			}
		}

		// Movements
		_inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (_inputVec == Vector2.zero)
			_anim.SetBool("Moving", false);
		else
		{
			_anim.SetBool("Moving", true);
			_facingVec = _inputVec.normalized;
		}
		
		// Flip Character
		if ((_facingLeft && _inputVec.x > 0) || (!_facingLeft && _inputVec.x < 0))
			FlipCharacter();
	}

	void FixedUpdate()
	{
        if (_state == PlayerState.Dead || _state == PlayerState.Attacking)
            return;

        _rb.MovePosition(_rb.position + _inputVec * Time.fixedDeltaTime * _moveSpeed);
	}

	#endregion

	private void ChangeWeapon(int idx)
	{
		_weapon = (Weapon)idx;

		switch (idx)
		{
			case 0:
				_weaponSprite.sprite = _meleeWeapon;
				break;
			case 1:
				_weaponSprite.sprite = _rangedWeapon;
				break;
			case 2:
				_weaponSprite.sprite = null;
				break;
		}
		Game.UI.ChangeWeapon(idx);
	}

	public void OnToxicGas()
	{
		if (_maskHp > 0)
		{
			_maskHp -= 1;
			return;
		}

		OnDamage(new Damage(DamageType.Melee, 1, Vector2.zero, 0f));
	}

	#region Battle

	private void Attack()
	{
		_state = PlayerState.Attacking;

		switch (_weapon)
		{
			case Weapon.Melee:
				MeleeAttack();
				break;
			case Weapon.Ranged:
				RangedAttack();
				break;
			case Weapon.Explosive:
				PlaceExplosive();
				break;
		}
	}

	private void MeleeAttack()
	{
		_anim.SetTrigger("Attack_Melee");
	}

	private void RangedAttack()
	{
		_anim.SetTrigger("Attack_Ranged");
	}

	private void PlaceExplosive()
	{
		// TODO
	}

	public void OnDamage(Damage damage)
	{
		_hp -= damage.HitPoints;

		Game.UI.FillHearts(Mathf.Max(_hp, 0));

		_colorFlash.Flash(Color.red, _flashDuration);
		
		if (_hp <= 0)
		{
			OnDeath();
		}
	}

	private void OnDeath()
	{
		_anim.SetTrigger("Death");
		_isDead = true;
		// TODO : UI game over
	}

	#endregion

	#region Items & PowerUps

	public void AddExtraHeart()
	{
		_maxHp += 2;
		Game.UI.AddExtraHeart();
    }

	public void SpeedUp(float speedUpModifier)
	{
		_moveSpeed += speedUpModifier;
	}

	public void EquipGasMask(int amount)
	{
		_maskHp += amount;
	}

	#endregion

	#region Animations

	private void FlipCharacter()
	{
		_facingLeft = !_facingLeft;

		Vector3 scaler = transform.localScale;
		scaler.x *= -1;
		transform.localScale = scaler;
	}

	public void AnimMeleeAttackReset()
	{
		_anim.ResetTrigger("Attack_Melee");
		_state = PlayerState.Idle;
	}

	public void AnimRangedAttackReset()
	{
		_anim.ResetTrigger("Attack_Ranged");
		_state = PlayerState.Idle;
	}

	private void OnMeleeAttackHit()
	{
		Vector2 startPos = _attackOrigin.position;
		Vector2 endPos = startPos + (_facingVec * _meleeAttackRange);

		RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, ~(LayerMask.GetMask("Player") | LayerMask.GetMask("PlayerCollideOnly")));

		if (hit.collider != null)
		{
			if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
			{
				damageable.OnDamage(new Damage(DamageType.Melee, _meleeHitPoints, _facingVec, _meleeKnockbackForce));
			}
		}

		Debug.DrawLine(startPos, endPos, Color.red, .5f);
	}

	private void OnRangedAttackRelease()
	{
		Vector2 direction = _facingVec.normalized;
		Vector2 shootPos = (Vector2)_attackOrigin.position + _facingVec;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		angle -= 90f;
		Quaternion rotation = Quaternion.Euler(0, 0, angle);

		var projectile = Instantiate(_projectile, shootPos, rotation);
		Damage damage = new Damage(DamageType.Ranged, _rangedDamage, _facingVec, _rangedForce);
		projectile.SetPlayerProjectile(damage, _projectileSpeed);
	}

	#endregion
}
