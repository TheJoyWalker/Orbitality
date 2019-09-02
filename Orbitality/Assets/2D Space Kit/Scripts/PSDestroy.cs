using UnityEngine;
using System.Collections;
#pragma warning disable 618
 
public class PSDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
