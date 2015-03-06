using UnityEngine;
using System.Collections;

public class sseffect : MonoBehaviour {

	//スプライトスタジオアニメを操作するためのクラス
	private Script_SpriteStudio_PartsRoot spriteStudioRoot;
	private camera2d Camera2DControl;
	
	Vector3 pos;

	// Use this for initialization
	void Start () {
		//ゲームコントロールスクリプトの取得
		var go = GameObject.Find("GameControl");
		Camera2DControl = go.GetComponent<camera2d>();

		//初期位置を保存
		pos = transform.position;

	}
	
	// Update is called once per frame
	void Update () {

		//spritestudioルートクラスの取得
		//spriteStudioRootを使用してアニメーションの制御を行います。
		if(null == spriteStudioRoot)
		{
			int Count = transform.childCount;
			Transform InstanceTransformChild = null;
			for(int i=0; i<Count; i++)
			{
				InstanceTransformChild = transform.GetChild(i);
				spriteStudioRoot = InstanceTransformChild.gameObject.GetComponent<Script_SpriteStudio_PartsRoot>();
				if(null != spriteStudioRoot)
				{
					//初期化
					//アニメーションの終了割り込みを設定
					spriteStudioRoot.FunctionPlayEnd = AnimEnd;
					//アニメーションの再生
					spriteStudioRoot.AnimationPlay(0, 1, 0, 1.0f);	
					break;
				}
			}
		}

		Vector2 camera = Camera2DControl.GetCamera();
		Vector2 tmp = new Vector2(pos.x - camera.x, pos.y - camera.y);

		transform.position = tmp;
	}

	//アニメーションの終了コールバック
	public bool AnimEnd(GameObject ObjectControl)
	{
		return false;	//アニメ削除
	}

}
