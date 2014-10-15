using UnityEngine;
using UnityEditor;
using System.Collections;

//プレイヤークラス
public class player : MonoBehaviour {

	//スプライトスタジオアニメを操作するためのクラス
	//Unity側で関連をつけておく
	private Script_SpriteStudio_PartsRoot spriteStudioRoot;

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

	//キャラクター制御
	// 速度
	private Vector2 SPEED = new Vector2(10.0f, 10.0f);			//移動速度
	private AnimationType motion = AnimationType.STANCE;		//表示モーション
	private bool isground = false;								//地面に着地しているか？
	private float ground_y = -500.0f;							//地面の位置
	private float gravitiy = 98.0f;								//重力加速度
	private float jump_force = 0;								//ジャンプ力
	private float jump_speed = 0;								//ジャンプ中の移動速度
	public Vector2 player_pos = new Vector2(0.0f, -500.0f);	//プレイヤーの位置（表示位置とは別）
	private float init_scale = 0.7f;							//プレイヤーの初期スケール
	public int direction = 0;									//向き 0:左　1:右
	private bool anime_end = false;								//アニメーションの終了フラグ
	private int attack_wait_init = 20;							//攻撃の硬直時間設定値
	private int attack_wait = 0;								//攻撃の硬直時間
	private int ren_attack_count = 0;							//連続攻撃の回数
	private int knockback = 0;									//ノックバック
	private int attack_time = 0;								//攻撃開始からの時間
	private int hit_muteki = 0;									//連続ヒットしないようにヒットフラグ
	private int timer = 0;										//生存時間

	//コマンド入力
	private bool dash = false;				//ダッシュ中か？
	private int dash_command_count = 0;		//前回のキー入力からの時間
	private int dash_command_time = 25;		//コマンド成立時間
	private int dash_oldinput_key = 0;		//前回入力されたキー

	//ゲームコントロール
	private gamemain GameControl;
	private camera2d Camera2DControl;
	private effect_control effectcontrol;

	// Use this for initialization
	void Start () 
	{
		//ルートパーツの取得
		spriteStudioRoot = GetComponentInChildren<Script_SpriteStudio_PartsRoot>();

		//ゲームコントロールスクリプトの取得
		var go = GameObject.Find("GameControl");
		GameControl = go.GetComponent<gamemain>();
		Camera2DControl = go.GetComponent<camera2d>();
		effectcontrol = go.GetComponent<effect_control>();

		//アニメーションの終了割り込みを設定
		spriteStudioRoot.FunctionPlayEnd = AnimEnd;
		//初期アニメーションを設定
		set_motion( AnimationType.STANCE, true);

		direction = 0;									//左向き


	}

