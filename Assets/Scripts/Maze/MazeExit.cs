using UnityEngine;

public class MazeExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Game.Instance.OnMazeExitReached();
    }
}
