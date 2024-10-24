using UnityEngine;
using System.Collections.Generic;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private Explosion _explosionPrefab;

    [SerializeField] private Sprite _explosionCenterSprite;
    [SerializeField] private Sprite _explosionMiddleSprite;
    [SerializeField] private Sprite _explosionEndSprite;

    public void MakeExplosion(Coords coords, int hitPoints, int range, float duration, float knockBack)
    {
        HashSet<Collider2D> damagedEntities = new HashSet<Collider2D>();

        
    }

    // calculate damage based of off distance falloff
    //private Damage CalculateDamageFallOff(int centerHitPoints, float centerKnockBack, int distance)
    //{
        
    //}
}
