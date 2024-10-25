using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds;
    public float scrollSpeed = 1f;
    public float resetPosition = -10f;
    public float startPosition = 10f;

    private void Update()
    {
        if (!PauseGame.isPaused)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

                if (backgrounds[i].transform.position.x <= resetPosition)
                {
                    backgrounds[i].transform.position = new Vector3(startPosition, backgrounds[i].transform.position.y, backgrounds[i].transform.position.z);
                }
            }
        }
    }
}