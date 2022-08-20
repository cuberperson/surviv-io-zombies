using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject howToPanel;
    public void Pressed()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void PressedHowTo(bool open)
    {
        howToPanel.SetActive(open);
    }
}
