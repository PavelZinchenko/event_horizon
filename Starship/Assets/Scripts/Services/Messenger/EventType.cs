public enum EventType
{
    SessionCreated,

    DatabaseLoaded,

    ConstructorShipChanged, // GameModel.Ship

    GameSettingsChanged, // string key

    WindowOpened,
    WindowClosed,
    EscapeKeyPressed,

    AccountStatusChanged, // Status
    CloudDataLoaded,
    CloudGamesReceived,
    CloudStorageStatusChanged, // CloudStorageStatus

    MultiplayerStatusChanged, // Status
    EnemyFleetLoaded, // IPlayerInfo
    ArenaEnemyFound, // IPlayerInfo

    AchievementUnlocked, // IAchievement

    PlayerPositionChanged, // int starId
    FocusedPositionChanged, // int starId
    PlayerShipMoved, // int start, int end, float progress

	ViewModeChanged, // ViewMode
	StarContentChanged,
    StarMapChanged,
	ArrivedToObject, // Galaxy.StarObjectType
    ArrivedToPlanet, // int planetId

    QuestListChanged,

    MoneyValueChanged, // int
    FuelValueChanged, // int
    StarsValueChanged, // int
    TokensValueChanged, // int
    SpecialResourcesChanged,

    SupplyShipActivated, // bool

    TechResearched,
    TechPointsChanged,
	CraftStarted,
	CraftFinished,
	GuardianDefeated,
	CommonResourcesChanged,
	UpgradeLevelChanged,

	ActiveShipsChanged,
    ShowShipControls, // IShipModel

    PlayerGainedExperience,
    PlayerSkillsChanged,

    IapItemsRefreshed,

    PlayerShipCountChanged, // int
    EnemyShipCountChanged, // int
	CombatShipCreated, // IShip
    CombatShipDestroyed, // IShip
    GamePaused, // bool
    PlayerShipDocked, // int
    PlayerShipUndocked, // int
    ObjectiveDestroyed, // int

    AdsManagerStatusChanged,
    RewardedVideoCompleted,
    SocialShareCompleted,

    Surrender,
    KillAllEnemies,
}
