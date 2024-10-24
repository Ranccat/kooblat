using UnityEngine;

public abstract class BreakableBlockBase : MonoBehaviour, IDamageable
{
    protected int _row { private set; get; }
    protected int _col { private set; get; }

    public void Init(int row, int col)
    {
        _row = row;
        _col = col;
    }

    public abstract void OnDamage(Damage damage);

    protected virtual void GetDestroyed()
    {
        Destroy(gameObject);

        Game.Maze.NotifyBlockRemoved(_row, _col);
    }
}
