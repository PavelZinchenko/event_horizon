using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Constructor;
using Utils;

namespace ViewModel
{
	public class StatsPanelViewModel : MonoBehaviour
	{
		public TextFieldViewModel ArmorPoints;
		public TextFieldViewModel RepairRate;
		public TextFieldViewModel Energy;
		public TextFieldViewModel RechargeRate;
        public TextFieldViewModel Shield;
        public TextFieldViewModel ShieldRechargeRate;
        public TextFieldViewModel Weight;
		public TextFieldViewModel RamDamage;
		public TextFieldViewModel DamageAbsorption;
		public TextFieldViewModel Velocity;
		public TextFieldViewModel TurnRate;
		public TextFieldViewModel WeaponDamage;
		public TextFieldViewModel WeaponFireRate;
		public TextFieldViewModel WeaponRange;
		public TextFieldViewModel WeaponEnergyConsumption;
		public TextFieldViewModel DroneDamageModifier;
	    public TextFieldViewModel DroneDefenseModifier;
        public TextFieldViewModel DroneRangeModifier;
		public TextFieldViewModel DroneSpeedModifier;
		public TextFieldViewModel DroneTimeModifier;
		public TextFieldViewModel EnergyDamageResistance;
		public TextFieldViewModel KineticDamageResistance;
		public TextFieldViewModel HeatDamageResistance;
		public GameObject WeaponsBlock;
		public GameObject DronesBlock;
		public GameObject ResistanceBlock;

		public Color NormalColor;
		public Color ErrorColor;

		public Text HitPointsSummaryText;
		public Text EnergySummaryText;
		public Text VelocitySummaryText;

		public CanvasGroup CanvasGroup;

		public void OnMoreInfoButtonClicked(bool isOn)
		{
			CanvasGroup.alpha = isOn ? 1 : 0;
		}

        public void UpdateStats(Constructor.IShipSpecification spec)
		{
			HitPointsSummaryText.text = Mathd.ToInGameString(spec.Stats.ArmorPoints);
            HitPointsSummaryText.color = spec.Stats.ArmorPoints > 0 ? NormalColor : ErrorColor;

            EnergySummaryText.text = Mathd.ToInGameString(spec.Stats.EnergyPoints) + " [" +  Mathd.ToSignedInGameString(spec.Stats.EnergyRechargeRate) + "]";
			EnergySummaryText.color = spec.Stats.EnergyRechargeRate > 0 ? NormalColor : ErrorColor;
			VelocitySummaryText.text = spec.Stats.EnginePower.ToString("N1") + " / " + spec.Stats.TurnRate.ToString("N1");
			VelocitySummaryText.color = spec.Stats.EnginePower > 0 ? NormalColor : ErrorColor;

            ArmorPoints.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ArmorPoints, 0));
            ArmorPoints.Value.text = spec.Stats.ArmorPoints.ToString("N1");
            ArmorPoints.Color = spec.Stats.ArmorPoints > 0 ? NormalColor : ErrorColor;

            RepairRate.gameObject.SetActive(spec.Stats.ArmorRepairRate > 0);
			RepairRate.Value.text = spec.Stats.ArmorRepairRate.ToString("N1");

            Shield.gameObject.SetActive(spec.Stats.ShieldPoints > 0);
            Shield.Value.text = spec.Stats.ShieldPoints.ToString("N1");
            ShieldRechargeRate.gameObject.SetActive(spec.Stats.ShieldPoints > 0);
            ShieldRechargeRate.Value.text = spec.Stats.ShieldRechargeRate.ToString("N");

            Energy.Value.text = spec.Stats.EnergyPoints.ToString("N");
			Weight.Value.text = Mathf.RoundToInt(spec.Stats.Weight*1000).ToString();
			RechargeRate.Value.text = spec.Stats.EnergyRechargeRate.ToString("N");
			RechargeRate.Color = spec.Stats.EnergyRechargeRate > 0 ? NormalColor : ErrorColor;
			Velocity.Color = spec.Stats.EnginePower > 0 ? NormalColor : ErrorColor;
			Velocity.Value.text = spec.Stats.EnginePower.ToString("N");
			TurnRate.Color = spec.Stats.TurnRate > 0 ? NormalColor : ErrorColor;
			TurnRate.Value.text = spec.Stats.TurnRate.ToString("N");