	// Update is called once per frame
	void Update () 
	{
		// for test code 
		//ボタンを押したら攻撃
		if (GameControl.IsPushKey((int)gamemain.INPUTBUTTON.BUTTON_1)) {
//			set_motion(AnimationType.JUMP_END);

//			set_motion(AnimationType.JUMP_AIR, true);
			//エフェクトの生成（パーティクルシステム）
//			GameObject hit = Object.Instantiate(hit_effect) as GameObject;
//			hit.transform.position = new Vector3(0.0f, 0.0f, -30.0f);
/*
			//エフェクトの生成
			var go = Instantiate(Resources.Load("Prefabs/effect2/effect001_Control"),new Vector3(0,0,0),Quaternion.identity) as GameObject;
			//生成したアニメデータは Camera2D Pixel (1080p) の　View　の下に配置しないと表示できない。
			//このアニメの親（View）を親に設定する必要がある
			go.transform.parent = transform.parent;
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
				attack_time = 0;

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

		//ノックバック移動
		if ( knockback > 0 )
		{
			float move_x = (float)( knockback * knockback ) * 0.08f;
			if (direction == 0) {
				//左向き
				Position.x += move_x;
			}
			else{
				//右向き
				Position.x -= move_x;
			}
			set_motion(AnimationType.DAMEGE);
		}
		knockback--;
		if ( knockback == 0 )
		{
			//ノックバック終了
			set_motion(AnimationType.STANCE);
		}
		if ( knockback < 0 )
		{
			knockback = 0;
			hit_muteki = 0;
		}

		//移動範囲を設定
		if (Position.x < -3000.0f) {
			Position.x = -3000.0f;
		}
		if (Position.x > 3000.0f) {
			Position.x = 3000.0f;
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
		{
			//体
			float h = 200.0f;
			float w = 400.0f;
			float xofs = 0.0f;
			float yofs = 250.0f;
			GameControl.set_collision( gamemain.COLTYPE.EN_COLTYPE_PLAYER, transform.gameObject, player_pos.x + xofs, player_pos.y + yofs, h, w, 0, 0 );

			//攻撃
			xofs = -300.0f;
			if ( direction == 1 )
			{
				xofs = 300.0f;
			}
			switch (motion) {
			case AnimationType.ATTACK1:
			case AnimationType.ATTACK2:
			case AnimationType.ATTACK3:
			case AnimationType.KICK1:
			case AnimationType.KICK2:
			case AnimationType.JUMP_ATTACK1:
			case AnimationType.JUMP_ATTACK2:
				attack_time++;
				if ( attack_time == 20 )
				{
					h = 300.0f;
					w = 300.0f;
					yofs = 250.0f;
					GameControl.set_collision( gamemain.COLTYPE.EN_COLTYPE_PLAYER_SHOT, transform.gameObject, player_pos.x + xofs, player_pos.y + yofs, h, w, 0, 0 );
				}
				break;
			default:
				break;
			}

		}
		//カメラの位置を設定
		{
			Vector2 camerapos = new Vector2( Position.x, Position.y );
			Camera2DControl.SetCamera( camerapos );
		}
		//カメラの位置から表示位置を設定
		{
			Vector2 camerapos = Camera2DControl.GetCamera( );
			transform.position = Position - camerapos;
		}

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
		bool rc = true;
		switch (motion) {
		case AnimationType.JUMP_AIR:
		case AnimationType.JUMP_ALL:
		case AnimationType.JUMP_END:
		case AnimationType.JUMP_START:
		case AnimationType.RUN:
		case AnimationType.STANCE:
		case AnimationType.WAIT:
		case AnimationType.WALK:
		case AnimationType.SIT:
			rc = false;
			break;
		default:
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
			break;
		case AnimationType.JUMP_KICK:
			set_motion( AnimationType.JUMP_AIR, true);
			break;
		case AnimationType.JUMP_PUNCH:
			set_motion( AnimationType.JUMP_AIR, true);
			break;
		case AnimationType.JUMP_START:
			set_motion( AnimationType.JUMP_AIR, true);
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
		case AnimationType.SIT:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.SIT_ATTACK1:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.SIT_ATTACK2:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.SIT_KICK:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.SIT_PANCH:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.SITDOWN:
			set_motion( AnimationType.SIT, true);
			break;
		case AnimationType.STANCE:
			set_motion( AnimationType.STANCE, true);
			break;
		case AnimationType.STANDUP:
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

	//連続ヒットを行う場合に無敵を解除する
	public void Set_hit_muteki_clear( )
	{
		hit_muteki = 0;
	}
	public void Set_knockback( int time, int dir )
	{
		if ( knockback < time )
		{
			knockback = time;									//ノックバック
		}
		if ( dir == 0 )
		{
			direction = 1;								//向き
		}
		else
		{
			direction = 0;								//向き
		}
	}

	//コリジョンコールバック
	public void collision_callback( gamemain.COLLISION my, gamemain.COLLISION you )
	{
		switch( you.coltype )
		{
		case gamemain.COLTYPE.EN_COLTYPE_PLAYER:
			//相手がプレイヤー
			break;
		case gamemain.COLTYPE.EN_COLTYPE_PLAYER_SHOT:
			//相手がプレイヤー攻撃
			break;
		case gamemain.COLTYPE.EN_COLTYPE_ENEMY:
			if (my.coltype == gamemain.COLTYPE.EN_COLTYPE_PLAYER)
			{
				//相手が敵
				//重ならない様に位置を補正
				//敵側の位置もここで補正するので敵側に補正処理はいらない
				player playerclass = my.obj.GetComponent<player>();
				enemy enemyclass = you.obj.GetComponent<enemy>();

				Vector2 mypos = playerclass.player_pos; 
				Vector2 youpos = enemyclass.player_pos; 

				if ( mypos.x < youpos.x )
				{
					//自分の右側から相手の左側を引くと移動量がでる
					float move_x = ( mypos.x + ( my.w / 2.0f ) ) - ( youpos.x - ( you.w / 2.0f ) );
					move_x = move_x / 2.0f;

					mypos.x = mypos.x - move_x; 
					playerclass.player_pos = mypos;

					youpos.x = youpos.x + move_x; 
					enemyclass.player_pos = youpos;
				}
				else
				{
					//自分の右側から相手の左側を引くと移動量がでる
					float move_x = ( youpos.x + ( you.w / 2.0f ) ) - ( mypos.x - ( my.w / 2.0f ) );
					move_x = move_x / 2.0f;
					
					mypos.x = mypos.x + move_x; 
					playerclass.player_pos = mypos;

					youpos.x = youpos.x - move_x; 
					enemyclass.player_pos = youpos;
				}

			}
			if (my.coltype == gamemain.COLTYPE.EN_COLTYPE_PLAYER_SHOT)
			{
			}
			break;
		case gamemain.COLTYPE.EN_COLTYPE_ENEMY_SHOT:
			//相手が敵の攻撃
			if (my.coltype == gamemain.COLTYPE.EN_COLTYPE_PLAYER)
			{
				//相手がプレイヤー攻撃
				player playerclass = my.obj.GetComponent<player>();
				enemy enemyclass = you.obj.GetComponent<enemy>();
				
				if (hit_muteki == 0 )
				{
					playerclass.Set_knockback( 30, enemyclass.direction );
					effectcontrol.CreateEffect(0, you.x, you.y);
				}
			}
			break;
		case gamemain.COLTYPE.EN_COLTYPE_ITEM:
			//相手がアイテム
			break;
		}


	}

}
