using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
	public AudioMixer audioMixer;

	public Slider bgmSlider;
	public Slider sfxSlider;

	private void Start()
	{
		SoundManager.Instance.PlayBGM("LobbyBGM");
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.S))
		{
			SoundManager.Instance.PlaySFX("ButtonSFX");
		}
	}
	public void ToggleAudioVolume()
	{
		AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
	}
	public void BGMAudioControl()
	{
		float sound = bgmSlider.value;

		if(sound == -40f)
		{
			audioMixer.SetFloat("BGM", -80);
		}
		else
		{
			audioMixer.SetFloat("BGM", sound);
		}
	}
	public void SFXAudioVolume()
	{
		float sound = sfxSlider.value;

		if (sound == -40f)
		{
			audioMixer.SetFloat("SFX", -80);
		}
		else
		{
			audioMixer.SetFloat("SFX", sound);
		}
	}
}