			WeaponDamage.gameObject.SetActive(spec.Stats.WeaponDamageMultiplier.HasValue);
			WeaponDamage.Value.text = spec.Stats.WeaponDamageMultiplier.ToString();
			WeaponFireRate.gameObject.SetActive(spec.Stats.WeaponFireRateMultiplier.HasValue);
			WeaponFireRate.Value.text = spec.Stats.WeaponFireRateMultiplier.ToString();
			WeaponRange.gameObject.SetActive(spec.Stats.WeaponRangeMultiplier.HasValue);
			WeaponRange.Value.text = spec.Stats.WeaponRangeMultiplier.ToString();
			WeaponEnergyConsumption.gameObject.SetActive(spec.Stats.WeaponEnergyCostMultiplier.HasValue);
			WeaponEnergyConsumption.Value.text = spec.Stats.WeaponEnergyCostMultiplier.ToString();
			WeaponsBlock.gameObject.SetActive(WeaponsBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);

			DroneDamageModifier.gameObject.SetActive(spec.Stats.DroneDamageMultiplier.HasValue);
			DroneDamageModifier.Value.text = spec.Stats.DroneDamageMultiplier.ToString();
            DroneDefenseModifier.gameObject.SetActive(spec.Stats.DroneDefenseMultiplier.HasValue);
            DroneDefenseModifier.Value.text = spec.Stats.DroneDefenseMultiplier.ToString();
            DroneRangeModifier.gameObject.SetActive(spec.Stats.DroneRangeMultiplier.HasValue);
			DroneRangeModifier.Value.text = spec.Stats.DroneRangeMultiplier.ToString();
			DroneSpeedModifier.gameObject.SetActive(spec.Stats.DroneSpeedMultiplier.HasValue);
			DroneSpeedModifier.Value.text = spec.Stats.DroneSpeedMultiplier.ToString();
			DroneTimeModifier.gameObject.SetActive(spec.Stats.DroneBuildSpeed > 0);
			DroneTimeModifier.Value.text = spec.Stats.DroneBuildTime.ToString("N1");
			DronesBlock.gameObject.SetActive(DronesBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);

			RamDamage.gameObject.SetActive(!Mathf.Approximately(spec.Stats.RammingDamage, 0));
			RamDamage.Value.text = FormatPercent(spec.Stats.RammingDamageMultiplier);
			DamageAbsorption.gameObject.SetActive(spec.Stats.EnergyAbsorption > 0);
			DamageAbsorption.Value.text = Mathf.RoundToInt(spec.Stats.EnergyAbsorptionPercentage*100) + "%";

			KineticDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.KineticResistance, 0));
			KineticDamageResistance.Value.text = FloatToString(spec.Stats.KineticResistance) + " ( " + Mathf.RoundToInt(spec.Stats.KineticResistancePercentage*100) + "% )";
			HeatDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ThermalResistance, 0));
			HeatDamageResistance.Value.text = FloatToString(spec.Stats.ThermalResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ThermalResistancePercentage*100) + "% )";
			EnergyDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyResistance, 0));
			EnergyDamageResistance.Value.text = FloatToString(spec.Stats.EnergyResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyResistancePercentage*100) + "% )";
			ResistanceBlock.gameObject.SetActive(ResistanceBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);
		}

		private string FormatPercent(float value)
		{
			if (value >= 1)
				return "+" + Mathf.RoundToInt(100*(value - 1)) + "%";
			else
				return "-" + Mathf.RoundToInt(100*(1 - value)) + "%";
		}

		private static string RoundToInt(float value, bool showSign = false)
		{
			var intValue = Mathf.RoundToInt(value);
			if (showSign && intValue >= 0)
				return "+" + intValue;
			else
				return intValue.ToString();
		}

		private static string FloatToString(float value, bool showSign = false)
		{
			if (showSign && value >= 0)
				return "+" + value.ToString("N");
			else
				return value.ToString("N");
		}
    }
}
