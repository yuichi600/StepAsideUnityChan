using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityChanController : MonoBehaviour {

	//アニメーションするためのコンポーネントを入れる
	private Animator myAnimator;
	//Unityちゃんを移動させるコンポーネントを入れる（追加）
	private Rigidbody myRigidbody;
	//前進するための力（追加）
	private float forwardForce = 800.0f;
	//左右に移動するための力（追加）
	private float turnForce = 500.0f;
	//左右の移動できる範囲（追加）
	private float movableRange = 3.4f;
	//ジャンプするための力（追加）
	private float upForce = 500.0f;
	 //動きを減速させる係数（追加）
	 private float coefficient = 0.95f;
	//ゲーム終了の判定（追加）
	private bool isEnd = false;
	//ゲーム終了時に表示するテキスト（追加）
	private GameObject stateText;
	//得点を表示するテキスト（追加）
	private GameObject scoreText;
	//得点
	private int score;
	//左ボタン押下の判定（追加）
	private bool isLButtonDown = false;
	//右ボタン押下の判定（追加）
	private bool isRButtonDown = false;
	//Unityちゃんの位置を確認
	private GameObject UnityPlace;

	//課題様追加変数
	//carPrefabを入れる
	public GameObject carPrefab;
	//coinPrefabを入れる
	public GameObject coinPrefab;
	public GameObject conePrefab;
	//アイテムを出すx方向の範囲
	private float posRange = 3.4f;
	//15m置きを記録
	private double distance15 = 0;
	//アイテムリスト
	private GameObject[] item_list;
	//Objectナンバー
	private int item_no = 0;
	

	// Use this for initialization
	void Start () {
		//Animatorコンポーネントを取得
		this.myAnimator = GetComponent<Animator>();
		//走るアニメーションを開始
		this.myAnimator.SetFloat ("Speed", 1);
		//Rigidbodyコンポーネントを取得（追加）
		this.myRigidbody = GetComponent<Rigidbody>();
		this.stateText =GameObject.Find("GameResultText");
		//スコアのオブジェクトを取得
		this.scoreText = GameObject.Find("ScoreText");
		
		
	}
	
	// Update is called once per frame
	void Update () {

		//最初の15m移動時の処理
		if(this.distance15==0){
			Debug.Log(this.transform.position.z);
			this.distance15=this.transform.position.z;
		//継続的な15m移動時の処理
		}else{
			
			if((this.distance15-this.transform.position.z)>15||(this.distance15-this.transform.position.z)<-15){
				
				this.distance15=this.transform.position.z;
				item_list[0] = this.CreateItem(this.transform.position.z);
				Debug.Log(item_list );
				//item_no++;
			}
		}

		//ゲーム終了ならUnityちゃんの動きを減衰する（追加）
		if (this.isEnd) {
			this.forwardForce *= this.coefficient;
			this.turnForce *= this.coefficient;
			this.upForce *= this.coefficient;
			this.myAnimator.speed *= this.coefficient;
		}
		//Unityちゃんに前方向の力を加える（追加）
		this.myRigidbody.AddForce (this.transform.forward * this.forwardForce);
		//Unityちゃんを矢印キーまたはボタンに応じて左右に移動させる（追加）
		if ((Input.GetKey (KeyCode.LeftArrow)|| this.isLButtonDown)  && -this.movableRange < this.transform.position.x) {
			  //左に移動（追加）
			 this.myRigidbody.AddForce (-this.turnForce, 0, 0);
		} else if ((Input.GetKey (KeyCode.RightArrow) || this.isRButtonDown)  && this.transform.position.x < this.movableRange) {
			//右に移動（追加）
			this.myRigidbody.AddForce (this.turnForce, 0, 0);
		}
		
		//Jumpステートの場合はJumpにfalseをセットする（追加）
		if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName ("Jump")) {
			this.myAnimator.SetBool ("Jump", false);
		}
		
		//ジャンプしていない時にスペースが押されたらジャンプする（追加）
		if (Input.GetKeyDown(KeyCode.Space) && this.transform.position.y < 0.5f) {
			//ジャンプアニメを再生（追加）
			this.myAnimator.SetBool ("Jump", true);
			//Unityちゃんに上方向の力を加える（追加）
			this.myRigidbody.AddForce (this.transform.up * this.upForce);
		}

	}
	//トリガーモードで他のオブジェクトと接触した場合の処理（追加）
	void OnTriggerEnter(Collider other) {
		//障害物に衝突した場合（追加）
		if (other.gameObject.tag == "CarTag" || other.gameObject.tag == "TrafficConeTag") {
			this.isEnd = true;
			//GameoverTextにゲームオーバを表示
			this.stateText.GetComponent<Text> ().text = "Game Over";
		}
		//ゴール地点に到達した場合（追加）
		if (other.gameObject.tag == "GoalTag") {
			Debug.Log("Goal = "+other.gameObject.tag);
			this.isEnd = true;
			//stateTextにGAME CLEARを表示（追加）
			this.stateText.GetComponent<Text>().text = "CLEAR!!";
		}
		//コインに衝突した場合（追加）
		if (other.gameObject.tag == "CoinTag") {
			//得点を追加
			this.score += 10;
			//得点を表示
			this.scoreText.GetComponent<Text> ().text = "Score "+this.score+"pt";
			//パーティクルを再生（追加）
			GetComponent<ParticleSystem> ().Play ();
			//接触したコインのオブジェクトを破棄（追加）
			Destroy (other.gameObject);
		}
	}
	//ジャンプボタンを押した場合の処理（追加）
	public void GetMyJumpButtonDown() {
		 if (this.transform.position.y < 0.5f) {
			 this.myAnimator.SetBool ("Jump", true);
			 this.myRigidbody.AddForce (this.transform.up * this.upForce);
		 }
	}
	//左ボタンを押し続けた場合の処理（追加）
	public void GetMyLeftButtonDown() {
		this.isLButtonDown = true;
	}
	//左ボタンを離した場合の処理（追加）
	public void GetMyLeftButtonUp() {
		this.isLButtonDown = false;
	}
	//右ボタンを押し続けた場合の処理（追加）
	public void GetMyRightButtonDown() {
		this.isRButtonDown = true;
	}
	//右ボタンを離した場合の処理（追加）
	public void GetMyRightButtonUp() {
		this.isRButtonDown = false;
	}

	public GameObject CreateItem(float now_place){

		GameObject coin = Instantiate (coinPrefab) as GameObject;
		coin.transform.position = new Vector3 (-3.4f, coin.transform.position.y,now_place+30);

		return coin;
		/*int num = Random.Range (1, 11);
                        if (num <= 2) {
                                //コーンをx軸方向に一直線に生成
                                for (float j = -1; j <= 1; j += 0.4f) {
                                        GameObject cone = Instantiate (conePrefab) as GameObject;
                                        cone.transform.position = new Vector3 (4 * j, cone.transform.position.y,now_place+40;
                                }
                        } else {

                                //レーンごとにアイテムを生成
                                for (int j = -1; j <= 1; j++) {
                                        //アイテムの種類を決める
                                        int item = Random.Range (1, 11);
                                        //アイテムを置くZ座標のオフセットをランダムに設定
                                        int offsetZ = Random.Range(-5, 6);
                                        //60%コイン配置:30%車配置:10%何もなし
                                        if (1 <= item && item <= 6) {
                                                //コインを生成
                                                GameObject coin = Instantiate (coinPrefab) as GameObject;
                                                coin.transform.position = new Vector3 (posRange * j, coin.transform.position.y,now_place+40);
                                        } else if (7 <= item && item <= 9) {
                                                //車を生成
                                                GameObject car = Instantiate (carPrefab) as GameObject;
                                                car.transform.position = new Vector3 (posRange * j, car.transform.position.y,now_place+40);
                                        }
                                }
                        }
						*/
	} 
}
