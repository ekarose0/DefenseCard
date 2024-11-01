using UnityEngine;

public enum CardType 
{   spades, 
    diamonds, 
    hearts, 
    clubs
}

[System.Serializable]
public class CardInfo
{
    public CardType cardType;
    public Sprite cardImage;
    public int cardNum;
    public bool isSelected;
}

[CreateAssetMenu(fileName = "Cards", menuName = "ScriptableObjects/CardsSO")]
public class CardSO : ScriptableObject
{
    public CardInfo[] cardInfo;
}
