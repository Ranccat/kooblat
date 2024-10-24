using UnityEngine;

public class ToxicGas : MonoBehaviour
{
    public int row { get; private set; }
    public int col { get; private set; }

    public bool isFullIntensity { get; private set; }

    private ParticleSystem _particleEffects;

    private const float _damageInterval = 1f;
    
    // Last recorded time where any given Toxic Gas instance hurt the player.
    // static, shared over all instances
    private static float _lastDamageTime;

    private void Awake()
    {
        _particleEffects = GetComponentInChildren<ParticleSystem>();
    }

    public void Init(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void SetFullIntensity()
    {
        isFullIntensity = true;

        var emission = _particleEffects.emission;
        emission.rateOverTime = 10f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isFullIntensity)
        {
            // Damage the player
            if (Time.time - _lastDamageTime >= _damageInterval)
            {
                _lastDamageTime = Time.time;
                Game.Player.OnToxicGas();
            }
        }
    }
}
