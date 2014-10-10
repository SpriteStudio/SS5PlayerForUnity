using UnityEngine;
using System.Collections;

public class sseffect : MonoBehaviour {

	//スプライトスタジオアニメを操作するためのクラス
	//Unity側で関連をつけておく
	public Script_SpriteStudio_PartsRoot spriteStudioRoot;

	// Use this for initialization
	void Start () {
/*
		//ルートパーツの取得
		spriteStudioRoot = GetComponentInChildren<Script_SpriteStudio_PartsRoot>();

		//アニメーションの終了割り込みを設定
		spriteStudioRoot.FunctionPlayEnd = AnimEnd;
		//アニメーションの再生
		spriteStudioRoot.AnimationPlay(0, 1, 0, 1.0f);	
*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//アニメーションの終了コールバック
	public bool AnimEnd(GameObject ObjectControl)
	{
		return false;	//アニメ削除
	}

}
