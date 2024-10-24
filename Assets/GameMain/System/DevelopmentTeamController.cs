using UnityEngine;
using UnityEngine.UI;

public class DevelopmentTeamController : MonoBehaviour
{
    
    public GameObject developmentTeamImage;

   
    public Button showDevelopmentTeamButton; 
    public Button closeButton; 

    void Start()
    {
        showDevelopmentTeamButton.onClick.AddListener(ShowDevelopmentTeamImage);

        closeButton.onClick.AddListener(HideDevelopmentTeamImage);

        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(false);
        }
    }

   
    void ShowDevelopmentTeamImage()
    {
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(true);
        }
    }

   
    void HideDevelopmentTeamImage()
    {
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(false);
        }
    }
}
