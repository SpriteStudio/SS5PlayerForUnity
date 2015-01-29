using UnityEngine;
using System.Collections;

public class hit_effect : MonoBehaviour {

	private camera2d Camera2DControl;
	Vector3 pos;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 3.0f);

		//ゲームコントロールスクリプトの取得
		var go = GameObject.Find("GameControl");
		Camera2DControl = go.GetComponent<camera2d>();

		//初期位置を保存
		pos = transform.position;

		Update ();	//座標尾更新するのに１ど呼ぶ
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 camera = Camera2DControl.GetCamera();
		Vector2 tmp = new Vector2(pos.x - camera.x, pos.y - camera.y);
		
		transform.position = tmp;

	}
}
