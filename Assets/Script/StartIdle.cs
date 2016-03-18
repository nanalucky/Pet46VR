using UnityEngine;
using System.Collections;

public class StartIdle : MonoBehaviour {

	public int minInterval = 1;
	public int maxInterval = 3;
	public string[] animations = new string[]{"Idle3", "Idle2", "Idle2"};
	public string stand = "Idle1";

	private float endTime;
	private int count;
	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
		endTime = Time.time + Random.Range (minInterval, maxInterval);
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(firstFrame)
		{
			firstFrame = false;
			gameObject.GetComponent<DogController>().EnableLookatIK(false);
		}

		if(endTime > 0)
		{
			if(Time.time > endTime)
			{
				endTime = 0;
				//GameObject.FindGameObjectWithTag("dog").GetComponent<Animator>().Play(animations[Random.Range(0, animations.Length)], 0);
				GameObject.FindGameObjectWithTag("dog").GetComponent<Animator>().CrossFade(animations[(count++)%animations.Length], 0.25f); 
			}
		}
		else
		{
			if(GameObject.FindGameObjectWithTag("dog").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(stand))
			{
				endTime = Time.time + Random.Range (minInterval, maxInterval);
			}
		}
	}
}
