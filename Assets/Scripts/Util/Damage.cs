using UnityEngine;

public struct Damage
{
    public DamageType Type { get; private set; }
    public int HitPoints { get; private set; }
    public Vector2 Direction { get; private set; }
    public float KnockbackForce { get; private set; }

    public Damage(DamageType type, int hitPoints, Vector2 direction, float knockbackForce)
    {
        Type = type;
        HitPoints = hitPoints;
        Direction = direction;
        KnockbackForce = knockbackForce;
    }
}

public enum DamageType
{
    Melee, Ranged, Explosion
}
