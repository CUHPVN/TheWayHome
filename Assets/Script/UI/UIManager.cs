using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	bool isPause = false;
	public static UIManager Instance { get; private set; }
	[SerializeField] private TextMeshProUGUI ScoreText;
	[SerializeField] private TextMeshProUGUI CoinCounter;
	[SerializeField] private Transform blacksetting;
	[SerializeField] private Transform setting;
	[SerializeField] private Slider bgmVolumeSlider;
	[SerializeField] private Slider sfxVolumeSlider;
	[SerializeField] private List<Button> buttons;

	public GameObject DeadScene;
	public GameObject ReviveScene;
	public GameObject PauseScene;
	public float Score = 0;
	public int Multiplyer;

	void Awake()
	{
		Instance = this;
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

		TransitionManager.Instance.PlayIn();
		AddEvent();
	}

	// Update is called once per frame
	void Update()
	{
		isPause = PauseScene.activeSelf;
		if (PowerUp.Instance.X2ScoreActive)
		{
			ScoreText.color = new Color32(58, 243, 91, 255);
		}
		else
		{
			ScoreText.color = Color.white;
		}
		ScoreText.text = $"{(int)Score}";
		CoinCounter.text = $"{CoinManager.Instance.GetCoin()}";
		if (StateManager.Instance.GetState() == StateManager.States.DeadState && !ReviveScene.activeSelf && !DeadScene.activeSelf)
		{
			Time.timeScale = 0f;
			ReviveScene.SetActive(true);
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && StateManager.Instance.GetState() != StateManager.States.DeadState)
		{
			isPause = !isPause;
			if (!isPause)
			{
				setting.gameObject.SetActive(false);
				blacksetting.gameObject.SetActive(false);
			}
			PauseScene.SetActive(isPause);
		}
	}
	private void FixedUpdate()
	{
		if (PowerUp.Instance.X2ScoreActive)
		{
			Score += 1f * Multiplyer * 0.3f * 2;
		}
		else
		{

			Score += 1f * Multiplyer * 0.3f;
		}
	}

	public float GetScore()
	{
		return Score;
	}
	private void AddEvent()
	{
		foreach (Button but in buttons)
		{
			but.onClick.AddListener(() => SoundManager.Instance.PlaySFX((int)SoundManager.SoundType.ButtonClick));
		}
	}
	public bool GetSetting()
	{
		return setting.gameObject.activeSelf;
	}
	public void SetVomume(float bgmVolume, float sfxVolume)
	{
		bgmVolumeSlider.value = bgmVolume;
		sfxVolumeSlider.value = sfxVolume;
	}
	public (float v1, float v2) GetVolume()
	{
		return (bgmVolumeSlider.value, sfxVolumeSlider.value);
	}
}
