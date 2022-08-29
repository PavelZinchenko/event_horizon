using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constructor;
using Constructor.Model;
using Constructor.Modification;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Player;
using Gui.ComponentList;
using Model;
using Services.Localization;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class ComponentViewModel : ComponentListItemBase
	{
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IResourceLocator _resourceLocator;
	    [Inject] private readonly PlayerResources _playerResources;

        [SerializeField] public ConstructorViewModel ConstructorViewModel;
        [SerializeField] public Button Button;
        [SerializeField] public Image Icon;
        [SerializeField] public Text Name;
        [SerializeField] public Text Modification;
        [SerializeField] public Text Count;
        [SerializeField] public Sprite EmptyIcon;
        [SerializeField] public LayoutGroup DescriptionLayout;

        public override void Initialize(ComponentInfo data, int count)
		{
		    if (Count != null)
		    {
                Count.gameObject.SetActive(count > 0);
		        Count.text = count.ToString();
		    }

            if (_component == data)
				return;

			_component = data;
            var model = _component.CreateComponent(ConstructorViewModel.ShipSize);

            if (Button)
                Button.interactable = model.IsSuitable(ConstructorViewModel.Ship.Model);

			UpdateDescription(model);
		}

		public void Clear()
		{
			Icon.sprite = EmptyIcon;
			Icon.color = Color.white;
			Name.text = "-";
			_component = new ComponentInfo();
		}

		public void OnClicked()
		{
			ConstructorViewModel.ShowComponent(_component);
		}

        public override ComponentInfo Component { get { return _component; } }

		private void UpdateDescription(Constructor.Component.IComponent component)
		{
		    if (Name != null)
		    {
				Name.text = _component.GetName(_localization);
		        Name.color = ColorTable.QualityColor(_component.ItemQuality);
		    }
		    if (Icon != null)
			{
				Icon.sprite = _resourceLocator.GetSprite(_component.Data.Icon);
				Icon.color = _component.Data.Color;
			}

		    if (Modification != null)
		    {
		        var modification = component.Modification ?? EmptyModification.Instance;
		        Modification.gameObject.SetActive(!string.IsNullOrEmpty(Modification.text = modification.GetDescription(_localization)));
		        Modification.color = ColorTable.QualityColor(_component.ItemQuality);
		    }

            if (DescriptionLayout)
		        DescriptionLayout.InitializeElements<TextFieldViewModel, KeyValuePair<string,string>>(
				    GetDescription(component, _localization), UpdateTextField);
		}

		private void UpdateTextField(TextFieldViewModel viewModel, KeyValuePair<string, string> data)
		{
			viewModel.Label.text = _localization.GetString(data.Key);
			viewModel.Value.text = data.Value;
		}

		public static IEnumerable<KeyValuePair<string, string>> GetDescription(Constructor.Component.IComponent component, ILocalization localization)
		{
			var stats = new ShipEquipmentStats();
			component.UpdateStats(ref stats);

			if (stats.ArmorPoints != 0)
				yield return new KeyValuePair<string, string>("$HitPoints", FormatFloat(stats.ArmorPoints));

            if (!Mathf.Approximately(stats.EnergyPoints, 0))
				yield return new KeyValuePair<string, string>("$Energy", FormatFloat(stats.EnergyPoints));

			if (stats.EnergyRechargeRate < 0)
				yield return new KeyValuePair<string, string>("$EnergyConsumption", FormatFloat(-stats.EnergyRechargeRate));
			if (stats.EnergyRechargeRate > 0)
				yield return new KeyValuePair<string, string>("$RechargeRate", FormatFloat(stats.EnergyRechargeRate));

		    if (!Mathf.Approximately(stats.ShieldPoints, 0))
		        yield return new KeyValuePair<string, string>("$ShieldPoints", FormatFloat(stats.ShieldPoints));
            if (!Mathf.Approximately(stats.ShieldRechargeRate, 0))
		        yield return new KeyValuePair<string, string>("$ShieldRechargeRate", FormatFloat(stats.ShieldRechargeRate));

		    if (stats.EnginePower != 0)
				yield return new KeyValuePair<string, string>("$Velocity", FormatFloat(stats.EnginePower));
			if (stats.TurnRate != 0)
				yield return new KeyValuePair<string, string>("$TurnRate", FormatFloat(stats.TurnRate));

			if (stats.WeaponDamageMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DamageModifier", stats.WeaponDamageMultiplier.ToString());
			if (stats.WeaponFireRateMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$FireRateModifier", stats.WeaponFireRateMultiplier.ToString());
			if (stats.WeaponRangeMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$RangeModifier", stats.WeaponRangeMultiplier.ToString());
			if (stats.WeaponEnergyCostMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$EnergyModifier", stats.WeaponEnergyCostMultiplier.ToString());

			if (stats.RammingDamage != 0)
				yield return new KeyValuePair<string, string>("$RamDamage", FormatFloat(stats.RammingDamage));
			if (stats.EnergyAbsorption != 0)
				yield return new KeyValuePair<string, string>("$DamageAbsorption", FormatFloat(stats.EnergyAbsorption));

			if (stats.KineticResistance != 0)
				yield return new KeyValuePair<string, string>("$KineticDamageResistance", FormatFloat(stats.KineticResistance));
			if (stats.ThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$ThermalDamageResistance", FormatFloat(stats.ThermalResistance));
			if (stats.EnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyDamageResistance", FormatFloat(stats.EnergyResistance));

			if (stats.DroneDamageMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneDamageModifier", stats.DroneDamageMultiplier.ToString());
            if (stats.DroneDefenseMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$DroneDefenseModifier", stats.DroneDefenseMultiplier.ToString());
            if (stats.DroneRangeMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneRangeModifier", stats.DroneRangeMultiplier.ToString());
			if (stats.DroneSpeedMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneSpeedModifier", stats.DroneSpeedMultiplier.ToString());
			if (stats.DroneReconstructionSpeed > 0)
				yield return new KeyValuePair<string, string>("$DroneReconstructionTime", (1f/stats.DroneReconstructionSpeed).ToString("N1"));
            if (stats.DroneReconstructionTimeMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$DroneReconstructionTime", stats.DroneReconstructionTimeMultiplier.ToString());

            // TODO: display component type
            //DeviceInfo.SetActive(component.Devices.Any());
            //DroneBayInfo.SetActive(component.DroneBays.Any());

            if (component.Weapons.Any())
                foreach (var item in GetWeaponDescription(component.Weapons.First(), localization))
                    yield return item;

            if (component.WeaponsObsolete.Any())
				foreach (var item in GetWeaponDescription(component.WeaponsObsolete.First(), localization))
					yield return item;

            if (component.DroneBays.Any())
                foreach (var item in GetDroneBayDescription(component.DroneBays.First(), localization))
                    yield return item;

            if (!Mathf.Approximately(stats.Weight, 0))
				yield return new KeyValuePair<string, string>("$Weight", Mathf.RoundToInt(stats.Weight).ToString());
		}

        public override bool Selected { get; set; }

		private static IEnumerable<KeyValuePair<string, string>> GetWeaponDescription(KeyValuePair<WeaponStats,AmmunitionObsoleteStats> weapon, ILocalization localization)
		{
			yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));

			var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
			if (weapon.Key.FireRate > 0)
			{
				yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
				yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
				yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f/weapon.Key.FireRate).ToString(_floatFormat));
			}
			else
			{
				yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
				yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
			}

			if (weapon.Value.Range > 0)
				yield return new KeyValuePair<string, string>("$WeaponRange", weapon.Value.Range.ToString(_floatFormat));
			if (weapon.Value.Velocity > 0)
				yield return new KeyValuePair<string, string>("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
			if (weapon.Value.Impulse > 0)
				yield return new KeyValuePair<string, string>("$WeaponImpulse", (weapon.Value.Impulse*1000).ToString(_floatFormat));
			if (weapon.Value.AreaOfEffect > 0)
				yield return new KeyValuePair<string, string>("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));
		}

	    private static IEnumerable<KeyValuePair<string, string>> GetWeaponDescription(Constructor.Component.WeaponData data, ILocalization localization)
	    {
            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (data.Weapon.Stats.WeaponClass == )
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}

	        if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
	        {
	            yield return new KeyValuePair<string, string>("$WeaponEPS", data.Ammunition.Body.EnergyCost.ToString(_floatFormat));
	        }
	        else
	        {
	            yield return new KeyValuePair<string, string>("$WeaponEnergy", data.Ammunition.Body.EnergyCost.ToString(_floatFormat));
	            yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / (data.Weapon.Stats.FireRate * data.StatModifier.FireRateMultiplier.Value)).ToString(_floatFormat));
            }

	        foreach (var item in GetWeaponDamageText(data.Ammunition, data.StatModifier, localization))
	            yield return item;

            //yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));

            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (weapon.Key.FireRate > 0)
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}

	        //if (data.Ammunition.Body.Range > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponRange", data.Ammunition.Body.Range.ToString(_floatFormat));
	        //if (data.Ammunition.Body.Velocity > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponRange", data.Ammunition.Body.Velocity.ToString(_floatFormat));
	        //if (weapon.Value.Velocity > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
	        //if (weapon.Value.Impulse > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponImpulse", (weapon.Value.Impulse * 1000).ToString(_floatFormat));
	        //if (weapon.Value.AreaOfEffect > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));
	    }

	    private static IEnumerable<KeyValuePair<string, string>> GetWeaponDamageText(Ammunition ammunition, WeaponStatModifier statModifier, ILocalization localization)
	    {
		    var effect = ammunition.Effects.FirstOrDefault(item =>
			    item.Type == ImpactEffectType.Damage || item.Type == ImpactEffectType.SiphonHitPoints);
		    if (effect != null && effect.Power > 0)
		    {
	            yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(effect.DamageType.Name()));
	            var damage = effect.Power * statModifier.DamageMultiplier.Value;
                yield return new KeyValuePair<string, string>(ammunition.ImpactType == BulletImpactType.DamageOverTime ? "$WeaponDPS" : "$WeaponDamage",  damage.ToString(_floatFormat));
	            yield break;
	        }

	        var trigger = ammunition.Triggers.OfType<BulletTrigger_SpawnBullet>().FirstOrDefault();
            if (trigger?.Ammunition != null)
                foreach (var item in GetWeaponDamageText(trigger.Ammunition, statModifier, localization))
                    yield return item;
	    }

        private static IEnumerable<KeyValuePair<string, string>> GetDroneBayDescription(KeyValuePair<DroneBayStats,ShipBuild> droneBay, ILocalization localization)
        {
            yield return new KeyValuePair<string, string>("$DroneBayCapacity", droneBay.Key.Capacity.ToString());
            if (!Mathf.Approximately(droneBay.Key.DamageMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneDamageModifier", FormatPercent(droneBay.Key.DamageMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.DefenseMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneDefenseModifier", FormatPercent(droneBay.Key.DefenseMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.SpeedMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneSpeedModifier", FormatPercent(droneBay.Key.SpeedMultiplier - 1f));

            yield return new KeyValuePair<string, string>("$DroneRangeModifier", droneBay.Key.Range.ToString("N"));

            var weapon = droneBay.Value.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase).FirstOrDefault(item => item.Info.Data.Weapon != null);
            if (weapon != null)
                yield return new KeyValuePair<string, string>("$WeaponType", localization.GetString(weapon.Info.Data.Name));
        }

        private static string FormatInt(int value)
		{
			return (value >= 0 ? "+" : "") + value;
		}

		private static string FormatFloat(float value)
		{
			return (value >= 0 ? "+" : "") + value.ToString(_floatFormat);
		}

		private static string FormatPercent(float value)
		{
			return (value >= 0 ? "+" : "") + Mathf.RoundToInt(100*value) + "%";
		}

		private ComponentInfo _component;
	    private const string _floatFormat = "0.##";
	}
}
