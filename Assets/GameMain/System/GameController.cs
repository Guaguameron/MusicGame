using cfg;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Note;

    public GameObject Note1;
    public GameObject Note2;

    float myTime = 0;
    private List<NoteModel> noteList = new List<NoteModel>();

    public Tables DataTables { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        LoadDataTables();
        // ��ʱֻ�õ�һ��noteList
        noteList = DataTables.TbNoteMap.DataList[0].NodeList;
    }

    // Update is called once per frame
    void Update()
    {
        myTime += Time.deltaTime;
        foreach (NoteModel model in noteList)
        {
            if (model.Time <= myTime)
            {
                GameObject instance;
                if (model.Track == 1)
                {
                    instance = Instantiate(Note, Note1.transform.position, Quaternion.identity);
                }
                else{
                    instance = Instantiate(Note, Note2.transform.position, Quaternion.identity);
                }
                instance.GetComponent<Note>().track = model.Track;
                instance.GetComponent<Note>().noteSpeed = model.Speed;
                noteList.Remove(model);
            }
        }
    }

    private void GenerateNote()
    {

    }

    public void LoadDataTables()
    {
        string gameConfDir = $"{Application.streamingAssetsPath}/DataTable";
        DataTables = new Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
    }
}
