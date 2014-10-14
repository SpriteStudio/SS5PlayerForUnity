using UnityEngine;
using System.Collections;

public class bg : MonoBehaviour {

	private camera2d Camera2DControl;

	//bg制御
	private GameObject background_front;						//BG
	private GameObject background_center;						//BG
	private GameObject background_back;							//BG
	private Vector3 background_front_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置
	private Vector3 background_center_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置
	private Vector3 background_back_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置


	// Use this for initialization
	void Start () {

		var go = GameObject.Find("GameControl");
		Camera2DControl = go.GetComponent<camera2d>();

		background_front = GameObject.Find("backgrounds_front");
		background_center = GameObject.Find("background_center");
		background_back = GameObject.Find("backgroung_back");

		background_front.transform.position = new Vector3(-81.08362f, 52.46774f, 169.4585f);
		background_center.transform.position = new Vector3(-48.76801f, 359.1096f, 119.4256f);
		background_back.transform.position = new Vector3(-81.75049f, 237.6096f, 405.6156f);

		background_front_initpos = background_front.transform.position;
		background_center_initpos = background_center.transform.position;
		background_back_initpos = background_back.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		move_bg( );
		
	}

	void move_bg( )
	{	
		Vector2 camerapos = Camera2DControl.GetCamera();

		float scale_x = 1.0f;
		float scale_y = 1.0f;
		Vector3 pos_fornt = new Vector3(background_front_initpos.x - ( camerapos.x * scale_x ), background_front_initpos.y - ( camerapos.y * scale_y ), background_front_initpos.z);
		background_front.transform.position = pos_fornt;
		
		scale_x = 0.3f;
		scale_y = 1.0f;
		Vector3 pos_center = new Vector3(background_center_initpos.x - ( camerapos.x * scale_x ), background_center_initpos.y - ( camerapos.y * scale_y ), background_center_initpos.z);
		background_center.transform.position = pos_center;
		
		scale_x = 0.2f;
		scale_y = 0.2f;
		Vector3 pos_back = new Vector3(background_back_initpos.x - ( camerapos.x * scale_x ), background_back_initpos.y - ( camerapos.y * scale_y ), background_back_initpos.z);
		background_back.transform.position = pos_back;

	}
}
