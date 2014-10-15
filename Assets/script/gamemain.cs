using UnityEngine;
using System.Collections;

public class gamemain : MonoBehaviour {

	private int gametime = 0;
	private camera2d Camera2DControl;
	private const bool collsion_disp = false;	//コリジョン表示

	public enum COLTYPE{
		EN_COLTYPE_NONE = 0,
		EN_COLTYPE_PLAYER,
		EN_COLTYPE_PLAYER_SHOT,
		EN_COLTYPE_ENEMY,
		EN_COLTYPE_ENEMY_SHOT,
		EN_COLTYPE_ITEM,
	};

	public struct COLLISION {
		public int life;		//存在の有無
		public COLTYPE coltype;	//コリジョンタイプ
		public GameObject obj;	//関連してるキャラクター
		public float x;			//位置   キャラクターの中央からのオフセット
		public float y;			//位置   キャラクターの中央からのオフセット
		public float w;			//サイズ
		public float h;			//サイズ
		public int hitparam;	//判定の種類
		public int damege;		//相手へのダメージ

		public void Clear( )
		{
			life = 0;		//存在の有無
			coltype = COLTYPE.EN_COLTYPE_NONE;		//コリジョンタイプ
			obj = null;	//関連してるキャラクター
			x = 0.0f;			//位置   キャラクターの中央からのオフセット
			y = 0.0f;			//位置   キャラクターの中央からのオフセット
			w = 0.0f;		//サイズ
			h = 0.0f;		//サイズ
			hitparam = 0;	//判定の種類
			damege = 0;		//相手へのダメージ
		}
	};

	const int COLLISION_MAX = 10;
	COLLISION[] collision = new COLLISION[10];
	GameObject[] coldisp = new GameObject[10];

