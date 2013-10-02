==============================================================================

#  SpriteStudio5 Player for Unity

  Copyright(C) 2013 Web Technology Corp. All rights reserved.

==============================================================================

## はじめに

本ソフトウェアはベータ版であり、動作検証、評価を目的として配布しているため、何
らかの不具合が含まれている可能性があることをご了承いただいた上で、ご利用いただ
きますようお願い致します。

本ソフトウェアは OPTPiX SpriteStudio 5 から出力された ssae ファイルを Unity 上で
再生するためのプレイヤープログラムのパッケージです。

## データのインポート手順

### プロジェクト(.sspj) をインポートする

本パッケージではアニメーションデータをプロジェクト単位でインポートします。

まず、UnityのタイトルメニューのCustom→SpriteStudio→Import SS5(.sspj)を選択します。

※上記メニューが存在しない場合は Unityを再起動してみてください。  
それでも変わらない場合はUnityの新規プロジェクトを作成し本パッケージを再インポートしてお試しください。

OPTPiX SpriteStudio Import-Settingダイアログが表示されますので必要に応じて下記の設定を行います。

#### インポート設定を指定する

各設定項目の意味は下記の通りです

	Collider-Thickness	
		SpriteStudioで設定した当たり判定をColliderとして変換を行う際にZ軸方向の厚みを設定します。

	Attach Rigid-Body
		当たり判定をColliderへ変換する際にRigid-Bodyをコンポーネントとして追加するかを設定します。

#### 保存先フォルダを指定する

設定が完了したらインポートするデータの保存先を予め選択しておきます。  
保存先はProject ビュー上で__現在選択しているフォルダ__の中になります。

Unity4 の場合は、__右ペイン上で選択しているフォルダ__が対象になります。  
左ペインで選択されているフォルダではありませんのでご注意ください。

#### インポートを開始する

「Import」ボタンを押して、SS5プロジェクトファイル(.sspj)を選択するとインポートが開始されます。

プロジェクトに登録された各アニメーションがUnityアセットに変換されて指定フォルダ内に保存されます。  

__※保存されていない場合はConsoleを開いてエラー等が出ていないかご確認ください。__

インポートが完了すると指定フォルダ内に下記のデータが作成されます。

- Textureフォルダ  
	使用しているテクスチャ（セルマップ）が格納されています。

- Materialフォルダ  
	使用しているテクスチャと、SSのアニメーションデータを表示するための
	シェーダが割り当てられたMaterialが格納されています。

- アニメーションPrefab  
	sspj内に定義されている各ssaeファイル毎に、Prefab化されて格納されます。
	1つのssaeファイルが1つのPrefabになり、ssaeファイル内に複数のアニメーション
	が格納されていた場合、対応するPrefab内に複数のアニメーション再生用データが
	格納されます。

また、本アセットがインポートできるパーツの当たり判定の種類は下記になります。

- なし
- 四角形
- 円形（スケール影響なし）

これ以外の当たり判定種別は「なし」と同等に扱われますのでご注意下さい。  
※Ver.0.91 時点では未対応になります。

※「四角形」「円形」は、Unity上ではtransformの影響を受けますので、transformで
　回転・スケールを変更すると、それらの値に影響を受けますので、ご注意下さい。

## 再生手順

### シーン(Hierarchy)へ登録する

#### カメラの配置

Project - SpriteStudio - CameraのCamera2D pixel (720p or 1080p)をHierarchy 上にドロップし
てシーンに登録します。  
シーンにデフォルトで登録されている Main Camera は通常、本パッケージでは使いませんので、他オブジェクトの表示に使わない場合は削除して頂いて構いません。

#### View とは

Camera2D の子にあるView は通常の2D表示を行う場合に、「表示範囲」を定めるためのものです。
Cameraは前述のものでなくともOrthoカメラであれば問題ありませんが、View の設定も適切に行ってください。  

このViewは表示領域をスクロールしたりする場合の2D表示領域の特定に使用されます。  
このViewにスクリプトを設定することで、様々な2D上での表示操作を行うことができます。

#### Viewの原点の変更方法

デフォルトで表示範囲の中央が0, 0 になるよう、View のPositionは0, 0に配置されています。
1080pのカメラで左下を原点にする場合は Position のX,Yを -960, -540 に変更してください。

#### アニメーションの登録と再生

シーンに登録したCamera2Dの子にあるViewの下に、インポートしたアニメーションPrefabをドロップします。  

再生ボタンを押してアニメーションが再生されることを確認してください。

#### 何も表示されない場合

