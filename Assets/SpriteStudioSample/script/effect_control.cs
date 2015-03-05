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
		GameObject go = null;

		switch( type )
		{
		case 0:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo01"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 1:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo02"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 4.0f,4.0f,1.0f);
			break;
		case 2:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo03"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 4.0f,4.0f,1.0f);
			break;
		case 3:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo04"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 4:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo05"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 5:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo06"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 6:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo07"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 7:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/Prefab/effect_comipo08"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			break;
		case 8:
			go = Instantiate(Resources.Load("Prefabs/hiteffect/hit_effect2"),new Vector3(x,y,-1),Quaternion.identity) as GameObject;
			break;

		}
		//生成したアニメデータは Camera2D Pixel (1080p) の　View　の下に配置しないと表示できない。
		//このアニメの親（View）を親に設定する必要がある
		go.transform.parent = view.transform;
	}

}
