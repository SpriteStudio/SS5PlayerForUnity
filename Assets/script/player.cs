using UnityEngine;
using UnityEditor;
using System.Collections;

//プレイヤークラス
public class player : MonoBehaviour {

	//スプライトスタジオアニメを操作するためのクラス
	//Unity側で関連をつけておく
	public Script_SpriteStudio_PartsRoot spriteStudioRoot;

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
		STANCE,
		WAIT,
		WALK
	}

	//キャラクター制御
	// 速度
	private Vector2 SPEED = new Vector2(10.0f, 10.0f);			//移動速度
	private AnimationType motion = AnimationType.STANCE;		//表示モーション
	private bool isground = false;								//地面に着地しているか？
	private float ground_y = -500.0f;							//地面の位置
	private float gravitiy = 98.0f;								//重力加速度
	private float jump_force = 0;								//ジャンプ力
	private float jump_speed = 0;								//ジャンプ中の移動速度
	private Vector2 player_pos = new Vector2(0.0f, -500.0f);	//プレイヤーの位置（表示位置とは別）
	private float init_scale = 0.7f;							//プレイヤーの初期スケール
	private int direction = 0;									//向き 0:左　1:右
	private bool anime_end = false;								//アニメーションの終了フラグ
	private int attack_wait_init = 20;							//攻撃の硬直時間設定値
	private int attack_wait = 0;								//攻撃の硬直時間
	private int ren_attack_count = 0;							//連続攻撃の回数

	//コマンド入力
	private bool dash = false;				//ダッシュ中か？
	private int dash_command_count = 0;		//前回のキー入力からの時間
	private int dash_command_time = 25;		//コマンド成立時間
	private int dash_oldinput_key = 0;		//前回入力されたキー

	//camera
	private Vector2 gamecamera = new Vector2(0.0f, 0.0f);		//ゲーム内カメラの位置

	//bg制御
	private GameObject background_front;						//BG
	private GameObject background_center;						//BG
	private GameObject background_back;							//BG
	private Vector3 background_front_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置
	private Vector3 background_center_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置
	private Vector3 background_back_initpos = new Vector3(0.0f, 0.0f, 0.0f);	//BG初期位置

	//ゲームコントロール
	private gamemain GameControl;

	// Use this for initialization
	void Start () 
	{
		//ゲームコントロールスクリプトの取得
		var go = GameObject.Find("GameControl");
		GameControl = go.GetComponent<gamemain>();


		//BG初期化
		//名前からGameObjectを取得
		background_front = GameObject.Find("backgrounds_front");
		background_center = GameObject.Find("background_center");
		background_back = GameObject.Find("backgroung_back");
		//初期位置を設定
		background_front.transform.position = new Vector3(-81.08362f, 52.46774f, 169.4585f);
		background_center.transform.position = new Vector3(-48.76801f, 359.1096f, 119.4256f);
		background_back.transform.position = new Vector3(-81.75049f, 237.6096f, 405.6156f);
		//初期値を保存
		background_front_initpos = background_front.transform.position;
		background_center_initpos = background_center.transform.position;
		background_back_initpos = background_back.transform.position;


		//アニメーションの終了割り込みを設定
		spriteStudioRoot.FunctionPlayEnd = AnimEnd;
		//初期アニメーションを設定
		set_motion( AnimationType.STANCE, true);

		direction = 0;									//左向き


	}

	//アニメーションの終了コールバック
	public bool EffectAnimEnd(GameObject ObjectControl)
	{
		return false;
	}

	// Update is called once per frame
	void Update () 
	{
		// for test code 
		//ボタンを押したら攻撃
		if (GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_1)) {
//			set_motion(AnimationType.JUMP_AIR, true);
			//エフェクトの生成（パーティクルシステム）
//			GameObject hit = Object.Instantiate(hit_effect) as GameObject;
//			hit.transform.position = new Vector3(0.0f, 0.0f, -30.0f);
/*
			//エフェクトの生成
//			Instantiate(Resources.Load("Prefabs/effect2/effect001_Control"),this.transform.position + new Vector3(0,2,0),Quaternion.identity);
			var go = Instantiate(Resources.Load("Prefabs/effect2/effect001_Control"),new Vector3(0,0,0),Quaternion.identity) as GameObject;
			//生成したアニメデータは Camera2D Pixel (1080p) の　View　の下に配置しないと表示できない。
			//このアニメの親（View）を親に設定する必要がある
			go.transform.parent = transform.parent;
			//アニメを制御するルートパーツの取得
			var rootPart = go.GetComponentInChildren<Script_SpriteStudio_PartsRoot>();
			//アニメーションの終了割り込みを設定
			rootPart.FunctionPlayEnd = EffectAnimEnd;
			//アニメーションの再生
			rootPart.AnimationPlay(0, 1, 0, 1.0f);	
*/
		}


		//アニメーションの終了を監視
		if (anime_end == true) {
			anime_end = false;
			//次のモーションを設定
			set_next_motion();
		}
			

		// 現在位置をPositionに代入
