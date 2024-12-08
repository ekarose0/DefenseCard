using UnityEngine;

public class ResourceDefine : MonoBehaviour
{
    // 각 심볼 이미지
    public const string SymbolImage = "Symbols";

    // CurrontGenealogy.json
    public const string CurrontJsonData = "Assets/Scripts/CurrontGenealogy.json";

    // 카드 프리팹 경로
    public const string SingleCardBack = "Prefab/BackCard";       // 개별 카드 뒷면
    public const string CardBackPack = "Prefab/BackCardPack";     // 카드 뭉치(뒷면)
    public const string SingleCardFront = "Prefab/Card";          // 카드 앞면

    // Scene 경로
    public const string MoneyUI = "UI/Background/Money";                                // MoneyUI접근 경로
    public const string RoundUI = "UI/Background/Round";                                // RoundUI접근 경로
    public const string MakeCardUI = "UI/Background/MakeCard";                          // MakeCardUI 접근경로

    public const string CardTableUI = "UI/Background/CardTable";                        // CardTable 접근경로

    public const string NoticeUI = "UI/Background/CardTable/Notice";                    // NoticeUI 접근경로
    public const string RePlaceButton = "UI/Background/CardTable/RePlacelButton";       // RePlacelButton 접근경로
    public const string UsedButton = "UI/Background/CardTable/UsedButton";              // UsedButton 접근경로
}
