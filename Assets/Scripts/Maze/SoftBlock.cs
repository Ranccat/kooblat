using UnityEngine;
using System.Collections;

public class SoftBlock : BreakableBlockBase
{
    [SerializeField] private int _maxHp = 5;
    [SerializeField] private Sprite[] _sprites;

    private int _hp;

    private SpriteRenderer _renderer;

    private Transform _graphicsTr;
    private Coroutine _shakeRoutine;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _graphicsTr = transform.GetChild(0);
    }

    private void Start()
    {
        _hp = _maxHp;
    }

    public override void OnDamage(Damage damage)
    {
        if (damage.Type == DamageType.Ranged)
            return;

        _hp -= damage.HitPoints;

        if (_hp <= 0)
        {
            GetDestroyed();
            return;
        }

        float hpPercent = (float)_hp / _maxHp;
        int spriteIndex = _sprites.Length - Mathf.CeilToInt(hpPercent * _sprites.Length);
        _renderer.sprite = _sprites[spriteIndex];

        Shake(damage.Direction);
    }

    private const float shakeIntensity = .125f;
    private const float shakeDuration = .125f;

    private void Shake(Vector2 direction)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(direction));
    }

    private IEnumerator ShakeRoutine(Vector2 direction)
    {
        Vector2 startPos = _graphicsTr.localPosition = Vector2.zero;
        Vector2 shakePos = startPos + direction * shakeIntensity;

        float t = 0f;
        while (t < shakeDuration)
        {
            float y = -4 * t / Mathf.Pow(shakeDuration, 2) * (t - shakeDuration); // Quadratic function
            _graphicsTr.localPosition = Vector2.Lerp(startPos, shakePos, y);

            t += Time.deltaTime;
            yield return null;
        }

        _graphicsTr.localPosition = Vector2.zero;
    }
}