//		Vector2 Position = transform.position;
		Vector2 Position = player_pos;

		//コマンド処理
		//同じ方向を２回連続で入力したらダッシュ
		if (GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_LEFT)) {
			if (dash_command_count > 0) {
				if (dash_oldinput_key == -1) {
					dash = true;
				}
			}
			dash_command_count = dash_command_time;
			dash_oldinput_key = -1;
		}
		else if (GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_RIGHT))
		{
			if (dash_command_count > 0) {
				if (dash_oldinput_key == 1) {
					dash = true;
				}
			}
			dash_command_count = dash_command_time;
			dash_oldinput_key = 1;
		}
		dash_command_count--;
		if ( dash_command_count < 0 )
		{
			dash_command_count = 0;
			dash_oldinput_key = 0;
		}

		//プレイヤーのキー入力処理
		bool ismove = false;
		if (is_wait () == false) {
			if (isground == true) {
				// 左キーを押し続けていたら
				if (GameControl.IsPressKey ((int)gamemain.INPUTBUTTON.BUTTON_LEFT)) {
					// 代入したPositionに対して加算減算を行う
					if (dash == true) {
						Position.x -= SPEED.x * 2.0f;
					} else {
						Position.x -= SPEED.x;
					}
					direction = 0;
					ismove = true;
				} else if (GameControl.IsPressKey ((int)gamemain.INPUTBUTTON.BUTTON_RIGHT)) { // 右キーを押し続けていたら
					// 代入したPositionに対して加算減算を行う
					if (dash == true) {
						Position.x += SPEED.x * 2.0f;
					} else {
						Position.x += SPEED.x;
					}
					direction = 1;
					ismove = true;
				}
				if (ismove == false)
				{
					//移動入力がない場合ダッシュ終了
					dash = false;
				}
			}
			if (GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_UP)) {
				// 上キーを押し続けていたら
				if (isground == true) {
					//地面にいたらジャンプを行う
					jump_force = 2500.0f;
					set_motion(AnimationType.JUMP_START);

					if (ismove == true) {
						if (dash == true) {
							if (direction == 0) {
								jump_speed = -SPEED.x * 2.0f;								//ジャンプ中の移動速度
							} else if (direction == 1) {
								jump_speed = SPEED.x * 2.0f;								//ジャンプ中の移動速度
							}
						} else {
							if (direction == 0) {
								jump_speed = -SPEED.x * 1.0f;								//ジャンプ中の移動速度
							} else if (direction == 1) {
								jump_speed = SPEED.x * 1.0f;								//ジャンプ中の移動速度
							}
						}
					} else {
						jump_speed = 0;								//ジャンプ中の移動速度
					}

				}
			} else if(GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_DOWN)){
				// 下キーを押し続けていたら
			}
		}
		//ボタンを押したら攻撃
		if ( (attack_wait == 0) || (is_wait () == false) ){
			if (GameControl.IsPushKey ((int)gamemain.INPUTBUTTON.BUTTON_1)) {
				attack_wait = attack_wait_init;							//攻撃の硬直時間設定値
				ren_attack_count++;										//連続攻撃の回数

				if (isground == true) {
					if ((ren_attack_count % 2) == 1) {
						set_motion (AnimationType.ATTACK1);
					} else {
						set_motion (AnimationType.ATTACK2);
					}
				} else {
					set_motion (AnimationType.JUMP_ATTACK1);
				}
			}
		}
		attack_wait--;
		if ( attack_wait < 0 )
		{
			attack_wait = 0;
		}

		//移動範囲を設定
		if (Position.x < -3000.0f) {
			Position.x = -3000.0f;
		}
		if (Position.x > 3000.0f) {
			Position.x = 3000.0f;
		}

		//jump制御
		jump_force = jump_force - gravitiy;
		Position.y += ( jump_force * 0.02f ) ; 
		if ( Position.y < ground_y ){
			jump_force = 0.0f;
			Position.y = ground_y; 
			if ( isground == false)
			{
				set_motion(AnimationType.JUMP_END);
			}
			isground = true;
		}else{
			if ( isground == true)
			{
//				set_motion( AnimationType.JUMP_AIR);
			}
			//空中では移動料がかかり続ける
			Position.x += jump_speed;

			isground = false;
		}

		//motion set
		if (is_wait () == false) {
			if (isground == true) {
				if (ismove == true) {
					if (dash == true) {
						set_motion(AnimationType.RUN);
					} else {
						set_motion(AnimationType.WALK);
					}
				} else {
					if (motion != AnimationType.JUMP_END) {
						set_motion(AnimationType.STANCE);
					}
				}
			}
		}

		//向きを反映
		if (direction == 0) {
			transform.localScale = new Vector3(init_scale, init_scale, 1.0f);							//左向き制御スケール
		} else {
			transform.localScale = new Vector3(-init_scale, init_scale, 1.0f);						//左向き制御スケール
		}

		// 現在の位置を設定
		player_pos = Position;
		//コリジョンの設定
		GameControl.set_collision( gamemain.COLTYPE.EN_COLTYPE_PLAYER, transform.gameObject, player_pos.x, player_pos.y, 100.0f, 100.0f, 0, 0 );

		//カメラの位置を設定
		gamecamera.x = Position.x;
		float scale = 0.5f;
		gamecamera.y = ( Position.y * scale ) + ( -ground_y * scale );

		Position.x = 0;
		transform.position = Position;


		move_bg( );

	}

	//モーションを設定する
	void set_motion( AnimationType now_motion, bool flg = false)
	{
		if ( (motion != now_motion) || ( flg == true ) )
		{
			//anime kousin
			motion = now_motion;
			spriteStudioRoot.AnimationPlay((int)now_motion, 1, 0, 1.0f);	
		}
	}

	//硬直があるかを取得
	bool is_wait( )
	{
		bool rc = false;
		switch (motion) {
		case AnimationType.ATTACK1:
		case AnimationType.ATTACK2:
		case AnimationType.ATTACK3:
		case AnimationType.CHARGE:
		case AnimationType.DAMEGE:
		case AnimationType.DEAD1:
		case AnimationType.DEAD2:
		case AnimationType.DEFENSE:
		case AnimationType.KICK1:
		case AnimationType.KICK2:
		case AnimationType.NO_ANIME:
		case AnimationType.PANCH1:
		case AnimationType.PANCH2:
		case AnimationType.PIYO:
		case AnimationType.POSE1:
		case AnimationType.POSE2:
		case AnimationType.JUMP_ATTACK1:
		case AnimationType.JUMP_ATTACK2:
			rc = true;
			break;
		case AnimationType.JUMP_AIR:
		case AnimationType.JUMP_ALL:
		case AnimationType.JUMP_END:
		case AnimationType.JUMP_START:
		case AnimationType.RUN:
		case AnimationType.STANCE:
		case AnimationType.WAIT:
		case AnimationType.WALK:
			rc = false;
			break;
		}
		return rc;
	}


	//アニメーションの終了コールバック
	public bool AnimEnd(GameObject ObjectControl)
	{
		anime_end = true;
		return true;
	}

	public bool set_next_motion()
	{
		//終了後のモーションを指定する
		//最終状態で停止する場合は指定なし
		switch (motion) {
		case AnimationType.ATTACK1:
			ren_attack_count = 0;										//連続攻撃の回数
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.ATTACK2:
			ren_attack_count = 0;										//連続攻撃の回数
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.ATTACK3:
			ren_attack_count = 0;										//連続攻撃の回数
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.CHARGE:
			set_motion( AnimationType.CHARGE, true);
			break;
		case AnimationType.DAMEGE:
			break;
		case AnimationType.DEAD1:
			break;
		case AnimationType.DEAD2:
			break;
		case AnimationType.DEFENSE:
			break;
		case AnimationType.JUMP_AIR:
			set_motion( AnimationType.JUMP_AIR, true);
			break;
		case AnimationType.JUMP_ALL:
			break;
		case AnimationType.JUMP_ATTACK1:
			ren_attack_count = 0;										//連続攻撃の回数
			set_motion( AnimationType.JUMP_AIR, true);
			break;
		case AnimationType.JUMP_ATTACK2:
			ren_attack_count = 0;										//連続攻撃の回数
			set_motion( AnimationType.JUMP_AIR, true);
			break;
		case AnimationType.JUMP_END:
			set_motion( AnimationType.STANCE, true);
			//			set_motion( AnimationType.JUMP_END, true);
			break;
		case AnimationType.JUMP_START:
			set_motion( AnimationType.JUMP_AIR, true);
			//			set_motion( AnimationType.JUMP_START, true);
			break;
		case AnimationType.KICK1:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.KICK2:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.NO_ANIME:
			break;
		case AnimationType.PANCH1:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.PANCH2:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.PIYO:
			set_motion( AnimationType.PIYO, true);
			break;
		case AnimationType.POSE1:
			break;
		case AnimationType.POSE2:
			break;
		case AnimationType.RUN:
			set_motion( AnimationType.RUN, true);
			break;
		case AnimationType.STANCE:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.WAIT:
			set_motion( AnimationType.WAIT, true);
			break;
		case AnimationType.WALK:
			set_motion( AnimationType.WALK, true);
			break;
		}
		return true;
	}

	//BG制御
	void move_bg( )
	{
		float scale_x = 1.0f;
		float scale_y = 1.0f;
		Vector3 pos_fornt = new Vector3(background_front_initpos.x - ( gamecamera.x * scale_x ), background_front_initpos.y - ( gamecamera.y * scale_y ), background_front_initpos.z);
		background_front.transform.position = pos_fornt;

		scale_x = 0.3f;
		scale_y = 1.0f;
		Vector3 pos_center = new Vector3(background_center_initpos.x - ( gamecamera.x * scale_x ), background_center_initpos.y - ( gamecamera.y * scale_y ), background_center_initpos.z);
		background_center.transform.position = pos_center;
	
		scale_x = 0.2f;
		scale_y = 0.2f;
		Vector3 pos_back = new Vector3(background_back_initpos.x - ( gamecamera.x * scale_x ), background_back_initpos.y - ( gamecamera.y * scale_y ), background_back_initpos.z);
		background_back.transform.position = pos_back;
	}



}
