using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConnection : MonoBehaviour {
    public GameObject cursorPrefab;
    public GameObject dwarfPrefab;
    // Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    //void Update () {

    //}
    private void ChangedActiveScene(Scene current, Scene next)
    {
        MakeDwarf();
    }
    public void MakeCursor()
    {
        Instantiate(cursorPrefab);
    }
    public void MakeDwarf()
    {
        Instantiate(dwarfPrefab);
    }
}
