using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int money;

    private void Awake()
    {
            Instance = this;
    }

    public void ChangeMoney(int amount)
    {
        money += amount;
        UIManager.Instance.UpdateMoneyText(money); 
    }
}
