using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnection : MonoBehaviour {
    public GameObject cursorPrefab;
    public GameObject dwarfPrefab;
	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
    public void MakeCursor()
    {
        Instantiate(cursorPrefab);
    }
    public void MakeDwarf()
    {
        Instantiate(dwarfPrefab);
    }
}