何も表示されない場合は、Viewの表示領域外にドロップされている可能性があります。
ドロップしたオブジェクトのPosition をX=0, Y=0, Z=0 に移動してみて下さい。  

また、Sceneビューで表示されても、Gameビューでは表示されない場合、
シーンに存在する別のカメラと干渉している可能性があります。
もしデフォルトの Main Camera が存在している場合、不要なら削除するか、適切に設定を変更してください。

## アニメーションの設定

## Inspector の情報

アニメーションPrefab の子オブジェクト(ルートパーツに相当)にはScript_SpriteStudio_PartsRootというスクリプトが付加されます。
このスクリプトによって下記の設定項目を参照・変更することができます。

#### Based-Material Table

アニメーションの表示に必要なマテリアルへの参照配列を示します。  
マテリアルはアニメーション内の全パーツが参照するセルマップの参照先イメージの数ｘ４シェーダ分だけ存在しています。

SpriteStudio 5 が持つ４種類のカラーブレンド方法を再現するために１つのイメージにつき常に４つのマテリアルが生成されます。

各マテリアルには下記４つのシェーダが割り当てられます。

- Mix（ミックス）
- Add（加算）
- Sub（減算）
- Mul（乗算）

※Unity 上で参照先のイメージを別のイメージへ置き換える場合は、上記4種のマテリアルを一度に入れ替えるようにします。

#### Animation Infomation

１つのssaeファイルに含まれる全アニメーションが列挙されています。

各項目の意味は下記の通りです。

- Start Frame-No    [開始フレーム]
- End Frame-No      [終了フレーム]
- Base FPS          [単位時間]:(１フレーム当たりの表示秒数)

各アニメーションは１つのタイムライン上に連結されて保存されます。  
そのため、アニメーション毎にタイムライン上のどの区間に割り当てられているかを示す「開始・終了フレーム」が存在します。

#### Initial/Preview Play Setting

- Animation Name

	再生するアニメーション名を指定します。  
	アニメーションが複数含まれる場合は、ここで別のアニメーションを選択し
	切り替えることができます。

- Start Offset Frame-No

	アニメーションの開始フレームを設定します。  
	複数のアニメーションがある場合、選択されているアニメーションの先頭フレー
	ムを0とした場合の値になります。

- Rate Time-Progress

	再生スピードを倍率で指定します。
	値は「アニメーションの経過時間に対する係数」であるため、  
	1.0fで等速・2.0fで倍速・0.5fで半分速になります。  
	この値に__負数を設定しても逆再生は行えない__ので注意して下さい。  
	※逆再生は今後のバージョンアップで補間の事前計算対応後に可能になる予定です。

- Loop Count  
	ループ回数の指定を行います。
	0でループしない（1回のみの再生）・-1で無限回数ループになります。

- Reset(Reinitialize)ボタン  
	インポート直後の設定に戻します。

#### Rendering Setting

Unity上でレンダリングが行われる際に、複数マテリアルの描画順序によって想定した画像が出ないことを調整するためのパラメータ群です。  
非常に特殊な設定になりますので、本項目の内容とUnityの仕様をご理解頂いた上で使用するようにして下さい。

- [Render-Queue Base]  

	描画順序の最も大きな区分での順序を指定するものです。  
	シェーダに記載される"Queue"タグでの指定と同じものとなります。  
	"Queue"タグにないパラメータとしては
	- Shader Setting : デフォルトの値で、使用するシェーダのTag"Queue"で義される値を使用します。  (本パッケージに内包される全シェーダは「Transparent」に
設定されています)

	- User Setting : 非常に特殊な設定で、次の[Render-Queue Offset]の値を「絶対値」として、描画順序指定とします。  
	※余程のことがない限り、本設定は「Shader Setting」のままで構いません。

- [Render-Queue Offset]

	[Render-Queue Base]に対するオフセット値で、本値が大きい程後から描画
	されます。  
	主に、「同じZ値を持つスプライト同士が重なってチラつく場合」などの
	調整用途で使用して下さい。  
	シェーダで"Queue"タグを"～ + x"と書いた場合と同様の効果となります。
	本値には負数を設定しないで下さい。

- [Rate Z Effect]

	スプライトのtransformのPosition.zを参考に、描画するカメラのClipPlane
	の「Near」と「Far」の区間を「1.0f～0.0f」とし・スプライトがこの区間の
	中に入っている時のCameraからの距離（最遠:0.0f～最近:1.0f）とした値に、
	本値を乗算したものを描画順序の考慮に入れます。  
	本値に0を指定すると、各アニメーション間でのZ値の違いは描画順序には
	考慮されません。  
	本値には負数を設定しないで下さい。

	具体的な描画順序値は
	[Render-Queue Base] + [Render-Queue Offset] + (nZ * [Rate Z Effect])
	になります（「nZ」は1.0f～0.0fの間に変換されたNear PlaneからのZ距離）。
	さらに、この値に各アニメーション内での優先度などから割り出された描画
	順序が足されることになります。  
	そしてこの値が大きい程、後から描画が行われることになります。

