using cfg;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject Note;
    public GameObject Note1;
    public GameObject Note2;

    public Slider musicProgressBar; // ������
    public AudioSource audioSource;

    private GameObject fillArea; // �������� Fill Area
    float myTime = 0;
    private List<NoteModel> noteList = new List<NoteModel>();

    void Start()
    {
        PlayNoteModel.Start();
        // ��ʱֻ�õ�һ��noteList
        noteList = PlayNoteModel.DataTables.TbNoteMap.DataList[0].NodeList;
              
        fillArea = musicProgressBar.transform.Find("Fill Area").gameObject;

        if (fillArea != null)
        {
            fillArea.SetActive(false);
        }

        if (audioSource != null && musicProgressBar != null)
        {
            musicProgressBar.maxValue = audioSource.clip.length;
            musicProgressBar.value = 0;
            musicProgressBar.interactable = false;
        }
    }

    void Update()
    {
        // �����ֿ�ʼ����ʱ����ʾ����������䲿��
        if (audioSource != null && musicProgressBar != null && audioSource.isPlaying)
        {
            if (fillArea != null && !fillArea.activeSelf)
            {
                fillArea.SetActive(true);
            }
            musicProgressBar.value = audioSource.time;
        }

        myTime += Time.deltaTime;

        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            NoteModel model = noteList[i];
            if (model.Time <= myTime)
            {
                GameObject instance;
                if (model.Track == 1)
                {
                    instance = Instantiate(Note, Note1.transform.position, Quaternion.identity);
                }
                else
                {
                    instance = Instantiate(Note, Note2.transform.position, Quaternion.identity);
                }
                instance.GetComponent<Note>().track = model.Track;
                instance.GetComponent<Note>().noteSpeed = model.Speed;

                noteList.RemoveAt(i);
            }
        }
    }

    private void GenerateNote()
    {

    }
}
