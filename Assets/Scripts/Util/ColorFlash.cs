using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorFlash : MonoBehaviour
{
    [Tooltip("Objects must be children objects under the object with this component")]
    [SerializeField] private SpriteRenderer[] _excludedRenderers;

    private SpriteRenderer[] _renderers;
    private Color[] _originalColors;

    private Coroutine _flashRoutine;

    private void Awake()
    {
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();

        _renderers = new SpriteRenderer[allRenderers.Length - _excludedRenderers.Length];
        _originalColors = new Color[allRenderers.Length - _excludedRenderers.Length];

        HashSet<SpriteRenderer> exludedObjectSet = new HashSet<SpriteRenderer>(_excludedRenderers);

        int i = 0;
        foreach (SpriteRenderer renderer in allRenderers)
        {
            if (!exludedObjectSet.Contains(renderer))
            {
                _renderers[i] = renderer;
                _originalColors[i] = renderer.color;
                i++;
            }
        }
    }

    public void Flash(Color color, float duration)
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        foreach (SpriteRenderer renderer in _renderers)
            renderer.color = color;

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].color = _originalColors[i];
    }
}
