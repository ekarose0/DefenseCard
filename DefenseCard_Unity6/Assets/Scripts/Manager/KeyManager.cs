using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.AnimeManager.StartInitialCardAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.Instance.AnimeManager.ToggleCardTableAnimation();
        }
    }
}
