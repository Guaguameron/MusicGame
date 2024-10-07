using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Note;

    public GameObject Note1;
    public GameObject Note2;

    float myTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myTime += Time.deltaTime;
        if(myTime > 1)
        {
            GameObject instance = Instantiate(Note, Note1.transform.position, Quaternion.identity);
            instance.GetComponent<Note>().track = 1;
            instance = Instantiate(Note, Note2.transform.position, Quaternion.identity);
            instance.GetComponent<Note>().track = 2;
            myTime = 0;
        }
    }

    private void GenerateNote()
    {

    }
}
