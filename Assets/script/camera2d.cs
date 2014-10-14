using UnityEngine;
using System.Collections;

public class camera2d : MonoBehaviour {

	//camera
	private Vector2 gamecamera = new Vector2(0.0f, 0.0f);		//ゲーム内カメラの位置

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

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
		return( gamecamera );
	}
}
