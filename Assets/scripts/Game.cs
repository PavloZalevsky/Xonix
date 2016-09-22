using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public SpriteRenderer sprite;
	public Transform grassLand;
	public Transform dirtRoad;
	public Transform city;
	public Transform wolf;
	public Transform bandit;

	public int randNum = 0;
	public int xCubes;
	public int yCubes;

	public bool fighting = false;
	public bool enterWolf = false;

	void Start () {

        Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
        Debug.Log(height);
        Debug.Log(width);
        width *= 0.5f;
		height *= 0.5f;

		var startX = Camera.main.transform.position.x - width;
		var startyY = Camera.main.transform.position.y - height;

		startX += (sprite.bounds.size.x/2);
		startyY += (sprite.bounds.size.y/2);



		for ( int z = 0; z < yCubes; z++ )
		{
			for ( int x = 0; x < xCubes; x++ )
			{
				float rnd = Random.value;
				Instantiate(sprite, new Vector3(startX  + (x * sprite.bounds.size.x),startyY+ (z * sprite.bounds.size.y), 0), Quaternion.identity);	

			//	Instantiate(sprite, new Vector3(startX + (x * sprite.bounds.size.x), startyY + (z * sprite.bounds.size.y), 0), Quaternion.identity);	
			}
		}


//		for ( int z = 0; z < zCubes; z++ )
//		{
//			for ( int x = 0; x < xCubes; x++ )
//			{
//				float rnd = Random.value;
//				if ( rnd < 0.05 )
//				{
//					Instantiate(city, new Vector3(x, z, 0), Quaternion.identity);
//				}
//				else if ( rnd < 0.15 )
//				{
//					Instantiate(dirtRoad, new Vector3(x, z, 0), Quaternion.identity);
//				}
//				else 
//				{
//					Instantiate(grassLand, new Vector3(x, z, 0), Quaternion.identity);
//					randNum = Random.Range(0,50);
//					if(randNum <= 3){
//						Instantiate(wolf,new Vector3(x,z,0), Quaternion.identity);
//					}
//					else if(randNum ==4){
//						Instantiate(bandit, new Vector3(x,z,0), Quaternion.identity);
//					}
//				}
//			}
//		}
	}
}
