using UnityEngine;

public class KeyManager : MonoBehaviour
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
