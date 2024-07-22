using GoogleSheet.Core.Type;

public enum ECriminalState
{
    NORMAL = 0,
    IN_FIRE = 1,
    IN_WATER = 2,

    IS_Cloacking = 4,
    IS_SLOW = 8,
    IS_STUNNED = 16,
    IS_INVINCIBLE = 32,
    IS_CAUGHTED = 64,
}

public enum ECriminalScaleLevel
{
    LEVEL0 = 10,
    LEVEL1,
    LEVEL2,
    LEVEL3,
    LEVEL4,
    LEVEL5,
}

public enum EFireState
{
    NORMAL = 0,
    ON_CRIMINAL = 1,
    IN_WATER = 2,
}

public enum State
{
    WaitingToStart,
    CountdownToStart,
    GamePlaying,
    GameOver,
}

public enum EMapInformation
{
    MAP1,
    MAP2,
    MAP3,
    MAP4,
}

public enum EScene
{
    IntroScene,
    LobbyScene,
    ReadyRoomScene,
    BattleScene,
}

public enum EplayerState
{
    IDLE,
    MOVE,
    DASH,
}

public enum EJob
{
    CIRIMINAL,
    FIREFIGHTER,
}

public enum ECharacterState
{
    NORMAL = 0,
    IN_WATER = 1,
    IN_FIRE = 2,
    IS_STUNNED = 4,
    IS_INVINCIBLE = 8,
}


[UGS(typeof(ERangeType))]
public enum ERangeType
{
    NONE,
    CONSTANT,
    CONE,
    LINE,
}

[UGS(typeof(ETargetType))]
public enum ETargetType
{
    NONE,
    ALL,
    SELF,
    ALLIANCE,
    ENEMY,
}