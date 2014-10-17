using UnityEngine;
using System.Collections;

public class sound : MonoBehaviour {

	public AudioClip audioClip_hit1;
	public AudioClip audioClip_atk;
	public AudioClip audioClip_dead1;
	public AudioClip audioClip_dead2;
	public AudioClip audioClip_run;
	public AudioClip audioClip_jump;

	private AudioSource audioSource;

	int[] sewait = new int[10];

	public enum SE_TYPE
	{
		HIT1 = 0,
		ATK,
		DEAD1,
		DEAD2,
		RUN,
		JUMP,
	}
	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();

		int i;
		for ( i = 0; i < 10; i++)
		{
			sewait[i] = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {

		int i;
		for ( i = 0; i < 10; i++)
		{
			sewait[i]--;
			if ( sewait[i] < 0 )
			{
				sewait[i] = 0;
			}
		}
	}

	public void PlaySE( SE_TYPE type )
	{
		if ( sewait[(int)type] > 0 )
		{
			return;
		}
		sewait[(int)type] = 3;

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
		case SE_TYPE.RUN:
			audioSource.clip = audioClip_run;
			break;
		case SE_TYPE.JUMP:
			audioSource.clip = audioClip_jump;
			break;
		}
		audioSource.PlayOneShot(audioSource.clip);
//		audioSource.Play();
	}
}