-	__注意__:  
	最終的な描画順序値が、[Render-Queue Base]で指定されている値の範囲を
	超えないような設定にするようにして下さい（範囲外の値になった場合の
	正常動作については保証いたしません）。  
	（[Render-Queue Base]の選択を切り替えると、そのキューの持つ値範囲が
	　その下に表示されるようになっています）。  
	これらの値の詳細については、UnityのReference Manualの
	「ShaderLab syntax: SubShader Tags」内の「Queue」タグの解説に記載が
	ありますので、そちらを参照して下さい。

## アニメーションの操作

インポートしたアニメーションをスクリプトから制御するための専用のゲームパーツ
（以降これを「コントロール用パーツ」と呼称します）が、rootパーツの親として付加
されています。  
原則として、スプライトの動きなどの制御は、このコントロール用パーツにスクリプト
を付加して行う前提です。

一方、rootパーツに付加されているスクリプトは、インポートしたアニメーションの
各種設定を参照・変更したり、初期状態を設定するためにあります。  
コントロール用パーツからアニメーションの再生状態を制御する場合、本rootパーツ
のスクリプト内にある関数を使用して行います。

アニメーションの操作は、各アニメーションに紐づいている
Script_SpriteStudio_PartsRootを通して行います。  
詳しい引数等はScript_SpriteStudio_PartsRoot.csへ記載しておりますのでそちらを
ご参照ください。

ここでは、現在publicとして公開している関数のみ列記します。  
下記関数以外についても、publicとして定義してあるものがありますが、それらは
主に子パーツ群との連絡用に使用される関数ですので、使用しないで下さい。

- アニメーション名から再生用インデックスを取得  
public int AnimationGetIndexNo(string AnimationName)

- アニメーションの再生開始  
public bool AnimationPlay(int No, int CountLoop, int StartFrameNo=0, float RateTime=-1.0f)

- アニメーションの再生停止  
public void AnimationStop()

- アニメーションの一時停止設定  
public bool AnimationPause(bool FlagSwitch)

- アニメーションが再生中かの状態取得  
public bool AnimationCheckPlay()

- アニメーションが一時停止中かの状態取得  
public bool AnimationCheckPause()

### コールバックの指定

本アセットでは後述するいくつかの条件を満たした時にユーザーが指定した関数をコールバックさせることができます。  
コールバックの設定・取得を行うにはプロパティを使用します。  
プロパティにnullを指定するとコールバックは行われません。

- アニメーションの再生が終了した時

    プロパティ名: Script_SpriteStudio_PartsRoot.FunctionPlayEnd
    
    デリゲート定義:  
    public delegate bool FunctionCallBackPlayEnd(GameObject ObjectControl);

    引数解説:  
    ObjectControl : コントロール用パーツのゲームオブジェクト

    返値解説:  
    true : 当該スプライトオブジェクトをヒエラルキー内に残存させる  
    false : 当該スプライトオブジェクトをヒエラルキー内から削除する

    注記:  
    本コールバックは、アニメーションのデコード最中に行われると様々な面で
    不都合が生じるため、最終フレームの検知と同じ処理フレームで呼ばれますが、
    （全子パーツのUpdate処理が終了した後で呼ばれる）rootパーツのスクリプトの
    「MonoBehaviour.LateUpdate」の処理内から呼び出されます。

    LateUpdate内での厳密な処理順序は下記の様になります。
    1. 描画
    2. 検知されたユーザーデータのコールバック
    3. 本コールバックの処理

