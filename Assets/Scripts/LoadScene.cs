using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadThisScene(int sceneNr)
    {
        SceneManager.LoadScene(sceneNr);
    }
}
