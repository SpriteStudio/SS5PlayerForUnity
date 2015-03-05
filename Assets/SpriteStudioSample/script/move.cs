using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {

	public GameObject Prefab;
	private GameObject ObjectInstance;
	
	// Use this for initialization
	void Start()
	{
		ObjectInstance = (GameObject)(Instantiate(Prefab));
		ObjectInstance.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(null != ObjectInstance)
		{
			Vector3 Position = transform.localPosition;
			Position.x += 0.5f;
			transform.localPosition = Position;
		}
	}
}
