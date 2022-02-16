using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public float speed = 5.0f;
	public float slideSpeed = 2.0f;
	public int jumpCount = 1;

	int defaultJumpCount;


	//アニメーション
	Animator animator;
	//UIを管理するスクリプト
	UIManager uiscript;
	//上半身のコライダー用
	GameObject headCollider;

	Rigidbody rig;


	void Start()
	{
		//変数に必要なデータを格納
		animator = GetComponent<Animator>();
		uiscript = GameObject.Find("Canvas").GetComponent<UIManager>();
		rig = GetComponent<Rigidbody>();
		headCollider = GameObject.Find("HeadCollider");

		defaultJumpCount = jumpCount;

	}



	void Update()
	{
		//前に進む
		transform.position += new Vector3(0, 0, speed) * Time.deltaTime;

		//現在のX軸の位置を取得
		float posX = transform.position.x;

		//右アローキーを押した時
		if (Input.GetKey(KeyCode.RightArrow))
        {
			if (posX < 2.0f)
            {
				transform.position += new Vector3(slideSpeed, 0, 0) * Time.deltaTime;
            }
        }

		//左アローキーを押した時
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			if (posX > -2.0f)
			{
				transform.position -= new Vector3(slideSpeed, 0, 0) * Time.deltaTime;
			}
		}

		//アニメーション
		if (Input.GetKeyDown(KeyCode.DownArrow))
        {
			animator.SetBool("Slide", true);
        }
		if (Input.GetKeyUp(KeyCode.DownArrow))
        {
			animator.SetBool("Slide", false);
		}

		//現在再生されているアニメーション情報を取得
		var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		//取得したアニメーションの名前が一致指定ればtrue
		bool isJump = stateInfo.IsName("Base Layer.Jump");
		bool isSlide = stateInfo.IsName("Base Layer.Slide");

		//ジャンプ
		if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount > 0)
        {
			rig.velocity = Vector3.zero;
			rig.AddForce(new Vector3(0, 6, 0), ForceMode.Impulse);
			animator.SetTrigger("Jump");
			jumpCount--;
        }


		//スライディングしていたら頭の判定をなくす
		if (isSlide)
        {
			headCollider.SetActive(false);
        }
		else
        {
			headCollider.SetActive(true);
        }

		//落下時のGameOver判定
		if (transform.position.y <= -3)
        {
			uiscript.Gameover();
			animator.SetBool("Dead", true);
        }

	}

	// Triggerである障害物にぶつかったとき
	void OnTriggerEnter(Collider colider)
	{
		//ゴールした時
		if (colider.gameObject.tag == "Goal")
		{
			//速度を0にして進むのを止める
			speed = 0;

			//横移動する速度を0にして左右移動できなくする
			slideSpeed = 0;

			//ゴールをしたら正面を向くようにする
			Vector3 lockpos = Camera.main.transform.position;
			lockpos.y = transform.position.y;
			transform.LookAt(lockpos);

			//アニメーション
			animator.SetBool("Win", true);

			//UIの表示
			uiscript.Goal();
			
		}
	}


    //Triggerでない障害物にぶつかったとき
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Barrier")
        {
			animator.SetBool("Dead", true);
			uiscript.Gameover();
        }

		if (collision.gameObject.tag == "Ground")
        {
			jumpCount = defaultJumpCount;
        }
    }
}
