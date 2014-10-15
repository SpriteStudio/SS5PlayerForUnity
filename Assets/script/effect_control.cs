using UnityEngine;
using System.Collections;

public class effect_control : MonoBehaviour {

	GameObject view;

	// Use this for initialization
	void Start () {
		view = GameObject.Find("View");	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	public void CreateEffect ( int type, float x, float y ) 
	{
		//エフェクトの生成
		var go = Instantiate(Resources.Load("Prefabs/effect2/effect001_Control"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
		//生成したアニメデータは Camera2D Pixel (1080p) の　View　の下に配置しないと表示できない。
		//このアニメの親（View）を親に設定する必要がある
		go.transform.parent = view.transform;
	}

}
