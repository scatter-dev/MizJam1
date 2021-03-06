﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum Face
{
	Sword,
	Shield,
	Potion
}

public class DiceController : MonoBehaviour
{
	public PlayerController player;
	public EnemyController enemy;
	public CameraController camera;
	public DieController[] dice;
	public RollButtonController buttons;
	public int framesPerRoll;
	public int rolls;
	public LayerMask dieLayer;
	public GameObject[] holdBoxes;
	List<Face> heldDice;
	int finishedDice = 0;
	int rollFrameCount = 0;
	int turnRolls = 3;
	int damage;
	int healing;
	int blocking;
	AudioSource sfx;

	public int Damage
	{
		get
		{
			return damage;
		}
	}

	public int Healing
	{
		get
		{
			return healing;
		}
	}

	public int Blocking
	{
		get
		{
			return blocking;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		sfx = GetComponent<AudioSource>();
		heldDice = new List<Face>();
		System.Random r = new System.Random();
		Array faceTypes = Enum.GetValues(typeof(Face));
		foreach (DieController die in dice)
		{
			die.diceController = this;
			for (int i = 0; i < 6; i++)
			{
				die.SetFace((Face)faceTypes.GetValue(r.Next(faceTypes.Length)), i);
			}
		}
    }

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, dieLayer))
			{
				DieController dc = hit.transform.gameObject.GetComponent<DieController>();
				if (dc && dc.Hold(holdBoxes[heldDice.Count].transform.position))
				{
					heldDice.Add(dc.GetUpwardFace());
				}
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        if (rollFrameCount > 0)
		{
			if (rollFrameCount % framesPerRoll == 0)
			{
				foreach (DieController die in dice)
				{
					if (!die.Held)
					{
						die.Roll(transform.position);
					}
				}
			}
			rollFrameCount--;
		}
    }

	public void RollDice()
	{
		if (turnRolls > 0)
		{

			for (int i = 0; i < dice.Length; i++)
			{
				if (!dice[i].stopped)
				{
					return;
				}
			}

			sfx.Play();
			turnRolls--;
			buttons.RemoveButton();
			rollFrameCount = rolls * framesPerRoll;
		}
		else
		{

		}
	}

	public void TryEndTurn()
	{
		finishedDice++;
		if (finishedDice == 5)
		{
			damage = 0;
			healing = 0;
			blocking = 0;
			finishedDice = 0;
			for (int i = 0; i < 5; i++)
			{
				if (heldDice[i].Equals(Face.Sword))
				{
					Debug.Log("hitting enemy");
					damage += 3;
				}
				else if (heldDice[i].Equals(Face.Potion))
				{
					healing += 2;
				}
				else if (heldDice[i].Equals(Face.Shield))
				{
					blocking += 2;
				}
				dice[i].GetComponent<Rigidbody>().useGravity = true;
				
			}
			ApplyBonuses();
			Debug.Log("hitting player");
			heldDice.Clear();
			turnRolls = 3;
			buttons.ReplaceButtons();
			StartCoroutine(camera.Turn());
		}
	}

	public void ResetDicePosition()
	{
		for (int i = 0; i < 5; i++)
		{
			dice[i].Reset();
			dice[i].transform.position = transform.position + Vector3.right * (i - 2) * 2 + Vector3.up * 2;
			dice[i].stopped = true;
		}
	}

	private void ApplyBonuses()
	{
		Dictionary<Face, int> facedict = new Dictionary<Face, int>();
		facedict.Add(Face.Potion, 0);
		facedict.Add(Face.Shield, 0);
		facedict.Add(Face.Sword, 0);
		foreach (Face f in heldDice)
		{
			facedict[f]++;
		}
		if (facedict.Values.Any(v => v == 5))
		{
			healing += 10;
			damage += 15;
		}
		else if (facedict.Values.Any(v => v == 4))
		{
			damage += 10;
		}
		else if (facedict.Values.Any(v => v == 3))
		{
			blocking += 3;
		}
		healing += facedict.Values.Where(v => v == 2).Count() * 2;
	}
}
