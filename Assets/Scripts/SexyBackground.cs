using UnityEngine;
using System.Collections;

public class SexyBackground : MonoBehaviour
{
    [SerializeField] private Color[] _cycleColors;
    [SerializeField] private float _cycleTime = 5f;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(OozeSexinessRoutine());
    }

    private IEnumerator OozeSexinessRoutine()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time / _cycleTime, 1);
            _cam.backgroundColor = Color.Lerp(_cycleColors[0], _cycleColors[1], t);
            yield return null;
        }
    }
}
