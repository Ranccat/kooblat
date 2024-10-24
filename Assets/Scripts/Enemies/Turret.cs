using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private float _rotateInterval = 1f;
    [SerializeField] private float _linecastInterval = 0.1f;
    [SerializeField] private float _searchDistance = 10f;
    [SerializeField] private float _waitDuration = 3f;
    [SerializeField] private float _attackInterval = 2f;

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public Transform shootOrigin;
    public Transform LinecastStartPoint;

    private Coroutine _searchingCoroutine;
    private Coroutine _linecastCoroutine;
    private Coroutine _waitingCoroutine;

	private const float _rotateSpeed = 120f;
    private bool _playerFound;
    private bool _isSearching;
    private bool _isRotating;

	private void Start()
    {
        _playerFound = false;
        _isSearching = true;
		_linecastCoroutine = StartCoroutine(LinecastRoutine());
		_searchingCoroutine = StartCoroutine(SearchRoutine());
	}

    private void WaitingToSearching()
    {
        StopCoroutine(_waitingCoroutine);
        Debug.Log("Changing to searching mode");

        _isSearching = true;
        _searchingCoroutine = StartCoroutine(SearchRoutine());
    }

    private void SearchingToWaiting()
    {
        StopCoroutine(_searchingCoroutine);
		Debug.Log("Changing to waiting mode");

		_isSearching = false;
        _waitingCoroutine = StartCoroutine(WaitingRoutine());
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, shootOrigin.position, shootOrigin.rotation);
    }

    private IEnumerator WaitingRoutine()
    {
        float waitStartTime = Time.time;
        float attackStartTime = Time.time - _attackInterval;
        
        while (true)
        {
            if (_playerFound == true)
            {
                waitStartTime = Time.time;
                if (Time.time - attackStartTime >= _attackInterval)
                {
                    Shoot();
                    attackStartTime = Time.time;
                }
            }
            else if (Time.time - waitStartTime >= _waitDuration)
            {
                WaitingToSearching();
            }

            yield return null;
        }
    }

    private IEnumerator SearchRoutine()
    {
        float angle = 90f;

        while (true)
        {
            _isRotating = false;
            yield return new WaitForSeconds(_rotateInterval);
            _isRotating = true;

			// transform.Rotate(0, 0, _rotateAngle);

			float startRotation = transform.eulerAngles.z;
			float endRotation = startRotation + angle;
			float currentRotation = startRotation;

			while (Mathf.Abs(currentRotation - endRotation) > 0.01f)
			{
				currentRotation = Mathf.MoveTowards(currentRotation, endRotation, _rotateSpeed * Time.deltaTime);
				transform.rotation = Quaternion.Euler(0, 0, currentRotation);

				yield return null;
			}

			transform.rotation = Quaternion.Euler(0, 0, endRotation);
		}
    }

    private IEnumerator LinecastRoutine()
    {
        while (true)
        {
            if (LinecastPlayer())
            {
                _playerFound = true;
                if (_isSearching && !_isRotating)
                {
                    SearchingToWaiting();
                }
            }
            else
            {
                _playerFound = false;
            }

            yield return new WaitForSeconds(_linecastInterval);
        }
    }

	private bool LinecastPlayer()
	{
		Vector2 startPos = LinecastStartPoint.position;
		Vector2 direction = LinecastStartPoint.up;

		int mask = LayerMask.GetMask("Player");
		RaycastHit2D hit = Physics2D.Linecast(startPos, startPos + direction * _searchDistance, mask);
		Debug.DrawLine(startPos, startPos + direction * _searchDistance, Color.red, _linecastInterval);

		if (hit.collider != null)
		{
			return true;
		}

		return false;
	}
}
