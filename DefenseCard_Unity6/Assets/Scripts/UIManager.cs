using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Text moneyTxt;

    private void Awake()
    {
         Instance = this;
    }

    private void Start()
    {
        UpdateMoneyText(GameManager.Instance.money);
    }

    public void UpdateMoneyText(int money)
    {
         moneyTxt.text = money.ToString();
    }
}
