﻿using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;

    private float _experience = 0;
    private float _expRate = 1;
    private float _expBenchmark = 10;
	private int _maxLevel = 8;

    public int CurrentLevel {get; private set;}

    private Temple _temple;

	// Use this for initialization
	void Start () {
        CurrentLevel = 4;

        instance = this;
        _temple = FindObjectOfType<Temple>();

    }
    void Update()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
            AddExperience();
        if (Input.GetKey(KeyCode.KeypadMinus))
            LowerExperience();
    }
	void LevelUp()
    {
        Debug.Log("LevelUp: " + CurrentLevel);
		StartCoroutine(LevelTransition(true));
    }
    public void LevelDown()
    {
        Debug.Log("LevelDown: " + CurrentLevel);
		StartCoroutine(LevelTransition(false));
    }
    public void AddExperience()
    {
        _experience += _expRate;
        if (_experience >= _expBenchmark)
            LevelUp();

    }
    public void LowerExperience()
    {
        LevelDown();
    }
    void SetBenchMark()
    {
        _expBenchmark = 10*CurrentLevel;
	}
	void GameOver() {
		Debug.Log("GAMEOVER!!!!!");
		Application.LoadLevel("LoseScene");
	}
	void GameWon() {
		Debug.Log("YOU WON THE GAME!!");
		Application.LoadLevel("WinScene");
	}
	public float PercentageAmount
    {
       get { return _experience / _expBenchmark; }
	}

	public IEnumerator LevelTransition(bool raise) {
		Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
		foreach(Enemy enemy in enemies) {
			enemy.Pause();
		}
		_experience = 0;
		SetBenchMark();
		yield return new WaitForSeconds(raise? 1 : 2);
		if (raise) {
			CurrentLevel++;
			_temple.RaiseTemple();
			FindObjectOfType<EnemyManager>().WhipeEnemies();
			if (CurrentLevel >= _maxLevel) {
				yield return new WaitForSeconds(2);
				GameWon();
			}
		} else {
			CurrentLevel--;
			_temple.LowerTemple();
			FindObjectOfType<EnemyManager>().WhipeEnemies();
			if (CurrentLevel <= 0) {
				yield return new WaitForSeconds(2);
				GameOver();
			}
		}
	}
}
