﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



	// Use this for initialization
	void Start () {
		//Animatorコンポーネントを取得
		this.myAnimator = GetComponent<Animator>();
		//走るアニメーションを開始
		//this.myAnimator.SetFloat ("Speed", 1);
		//Rigidbodyコンポーネントを取得（追加）
		this.myRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

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
		 if (Input.GetKey (KeyCode.LeftArrow)  && -this.movableRange < this.transform.position.x) {
			  //左に移動（追加）
			 this.myRigidbody.AddForce (-this.turnForce, 0, 0);
		} else if (Input.GetKey (KeyCode.RightArrow)  && this.transform.position.x < this.movableRange) {
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
		}
		//ゴール地点に到達した場合（追加）
		if (other.gameObject.tag == "GoalTag") {
			this.isEnd = true;
		}
		//コインに衝突した場合（追加）
		if (other.gameObject.tag == "CoinTag") {
			//パーティクルを再生（追加）
			GetComponent<ParticleSystem> ().Play ();
			//接触したコインのオブジェクトを破棄（追加）
			Destroy (other.gameObject);
		}
	}
}
