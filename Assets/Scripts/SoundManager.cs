using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance = null;

	public AudioSource efxSource;
	public AudioSource musicSource;
	public float lowPitchrange = .95f;
	public float highPitchrange = 1.05f;


	void Awake () {

		if (instance == null) {
			instance = this;
		} else if (instance != this) {
				Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}

	public void PlaySingle(AudioClip clip){
		efxSource.clip = clip;
		efxSource.Play ();
	}

	public void	RandomizeSfx (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchrange, highPitchrange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips [randomIndex];
		efxSource.Play ();
	}
}
