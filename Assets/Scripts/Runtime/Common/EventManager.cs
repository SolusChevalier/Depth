using System;

public static class EventManager
{
    public static Action OnGamePaused;
    public static Action OnGameResumed;
    public static Action OnGameStarted;
    public static Action OnGameEnded;
    public static Action NextTurn;
    public static Action<int> PlayerWin;
    public static Action<int> UpgradeBoatSpeed;
    public static Action<int> UpgradeFishingSpeed;
    public static Action<int> UpgradeCargoSpace;
    public static Action<int> UpgradeBoat;
    public static Action<int, int> SellFish;
}