	// Use this for initialization
	void Start () {

		//ゲームコントロールスクリプトの取得
		var go = GameObject.Find("GameControl");
		Camera2DControl = go.GetComponent<camera2d>();

		KeyInit( );	//キー入力初期化

		int i;
		for ( i = 0; i < COLLISION_MAX; i++ )
		{
			coldisp[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			coldisp[i].transform.position = new Vector3(-2000.0f, -2000.0f, 0.0f);
			coldisp[i].transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		KeyInput( );
		move_collision( );
		move_delete_collision( );

		gametime++;		//ゲーム時間
	}


	public void set_collision( COLTYPE coltype, GameObject obj, float x, float y, float w, float h, int hitparam, int damege )
	{
		int i;
		
		for ( i = 0; i < COLLISION_MAX; i++ )
		{
			if ( collision[i].life <= 0 )
			{
				collision[i].Clear();
				collision[i].life = 1;				//存在の有無
				collision[i].coltype = coltype;		//種類
				collision[i].obj = obj;				//オブジェクト
				collision[i].x = x;				//位置   キャラクターの中央からのオフセット
				collision[i].y = y;				//位置   キャラクターの中央からのオフセット
				collision[i].w = w;			//サイズ
				collision[i].h = h;			//サイズ
				collision[i].hitparam = hitparam;	//判定の種類
				collision[i].damege = damege;		//相手へのダメージ
				break;
			}

		}
	}
	void move_collision( )
	{
		int i;
		
		for ( i = 0; i < COLLISION_MAX; i++ )
		{
			coldisp[i].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);	//とりあえず表示を画面外に行う

			if ( collision[i].life > 0 )
			{
				Vector2 camerapos = Camera2DControl.GetCamera( );
				//範囲表示用プリミティブを作成
				switch( collision[i].coltype )
				{
				case COLTYPE.EN_COLTYPE_PLAYER:
					coldisp[i].renderer.material.color = Color.blue;
					break;
				case COLTYPE.EN_COLTYPE_PLAYER_SHOT:
					coldisp[i].renderer.material.color = Color.yellow;
					break;
				case COLTYPE.EN_COLTYPE_ENEMY:
					coldisp[i].renderer.material.color = Color.red;
					break;
				case COLTYPE.EN_COLTYPE_ENEMY_SHOT:
					coldisp[i].renderer.material.color = Color.green;
					break;
				}
				if ( collsion_disp == true)
				{
					coldisp[i].transform.position = new Vector3(collision[i].x - camerapos.x, collision[i].y - camerapos.y, 0.0f);
					coldisp[i].transform.localScale = new Vector3(collision[i].w, collision[i].h, 1.0f);	//コリジョンを表示しない場合はこの行をコメントにする
				}


				float ch1_x1 = 0;
				float ch1_x2 = 0;
				float ch1_y1 = 0;
				float ch1_y2 = 0;
				
				float ch2_x1 = 0;
				float ch2_x2 = 0;
				float ch2_y1 = 0;
				float ch2_y2 = 0;
				//プレイヤーの攻撃と敵
				if ( collision[i].coltype == COLTYPE.EN_COLTYPE_PLAYER_SHOT )
				{
					int j = 0;
					for ( j = 0; j < COLLISION_MAX; j++ )
					{
						
						if ( ( collision[j].coltype == COLTYPE.EN_COLTYPE_ENEMY ) && ( collision[j].life > 0 ) )
						{
							ch1_x1 = collision[i].x - ( collision[i].w / 2.0f );
							ch1_x2 = collision[i].x + ( collision[i].w / 2.0f );
							ch1_y1 = collision[i].y - ( collision[i].h / 2.0f );
							ch1_y2 = collision[i].y + ( collision[i].h / 2.0f );
							
							ch2_x1 = collision[j].x - ( collision[j].w / 2.0f );
							ch2_x2 = collision[j].x + ( collision[j].w / 2.0f );
							ch2_y1 = collision[j].y - ( collision[j].h / 2.0f );
							ch2_y2 = collision[j].y + ( collision[j].h / 2.0f );

							if ( 
							    ( ch1_x1 < ch2_x2 )
							    && ( ch2_x1 < ch1_x2 )
							    && ( ch1_y1 < ch2_y2 )
							    && ( ch2_y1 < ch1_y2 )
							    )
							{
								//ヒットコールバック
								player playerclass = collision[i].obj.GetComponent<player>();
								enemy enemyclass = collision[j].obj.GetComponent<enemy>();

								playerclass.collision_callback( collision[i], collision[j] );
								enemyclass.collision_callback( collision[j], collision[i] );
							}
						}
					}
				}
				//敵の攻撃とプレイヤー
				if ( collision[i].coltype == COLTYPE.EN_COLTYPE_ENEMY_SHOT )
				{
					int j = 0;
					for ( j = 0; j < COLLISION_MAX; j++ )
					{
						
						if ( ( collision[j].coltype == COLTYPE.EN_COLTYPE_PLAYER ) && ( collision[j].life > 0 ) )
						{
							ch1_x1 = collision[i].x - ( collision[i].w / 2.0f );
							ch1_x2 = collision[i].x + ( collision[i].w / 2.0f );
							ch1_y1 = collision[i].y - ( collision[i].h / 2.0f );
							ch1_y2 = collision[i].y + ( collision[i].h / 2.0f );
							
							ch2_x1 = collision[j].x - ( collision[j].w / 2.0f );
							ch2_x2 = collision[j].x + ( collision[j].w / 2.0f );
							ch2_y1 = collision[j].y - ( collision[j].h / 2.0f );
							ch2_y2 = collision[j].y + ( collision[j].h / 2.0f );

							if ( 
							    ( ch1_x1 < ch2_x2 )
							    && ( ch2_x1 < ch1_x2 )
							    && ( ch1_y1 < ch2_y2 )
							    && ( ch2_y1 < ch1_y2 )
							    )
							{
								//ヒットコールバック
								player playerclass = collision[j].obj.GetComponent<player>();
								enemy enemyclass = collision[i].obj.GetComponent<enemy>();

								playerclass.collision_callback( collision[j], collision[i] );
								enemyclass.collision_callback( collision[i], collision[j] );
							}
						}
					}
				}
				//プレイヤーと敵
				if ( collision[i].coltype == COLTYPE.EN_COLTYPE_PLAYER )
				{
					int j = 0;
					for ( j = 0; j < COLLISION_MAX; j++ )
					{
						
						if ( ( collision[j].coltype == COLTYPE.EN_COLTYPE_ENEMY ) && ( collision[j].life > 0 ) )
						{
							ch1_x1 = collision[i].x - ( collision[i].w / 2.0f );
							ch1_x2 = collision[i].x + ( collision[i].w / 2.0f );
							ch1_y1 = collision[i].y - ( collision[i].h / 2.0f );
							ch1_y2 = collision[i].y + ( collision[i].h / 2.0f );
							
							ch2_x1 = collision[j].x - ( collision[j].w / 2.0f );
							ch2_x2 = collision[j].x + ( collision[j].w / 2.0f );
							ch2_y1 = collision[j].y - ( collision[j].h / 2.0f );
							ch2_y2 = collision[j].y + ( collision[j].h / 2.0f );

							if ( 
							    ( ch1_x1 < ch2_x2 )
							    && ( ch2_x1 < ch1_x2 )
							    && ( ch1_y1 < ch2_y2 )
							    && ( ch2_y1 < ch1_y2 )
							    )
							{
								//ヒットコールバック
								player playerclass = collision[i].obj.GetComponent<player>();
								playerclass.collision_callback( collision[i], collision[j] );

								enemy enemyclass = collision[j].obj.GetComponent<enemy>();
								enemyclass.collision_callback( collision[j], collision[i] );
							}
						}
					}
				}
			}
		}
	}
	
	void move_delete_collision( )
	{
		int i;
		
		for ( i = 0; i < COLLISION_MAX; i++ )
		{
			//コリジョン消去
			collision[i].life = 0;
		}
	}

	//key input
	public enum INPUTBUTTON{
		BUTTON_UP,
		BUTTON_DOWN,
		BUTTON_LEFT,
		BUTTON_RIGHT,
		BUTTON_1,
		BUTTON_2,
		BUTTON_MAX
	}
	private int[] key_eventPress = new int[6];
	private int[] key_eventPress_old = new int[6];
	private int[] key_eventPush = new int[6];
	
	enum INPUTPAD{
		PAD_UP = 0x0001,
		PAD_DOWN = 0x0002,
		PAD_LEFT = 0x0004,
		PAD_RIGHT = 0x0008,
		PAD_BUTTON1 = 0x0010,
		PAD_BUTTON2 = 0x0020,
	};
	void KeyInit( )
	{
		int i;
		
		for ( i = 0; i < (int)INPUTBUTTON.BUTTON_MAX; i++ )
		{
			key_eventPress[i] = 0;
			key_eventPress_old[i] = 0;
			key_eventPush[i] = 0;
		}
	}
	void KeyInput( )
	{
		int i;
		int pad = 0;
		
		if (Input.GetKey ("right")) {
			pad |= (int)INPUTPAD.PAD_RIGHT;
		}
		if (Input.GetKey ("left")) {
			pad |= (int)INPUTPAD.PAD_LEFT;
		}
		if (Input.GetKey ("down")) {
			pad |= (int)INPUTPAD.PAD_DOWN;
		}
		if (Input.GetKey ("up")) {
			pad |= (int)INPUTPAD.PAD_UP;
		}
		
		//		if (Input.GetKey ("Joystick1Button0") || Input.GetKey ("Z")) {
		if (Input.GetKey ("z")) {
			pad |= (int)INPUTPAD.PAD_BUTTON1;
		}
		//		if (Input.GetKey ("Joystick1Button1") || Input.GetKey ("X")) {
		if (Input.GetKey ("x")) {
			pad |= (int)INPUTPAD.PAD_BUTTON2;
		}
		
		for ( i = 0; i < (int)INPUTBUTTON.BUTTON_MAX; i++ )
		{
			key_eventPress[i] = 0;
		}
		
		if ( ( pad & (int)INPUTPAD.PAD_UP ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_UP] = 1;
		}
		if ( ( pad & (int)INPUTPAD.PAD_DOWN ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_DOWN] = 1;
		}
		if ( ( pad & (int)INPUTPAD.PAD_LEFT ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_LEFT] = 1;
		}
		if ( ( pad & (int)INPUTPAD.PAD_RIGHT ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_RIGHT] = 1;
		}
		if ( ( pad & (int)INPUTPAD.PAD_BUTTON1 ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_1] = 1;
		}
		if ( ( pad & (int)INPUTPAD.PAD_BUTTON2 ) != 0 )
		{
			key_eventPress[(int)INPUTBUTTON.BUTTON_2] = 1;
		}
		for ( i = 0; i < (int)INPUTBUTTON.BUTTON_MAX; i++ )
		{
			if ( ( key_eventPress_old[i] == 0 ) && ( key_eventPress[i] != 0 ) )
			{
				key_eventPush[i] = 1;
			}
			else 
			{
				key_eventPush[i] = 0;
			}
			key_eventPress_old[i] = key_eventPress[i];
			
		}
	}
	//押されたフレームかを取得
	public bool IsPushKey( int keycode )
	{
		if ( key_eventPush[keycode] == 1 )
		{
			return( true );
		}
		return( false );
	}
	//押されているかを取得
	public bool IsPressKey( int keycode )
	{
		if ( key_eventPress[keycode] == 1 )
		{
			return( true );
		}
		return( false );
	}

}
