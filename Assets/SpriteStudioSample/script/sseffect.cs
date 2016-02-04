using UnityEngine;
using System.Collections;

public class sseffect : MonoBehaviour {

	//スプライトスタジオアニメを操作するためのクラス
	private Script_SpriteStudio_Root ScriptRoot;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		//spritestudioルートクラスの取得
		//spriteStudioRootを使用してアニメーションの制御を行います。
		if(null == ScriptRoot)
		{
			//初期化.
			ScriptRoot = Library_SpriteStudio.Utility.Parts.RootGetChild(gameObject);
			//アニメーション終了コールバックを設定.
			ScriptRoot.FunctionPlayEnd = AnimEnd;
			//アニメーション再生
			ScriptRoot.AnimationPlay(0, 1, 0, 1.0f);	
		}
	}

	//アニメーションの終了コールバック
	public bool AnimEnd(Script_SpriteStudio_Root InstanceRoot,GameObject ObjectControl)
	{
		return false;	//アニメ削除
	}

}
