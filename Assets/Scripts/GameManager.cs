using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;

	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;
	public float turnDelay = .1f;

	private int level = 3;
	private List<Enemy> enemies;
	private bool enemiesMoving;


	public void GameOver(){
		enabled = false;
	}

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame(){
		enemies.Clear ();
		boardScript.SetupScene (level);
	}
		
	void Update () {
		if (playersTurn || enemiesMoving) {
			return;
		}

		StartCoroutine (MoveEnemies());
	}

	public void AddEnemyToList(Enemy script){
		enemies.Add (script);
	}

	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);

		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		foreach (var enemy in enemies) {
			enemy.MoveEnemy ();
			yield return new WaitForSeconds (enemy.moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;

	}
}
