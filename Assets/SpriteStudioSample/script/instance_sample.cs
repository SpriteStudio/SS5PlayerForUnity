using UnityEngine;
using System.Collections;

public class instance_sample : MonoBehaviour {
	//アニメーション制御クラス　
	private Script_SpriteStudio_Root ScriptRoot;

	int count = 0;		//時間のカウント
	int type = 0;		//アニメーションの種類
	int IDParts;		//インスタンスパーツのID
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//spritestudioルートクラスの取得.
		//spriteStudioRootを使用してアニメーションの制御を行います.
		if(null == ScriptRoot)
		{
			//初期化.
			ScriptRoot = Library_SpriteStudio.Utility.Parts.RootGetChild(gameObject);
			//アニメーション再生
			ScriptRoot.AnimationPlay(0);	
			//名前からインスタンスパーツのIDを取得する
			IDParts = ScriptRoot.IDGetParts("face_base");
		}
		
		count++;
		if ( ( count % 180 ) == 0 )
		{
			string NameAnimation = "";

			//インスタンスパーツが参照しているアニメーションを変更します
			//参照アニメーション(ssae)内でモーションを切り替えます。
			switch( type )
			{
			case 0:
				NameAnimation = "face2";
				type++;
				break;
			case 1:
				NameAnimation = "face3";
				type++;
				break;
			case 2:
				NameAnimation = "face1";
				type = 0;
				break;
			default:
				break;
			}
			//参照アニメの切り替えはアニメーションを停止して行ってください。
			ScriptRoot.AnimationStop();
			ScriptRoot.InstanceChange(IDParts,null,NameAnimation);
			ScriptRoot.AnimationPlay();	
		}
	
	}

}
