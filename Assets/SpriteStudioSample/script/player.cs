using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
	//アニメーション制御クラス　
	private Script_SpriteStudio_Root ScriptRoot;

	//アニメーションに含まれるモーション番号
	enum AnimationType
	{
		ATTACK1 = 0,
		ATTACK2,
		ATTACK3,
		CHARGE,
		DAMEGE,
		DEAD1,
		DEAD2,
		DEFENSE,
		JUMP_AIR,
		JUMP_ALL,
		JUMP_ATTACK1,
		JUMP_ATTACK2,
		JUMP_END,
		JUMP_KICK,
		JUMP_PUNCH,
		JUMP_START,
		KICK1,
		KICK2,
		NO_ANIME,
		PANCH1,
		PANCH2,
		PIYO,
		POSE1,
		POSE2,
		RUN,
		SIT,
		SIT_ATTACK1,
		SIT_ATTACK2,
		SIT_KICK,
		SIT_PANCH,
		SITDOWN,
		STANCE,
		STANDUP,
		WAIT,
		WALK
	}
	private AnimationType motion = AnimationType.STANCE;		//表示モーション
	bool press = false;	//初回入力のみ取得.

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
			//アニメーション終了コールバックを設定.
			ScriptRoot.FunctionPlayEnd = AnimEnd;
			//アニメーション再生
			ScriptRoot.AnimationPlay((int)AnimationType.STANCE, 1, 0, 1.0f);	
		}
		//入力によってモーションを変更する
		if (Input.GetKey ("z")) {
			if (press == false )
			{
				motion = AnimationType.ATTACK1;
				ScriptRoot.AnimationPlay((int)AnimationType.ATTACK1, 1, 0, 1.0f);	
			}
			press = true;
		}
		else if (Input.GetKey ("x")) {
			if (press == false )
			{
				motion = AnimationType.ATTACK2;
				ScriptRoot.AnimationPlay((int)AnimationType.ATTACK2, 1, 0, 1.0f);	
			}
			press = true;
		}
		else if (Input.GetKey ("c")) {
			if (press == false )
			{
				GameObject go = null;
				go = Instantiate(Resources.Load("Prefabs/hiteffect/effect_comipo/effect_comipo01_Control"),new Vector3(-400,0,0),Quaternion.identity) as GameObject;
				go.transform.localScale = new Vector3( 2.5f,2.5f,1.0f);
			}
			press = true;
		}
		else
		{
			press = false;
		}
	}
	//アニメーションの終了コールバック
	public bool AnimEnd(Script_SpriteStudio_Root InstanceRoot,GameObject ObjectControl)
	{
		//モーションが終了したら次のモーションを遷移させる.
		switch(motion)
		{
		case AnimationType.STANCE:
			motion = AnimationType.STANCE;
			ScriptRoot.AnimationPlay((int)AnimationType.STANCE, 1, 0, 1.0f);	
			break;
		case AnimationType.ATTACK1:
			motion = AnimationType.STANCE;
			ScriptRoot.AnimationPlay((int)AnimationType.STANCE, 1, 0, 1.0f);	
			break;
		case AnimationType.ATTACK2:
			motion = AnimationType.STANCE;
			ScriptRoot.AnimationPlay((int)AnimationType.STANCE, 1, 0, 1.0f);	
			break;
		default:
			break;
		}
		return true;
	}
}
