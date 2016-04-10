﻿using UnityEngine;
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
	private int food;

	void Update(){
		if (!GameManager.instance.playersTurn) {
			return;
		}

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		PreventDiagonalMove (horizontal, vertical);

		if (horizontal != 0 || vertical != 0 ) {
			AttemptMove<Wall> (horizontal, vertical);
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