- アニメーションを再生中、ユーザーデータのキーが設定されたフレームに来た時

    プロパティ名: Script_SpriteStudio_PartsRoot.FunctionUserData
    
    デリゲート定義:  
    `public delegate void FunctionCallBackUserData(  
                           GameObject ObjectControl,  
                           string PartsName, 
                           Library_SpriteStudio.SpriteData AnimationDataParts,  
                           int AnimationNo,  
                           int FrameNoDecode,  
                           int FrameNoKeyData,  
                           Library_SpriteStudio.KeyFrame.ValueUser Data  
    );`

    
    引数解説:  
    ObjectControl: コントロール用パーツのゲームオブジェクト  
    PartsName: ユーザーデータが検知されたパーツ名 
    AnimationDataParts: ユーザーデータが検知されたパーツのSpriteStudioアニメーション再生管理情報 
    AnimationNo: ユーザーデータが検知された際のアニメーション番号 
    FrameNo: ユーザーデータが検知された、再生時のフレーム数  
    FrameNoKeyData: 検知されたユーザーデータが定義されている、パーツ内フレーム番号  
    Data: 検知されたユーザーデータ  
    
    注記:
    本コールバックは、アニメーションのデコード最中に行われると様々な面で
    不都合が生じるため、ユーザーデータの検知と同じ処理フレームで呼ばれますが、
    （全子パーツのUpdate処理が終了した後で呼ばれる）rootパーツのスクリプトの
    「MonoBehaviour.LateUpdate」の処理内から呼び出されます。
    
    LateUpdate内での厳密な処理順序は下記の様になります。
    1. 描画
    2. 本コールバックの処理
    3. アニメーション終了時のコールバック

    また下記のプロパティで値が格納されているかどうかを知ることができます。

    - Data.IsNumber : 整数データを所有しているか？
    - Data.IsRectangle : 領域データを所有しているか？
    - Data.IsCoordinate : 頂点データを所有しているか？
    - Data.IsText : テキストデータを所有しているか？
    
    上記の各値が「true」である場合に、下記の各値を取得できます。
    - uint型: Data.Number
    - Rect型: Data.Rectangle
    - Vector2型: Data.Coordinate.Point
    - string型: Data.Text

    ※各プロパティがfalseの場合は値が有効でない（SS上で設定されていない）ことを意味するため、値を取り出そうとしても正常な動作が保証されない事に注意して下さい。

- 自身のコライダが他のコライダと接触した時（トリガー型）

    - 接触開始 プロパティ名: Script_SpriteStudio_PartsRoot.FunctionOnTriggerEnter
    - 接触中   プロパティ名: Script_SpriteStudio_PartsRoot.FunctionOnTriggerStay
    - 接触終了 プロパティ名: Script_SpriteStudio_PartsRoot.FunctionOnTriggerEnd

    ※上記プロパティは、いずれもMonoBehaviourクラスの  
    OnTriggerEnter  
    OnTriggerStay  
    OnTriggerExit  
    に対応しています。
    
    デリゲート定義（共通）:  
    public delegate void FunctionCallBackOnTrigger(Collider Self, Collider Pair);
    
    引数解説:  
    self: 自分自身のコライダ 
    Pair: 自分自身と接触したコライダ
    
    注記:  
    接触を判定する際に、トリガーでの検知とコリジョンコンタクトでの検知の2種類が
    ありますが、これらは、コライダに対してリジッドボディの有無や、その他運用状況
    で使い分ける必要があります。  
    それらの使い分けについては、Unityの仕様を参照して下さい。  
    本、コールバックはアニメーション処理とは非同期に行われます（Unity内の物理・接触
    処理と同期しています）。

- 自身のコライダがコリジョンとの接触した時（コリジョン型）

    - 接触開始　Script_SpriteStudio_PartsRoot.FunctionOnCollisionEnter
    - 接触中  　Script_SpriteStudio_PartsRoot.FunctionOnCollisionStay
    - 接触終了　Script_SpriteStudio_PartsRoot.FunctionOnCollisionEnd
    
    ※上記プロパティは、いずれもMonoBehaviourクラスの  
    OnCollisionEnter  
    OnCollisionStay  
    OnCollisionExit  
    に対応しています。
    
    デリゲート定義（共通）:  
    public delegate void FunctionCallBackOnCollision(Collider Self, Collision Contacts);
    
    引数解説:  
    self: 自分自身のコライダ  
    Contacts: 自分自身のコライダと接触したコリジョンコンタクト情報
    
    注記:  
    接触を判定する際に、トリガーでの検知とコリジョンコンタクトでの検知の2種類が
    ありますが、これらは、コライダに対してリジッドボディの有無や、その他運用状況
    で使い分ける必要があります。  
    それらの使い分けについては、Unityの仕様を参照して下さい。  
    本、コールバックはアニメーション処理とは非同期に行われます（Unity内の物理・接触
    処理と同期しています）。

## 未対応の機能

- アンカー機能

==============================================================================

株式会社ウェブテクノロジ  
http://www.webtech.co.jp/  
Copyright(C) 2013 Web Technology Corp.  

==============================================================================

* SpriteStudio, Web Technologyは、株式会社ウェブテクノロジの登録商標です。
* その他の商品名は各社の登録商標または商標です。
