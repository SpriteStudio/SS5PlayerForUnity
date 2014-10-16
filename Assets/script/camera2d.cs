using UnityEngine;
using System.Collections;

public class camera2d : MonoBehaviour {

	//camera
	private Vector2 gamecamera = new Vector2(0.0f, 0.0f);		//ゲーム内カメラの位置

	//シェイク
	int shake = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		shake--;
		if ( shake < 0 )
		{
			shake = 0;
		}
	}

	public void SetShake( int time )
	{
		shake = time;
	}

	public void SetCamera( Vector2 vec )
	{
		gamecamera = vec;
		if ( gamecamera.x < -1800.0f )
		{
			gamecamera.x = -1800.0f;
		}
		if ( gamecamera.x > 1800.0f )
		{
			gamecamera.x = 1800.0f;
		}
		if ( gamecamera.y < -100.0f )
		{
			gamecamera.y = -100.0f;
		}
		if ( gamecamera.y > 300.0f )
		{
			gamecamera.y = 300.0f;
		}
	}
	public Vector2 GetCamera( )
	{
		float ex = 0;
		float ey = 0;

		if ( shake > 0 )
		{
			int bai = 3;
			ex = (float)UnityEngine.Random.Range (-shake * bai, shake * bai);
			ey = (float)UnityEngine.Random.Range (-shake * bai, shake * bai);
		}

		Vector2 rc = gamecamera;
		rc.x += ex;
		rc.y += ey;

		return( rc );
	}
}
