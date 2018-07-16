 using UnityEngine;
 using System.Collections;
 public class RandomAppear : MonoBehaviour {
     
     private Vector3 startPosition;
     private Quaternion startRotation;
     public Transform[] SpawnPoints;

	 public GameObject[] Items;

	 public ScoreControl game;
     public void Start() {

     }

	 void FixedUpdate()
	 {

	 }
     
	 void OnMouseUp()
    {
		Generate();
    }


	void OnCollisionEnter(Collision other)
	{
		game.score+=25;
		Generate();
	}

	   public void Generate() {
		int spawnIndex = Random.Range(0,SpawnPoints.Length);
         transform.position = SpawnPoints[spawnIndex].position;
         transform.rotation = SpawnPoints[spawnIndex].rotation;
     }

 }
