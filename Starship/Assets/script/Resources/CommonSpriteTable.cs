using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommonSpriteTable : MonoBehaviour 
{
	public CommonSpriteTable()
	{
        if (!_instance)
		    _instance = this;
	}
	
	public static Sprite CreditsIcon { get { return _instance._creditsIcon; } }
    public static Sprite SnowflakesIcon { get { return _instance._snowflakesIcon; } }
	public static Sprite FuelIcon { get { return _instance._fuelIcon; } }
	public static Sprite ShopIcon { get { return _instance._shopIcon; } }	
	public static Sprite StarMap { get { return _instance._starMap; } }
	public static Sprite FactionStarMap { get { return _instance._factionStarMap; } }
	public static Sprite Blueprint { get { return _instance._blueprint; } }
	public static Sprite TechIcon { get { return _instance._techIcon; } }
    public static Sprite RewardedAd { get { return _instance._rewardedAdIcon; } }
    public static Sprite RewardedFacebookPost { get { return _instance._rewardedFacebookPostIcon; } }

    public static Sprite FactionFaceIcon { get { return _instance._factionFaceIcon; } }
	public static Sprite StarCurrencyIcon { get { return _instance._starResource; } }
    public static Sprite TokenCurrencyIcon { get { return _instance._tokenResource; } }

    public static Sprite AsteroidBelt { get { return _instance._asteroidBelt; } }
	public static Sprite GasPlanet(System.Random random) { return _instance._gasPlanets[random.Next(_instance._gasPlanets.Length)]; }
	public static Sprite BarrenPlanet(System.Random random) { return _instance._barrenPlanets[random.Next(_instance._barrenPlanets.Length)]; }
	public static Sprite TerranPlanet(System.Random random) { return _instance._terranPlanets[random.Next(_instance._terranPlanets.Length)]; }
    public static Sprite InfectedPlanet => _instance._infectedPlanet;

    public static Sprite SkillIcon(GameModel.Skills.SkillType type)
    {
        if (_instance._skillSprites == null)
            _instance._skillSprites = _instance._skills.ToDictionary(item => item.Type, item => item.Icon);

        Sprite sprite;
        return _instance._skillSprites.TryGetValue(type, out sprite) ? sprite : null;
    }

    public static Sprite StarIcon { get { return _instance._starIcon; } }

	public static Sprite AttackUpgrade { get { return _instance._attackUpgrade; } }
	public static Sprite DefenseUpgrade { get { return _instance._defenseUpgrade; } }
	public static Sprite ExperienceUpgrade { get { return _instance._experienceUpgrade; } }
	public static Sprite FuelTankUpgrade { get { return _instance._fuelTankUpgrade; } }
	public static Sprite RadarRangeUpgrade { get { return _instance._radarRangeUpgrade; } }
	public static Sprite SpeedUpgrade { get { return _instance._speedUpgrade; } }
	public static Sprite EnergyUpgrade { get { return _instance._energyUpgrade; } }
	public static Sprite MissileWeapon { get { return _instance._missileWeapon; } }

	[SerializeField] Sprite _creditsIcon;
	[SerializeField] Sprite _fuelIcon;
	[SerializeField] Sprite _shopIcon;
    [SerializeField] Sprite _snowflakesIcon;

    [SerializeField] Sprite _starMap;
	[SerializeField] Sprite _factionStarMap;
	[SerializeField] Sprite _blueprint;
    [SerializeField] Sprite _starIcon;
    [SerializeField] Sprite _techIcon;
    [SerializeField] Sprite _rewardedAdIcon;
    [SerializeField] Sprite _rewardedFacebookPostIcon;

    [SerializeField] Sprite _factionFaceIcon;
	[SerializeField] Sprite _starResource;
    [SerializeField] Sprite _tokenResource;

    [SerializeField] Sprite _asteroidBelt;
    [SerializeField] Sprite _infectedPlanet;
	[SerializeField] Sprite[] _gasPlanets;
	[SerializeField] Sprite[] _barrenPlanets;
	[SerializeField] Sprite[] _terranPlanets;

	[SerializeField] Sprite _attackUpgrade;
	[SerializeField] Sprite _defenseUpgrade;
	[SerializeField] Sprite _experienceUpgrade;
	[SerializeField] Sprite _fuelTankUpgrade;
	[SerializeField] Sprite _radarRangeUpgrade;
	[SerializeField] Sprite _speedUpgrade;
	[SerializeField] Sprite _energyUpgrade;
	[SerializeField] Sprite _missileWeapon;
    [SerializeField] SkillData[] _skills;

    private void OnValidate()
    {
        _skillSprites = null;
    }

	private static CommonSpriteTable _instance;
    private Dictionary<GameModel.Skills.SkillType, Sprite> _skillSprites;
    
    [Serializable]
    private struct SkillData
    {
        public GameModel.Skills.SkillType Type;
        public Sprite Icon;
    }
}
