using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
	public static MapSpawner Instance { get; private set; }
	[SerializeField] private float speed = 2f;
	[SerializeField] private int count = 0;
	[SerializeField] private int pivot = Enum.GetNames(typeof(StateManager.States)).Length-2;
	[SerializeField] StateManager.States currentState = StateManager.States.JumpState;
	[SerializeField] private List<Transform> padApear = new List<Transform>();
	[SerializeField] private List<Transform> jumpStatePrefabs = new List<Transform>();
	[SerializeField] private List<Transform> flyStatePrefabs = new List<Transform>();
	[SerializeField] private List<Transform> ziczacStatePrefabs = new List<Transform>();
	[SerializeField] private List<Transform> spiderStatePrefabs = new List<Transform>();
	private string padName;

	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		SpawnOnceWithPos(Vector2.zero);
	}

	void FixedUpdate()
	{
		CaculateSpeed();
	}
	public void SpawnOnceWithPos(Vector2 pos)
	{
		padName = "PadBase";
		Transform pad = SpawnManager.Instance.Spawn(this.padName, pos.x, pos.y, Quaternion.identity);
		if (pad != null)
		{
			padApear.Add(pad);
			pad.gameObject.SetActive(true);
		}
	}
	public void RemovePad(Transform transform)
	{
		if (padApear.Contains(transform))
		{
			padApear.Remove(transform);
		}
		else
		{
			Debug.Log("Pad not found in the list");
		}
	}
	public void SetBase()
	{
		List<Transform> padTemp = new List<Transform>();
		foreach (Transform curpad in padApear)
		{
			SpawnManager.Instance.Despawn(curpad);
			Transform pad = MapSpawner.Instance.TransSpawnOnceWithPos(curpad.transform.position,"PadBase");
			pad.GetComponent<Pad>().SetSpawn(curpad.GetComponent<Pad>().GetSpawn());
			padTemp.Add(pad);
		}
		padApear = new List<Transform>(padTemp);
	}
	public void MSetBase()
	{
		List<Transform> padTemp = new List<Transform>();
        foreach (Transform curpad in padApear)
		{
            SpawnManager.Instance.Despawn(curpad);
            Transform pad = MapSpawner.Instance.TransSpawnOnceWithPos(curpad.transform.position,"MoneyPadBase");
            pad.GetComponent<Pad>().SetSpawn(curpad.GetComponent<Pad>().GetSpawn());
            padTemp.Add(pad);
        }
        padApear = new List<Transform>(padTemp);
    }
	public Transform TransSpawnOnceWithPos(Vector2 pos,string PadName)
	{
		padName = PadName;
		Transform pad = SpawnManager.Instance.Spawn(this.padName, pos.x, pos.y, Quaternion.identity);
		if (pad != null)
		{
			pad.gameObject.SetActive(true);
		}
		return pad;
	}
	public void SpawningWithPos(Vector3 pos)
	{
		StateManager.States state = StateManager.Instance.GetState();
		CheckCurrentState();

		switch (state)
		{
			case StateManager.States.JumpState:
				padName = jumpStatePrefabs[UnityEngine.Random.Range(pivot, jumpStatePrefabs.Count)].name;
				break;
			case StateManager.States.FlyState:
				padName = flyStatePrefabs[UnityEngine.Random.Range(pivot, flyStatePrefabs.Count)].name;
				break;
			case StateManager.States.ZiczacState:
				padName = ziczacStatePrefabs[UnityEngine.Random.Range(pivot, ziczacStatePrefabs.Count)].name;
				break;
			case StateManager.States.SpiderState:
				padName = spiderStatePrefabs[UnityEngine.Random.Range(pivot, spiderStatePrefabs.Count)].name;
				break;
			default:
				padName = jumpStatePrefabs[UnityEngine.Random.Range(pivot, jumpStatePrefabs.Count)].name;
				break;
		}
		Transform pad = SpawnManager.Instance.Spawn(this.padName, pos.x, pos.y, Quaternion.identity);
		if (pad != null)
		{
			padApear.Add(pad);
			pad.gameObject.SetActive(true);
		}
	}
	public void CheckCurrentState()
	{
		if(StateManager.Instance.GetState() != currentState)
		{
			currentState = StateManager.Instance.GetState();
			count = 0;
			pivot = Enum.GetNames(typeof(StateManager.States)).Length - 2;
		}
		else
		{
			count++;
			if (count > 2)
			{
				pivot = 0;
			}
		}
	}
	public float GetBaseSpeed()
	{
		return speed;
	}
	void CaculateSpeed()
	{
		float Score = UIManager.Instance.GetScore();
		if (Score > 10000f)
		{
			speed = 26f;
		}
		else if (Score >= 1000 && Score <= 10000f)
		{
			speed = Mathf.Sqrt((Score + 222) / 15);
		}
		else
		{

			speed = 9f;

		}
	}
}
