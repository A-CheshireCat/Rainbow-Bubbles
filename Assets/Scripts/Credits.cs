using UnityEngine;

public class Credits : MonoBehaviour
{
    public bool credits;
    public GameObject creditsUi;

    public void ShowHideCredits()
    {
        if (!credits)
            creditsUi.SetActive(true);
        else if(credits)
            creditsUi.SetActive(false);
        credits = !credits;
    }
}
