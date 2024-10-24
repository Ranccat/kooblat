using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerEx : MonoBehaviour
{
    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeart;
	[SerializeField] private Sprite halfHeart;
	[SerializeField] private Sprite emptyHeart;

    [Header("Heart List")]
	[SerializeField] private List<Image> hearts = new List<Image>();

	[Header("Transforms")]
    [SerializeField] private Transform heartPanel;
	[SerializeField] private List<RectTransform> weaponList = new List<RectTransform>();

    [Header("Texts")]
	[SerializeField] private Text timerText;
	[SerializeField] private Text scoreText;

    [Header("Frame")]
	[SerializeField] private RectTransform frame;

    private enum HeartType
    {
        Empty = 0,
        Half = 1,
        Full = 2,
    }

    private Dictionary<HeartType, Sprite> _typeToSprite = new Dictionary<HeartType, Sprite>();
	private int _lastMaxHpIdx = 0; // current max hp index
	private int _finalMaxHpIdx = 6; // UI max hp count index

	private void Awake()
	{
		_typeToSprite[HeartType.Empty] = emptyHeart;
		_typeToSprite[HeartType.Half] = halfHeart;
		_typeToSprite[HeartType.Full] = fullHeart;
	}

	private void Start()
    {
        UpdateScore(0);
        UpdateTimer(0f);

        frame.anchoredPosition = weaponList[0].anchoredPosition;
    }

    // Player should call this method on its initialization
    // ex) start HP 6 makes 3 full heart sprites on UI
    public void InitPlayerUI()
    {
        int startHp = Game.Player.MaxHp;
        int count = startHp / 2;
		for (int i = 0; i < count; i++)
		{
			AddExtraHeart();
		}
        FillHearts(startHp);
	}

    public void ChangeWeapon(int weapon)
    {
		frame.anchoredPosition = weaponList[weapon].anchoredPosition;
	}

    // Add a new empty heart
    public void AddExtraHeart()
    {
        // hit the max heart count
        if (_lastMaxHpIdx == _finalMaxHpIdx)
            return;

        hearts[_lastMaxHpIdx].sprite = _typeToSprite[HeartType.Empty];
		hearts[_lastMaxHpIdx].color = new Color(1, 1, 1, 1);

        _lastMaxHpIdx++;
    }

    // Fill hearts based on player's current HP
    public void FillHearts(int amount)
    {
        int full = amount / 2;
        bool half = amount % 2 == 1;

        for (int i = 0; i < full; i++)
        {
			hearts[i].sprite = _typeToSprite[HeartType.Full];
		}

        int next = full;
        if (half)
        {
			hearts[full].sprite = _typeToSprite[HeartType.Half];
            next++;
		}
            
        for (int i = next; i < _lastMaxHpIdx; i++)
        {
            hearts[i].sprite = _typeToSprite[HeartType.Empty];
        }
    }

    // Game class should call this function every second
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = string.Format("{0:000000}", score);
    }
}
