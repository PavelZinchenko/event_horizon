using UnityEngine;
using Economy.ItemType;
using GameDatabase.Enums;

public class ColorTable : MonoBehaviour 
{
	public ColorTable()
	{
		_instance = this;
	}

	public static Color ShipExplosionColor { get { return _instance._shipExplosionColor; } }
	public static Color ShipWarpColor { get { return _instance._shipWarpColor; } }

	public static Color DefaultUiColor { get { return _instance._defaultUiColor; } }
	public static Color DefaultTextColor { get { return _instance._defaultTextColor; } }

	public static Color CreditsColor { get { return _instance._creditsColor; } }
    public static Color TokensColor { get { return _instance._tokensColor; } }
    public static Color SnowflakesColor { get { return _instance._snowflakesColor; } }
    public static Color PremiumItemColor { get { return _instance._premiumItemColor; } }

    public static Color QualityColor(ItemQuality quality) { return _instance._qualityColors[(int)quality]; }

    [SerializeField] private Color _defaultUiColor;
	[SerializeField] private Color _defaultTextColor;
	[SerializeField] private Color _shipWarpColor;
	[SerializeField] private Color _shipExplosionColor;
	[SerializeField] private Color _creditsColor;
	[SerializeField] private Color _premiumItemColor;
    [SerializeField] private Color _tokensColor;
    [SerializeField] private Color _snowflakesColor;
    [SerializeField] private Color[] _qualityColors;

    private static ColorTable _instance;
}
