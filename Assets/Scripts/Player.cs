using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private Vector2 touchOrigin = -Vector2.one;
	private int food;

	void Update(){
		if (!GameManager.instance.playersTurn) {
			return;
		}



		int horizontal = 0;
		int vertical = 0;

		#if UNITY_STANDALONE || UNITY_WEBPLAYER
			keyboardControls (ref horizontal, ref vertical);
		#else
			touchControls (ref horizontal, ref vertical); 
		#endif

		if (horizontal != 0 || vertical != 0 ) {
			AttemptMove<Wall> (horizontal, vertical);
		}


	}

	void keyboardControls (ref int horizontal, ref int vertical)
	{
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		PreventDiagonalMove (horizontal, vertical);
	}

	void touchControls (ref int horizontal, ref int vertical)
	{
		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches [0];
			if (myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
			}
			else
				if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
					Vector2 touchEnd = myTouch.position;
					float x = touchEnd.x - touchOrigin.x;
					float y = touchEnd.y - touchOrigin.y;
					touchOrigin.x = -1;
					bool isHorizontalSwipe = Mathf.Abs (x) > Mathf.Abs (y);
					if (isHorizontalSwipe) {
						horizontal = x > 0 ? 1 : -1;
					}
					else {
						vertical = y > 0 ? 1 : -1;
					}
				}
		}
	}

	protected override void Start () {
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start ();
	}

	protected override void AttemptMove<T> (int xDir, int yDir)
	{
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}

	protected override void OnCantMove<T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart(){
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseFood(int loss){
		animator.SetTrigger ("playerHit");
		AlterFood (-loss);
		CheckIfGameOver ();
	}

	private void OnDisable(){
		GameManager.instance.playerFoodPoints = food;
	}

	private void CheckIfGameOver(){
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();

			GameManager.instance.GameOver ();
		}
	}

	private void PreventDiagonalMove(int horizontal, int vertical){
		if (horizontal != 0) {
			vertical = 0;
		}
	}

	private void OnTriggerEnter2D(Collider2D other){
		switch (other.tag) {
		case "Exit":
			Invoke("Restart", restartLevelDelay);
			enabled = false;
			break;

		case "Food":
			AlterFood (pointsPerFood);
			other.gameObject.SetActive (false);
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			break;

		case "Soda":
			AlterFood (pointsPerSoda);
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
			break;
		}
	}

	private void AlterFood(int amt){
		food += amt;
		string opp = amt >= 0 ? "+" : "";
		foodText.text = opp + " " + amt + " Food: " + food;
	}
}
