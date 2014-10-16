using UnityEngine;
using System.Collections;

public class sound : MonoBehaviour {

	public AudioClip audioClip_hit1;
	public AudioClip audioClip_atk;
	public AudioClip audioClip_dead1;
	public AudioClip audioClip_dead2;
	private AudioSource audioSource;

	public enum SE_TYPE
	{
		HIT1 = 0,
		ATK,
		DEAD1,
		DEAD2,
	}
	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlaySE( SE_TYPE type )
	{
		switch( type )
		{
		case SE_TYPE.HIT1:
			audioSource.clip = audioClip_hit1;
			break;
		case SE_TYPE.ATK:
			audioSource.clip = audioClip_atk;
			break;
		case SE_TYPE.DEAD1:
			audioSource.clip = audioClip_dead1;
			break;
		case SE_TYPE.DEAD2:
			audioSource.clip = audioClip_dead2;
			break;
		}
		audioSource.Play();
	}
}
