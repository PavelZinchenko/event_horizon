using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        [SerializeField] public GameObject DescriptionHolder;
        [SerializeField] public Text DescriptionText;
        [SerializeField] public LayoutGroup StatsLayout;

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

        public override ComponentInfo Component => _component;

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
                Modification.gameObject.SetActive(
                    !string.IsNullOrEmpty(Modification.text = modification.GetDescription(_localization)));
                Modification.color = ColorTable.QualityColor(_component.ItemQuality);
            }

            var description = _component.Data.Description ?? "";
            var lines = Enumerable.Empty<(string, string)>();
            if (StatsLayout || DescriptionText)
            {
                CalculateStats(component, _localization, out lines, ref description);
            }

            if (DescriptionText)
            {
                (DescriptionHolder ? DescriptionHolder : DescriptionText.gameObject).SetActive(
                    !string.IsNullOrEmpty(description));
                DescriptionText.text = description;
            }

            if (StatsLayout)
            {
                StatsLayout.InitializeElements<TextFieldViewModel, (string, string)>(
                    lines, UpdateTextField);
            }
        }

        public static void CalculateStats(Constructor.Component.IComponent component, ILocalization localization,
            out IEnumerable<(string, string)> lines, ref string description)
        {
            description = description ?? "";
            // List of stats that may get replaced
            var stats = GetDescription(component, localization).ToList();
            var statsDict = stats.ToDictionary(e => e.Item1, e => e.Item2);
            AddExtraFields(component, localization, statsDict);
            if (description.EndsWith("|"))
            {
                description = description.Substring(0, description.Length - 1);
            }
            if (description.Contains('|'))
            {
                // Original stats values
                var items = description.Split('|').Select(e => e.Trim()).ToList();
                description = items[0];
                lines = items.Skip(1).Pairs().ToList();
                if (lines.Any(e => e.Item1 == "*" && e.Item2 == HideValue))
                {
                    stats.Clear();
                }

                var linesList = new List<(string, string)>();
                foreach (var (key, value) in lines)
                {
                    (string key, string value) pair = (key, value ?? "");
                    // If key is in format a>b then it means that it's a "renaming" key
                    var originalKey = key;
                    if (key.Contains('>'))
                    {
                        var split = key.Split('>').Select(e => e.Trim()).ToList();
                        originalKey = split[0];
                        pair.key = string.Join(">", split.Skip(1));
                    }

                    // Not very efficient, but we never have enough lines for it to be a concern
                    var index = stats.FindIndex(e => e.Item1 == originalKey);

                    if (index >= 0)
                    {
                        var oldPair = stats[index];
                        // For renamed fields one can use $1 to get reference to the original value
                        pair.value = FormatValue(localization.GetString(pair.value, oldPair.Item2), statsDict);
                        stats[index] = pair;
                        continue;
                    }

                    pair.value = FormatValue(localization.GetString(pair.value), statsDict);

                    pair.value = localization.GetString(pair.value);
                    linesList.Add(pair);
                }

                linesList.AddRange(stats);

                lines = linesList.Where(e => e.Item2 != HideValue);
            }
            else
            {
                lines = stats;
            }

            description = FormatValue(localization.GetString(description), statsDict);
        }

        private static void AddExtraFields(Constructor.Component.IComponent component, ILocalization localization,
            IDictionary<string, string> mappings)
        {
            const string keyLifetime = "$WeaponLifetime";
            const string keyHp = "$WeaponHP";
            string lifetime = null;
            string hp = null;
            if (component.Weapons.Any())
            {
                lifetime = component.Weapons.First().Ammunition.Body.Lifetime.ToString(_floatFormat);
                hp = component.Weapons.First().Ammunition.Body.HitPoints.ToString(_floatFormat);
            }
            else if (component.WeaponsObsolete.Any())
            {
                lifetime = component.WeaponsObsolete.First().Value.LifeTime.ToString(_floatFormat);
                hp = component.WeaponsObsolete.First().Value.HitPoints.ToString(_floatFormat);
            }

            if (!string.IsNullOrEmpty(lifetime)) mappings[keyLifetime] = lifetime;
            if (!string.IsNullOrEmpty(hp)) mappings[keyHp] = hp;
        }

        private static string FormatValue(string value, IDictionary<string, string> mappings)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return FormatterRegex.Replace(value, match =>
            {
                var key = LocalizationManager.SpecialChar + match.Groups[1].Value;
                if (!mappings.TryGetValue(key, out var result)) result = match.Value;
                return result;
            });
        }

        private void UpdateTextField(TextFieldViewModel viewModel, (string, string) data)
        {
            viewModel.Label.text = _localization.GetString(data.Item1);
            viewModel.Value.text = data.Item2;
        }

        private static IEnumerable<(string, string)> GetDescription(Constructor.Component.IComponent component,
            ILocalization localization)
        {
            var stats = new ShipEquipmentStats();
            component.UpdateStats(ref stats);

            if (stats.ArmorPoints != 0)
                yield return ("$HitPoints", FormatFloat(stats.ArmorPoints));
            if (!Mathf.Approximately(stats.ArmorRepairRate, 0))
                yield return ("$RepairRate", FormatFloat(stats.ArmorRepairRate));
            if (!Mathf.Approximately(stats.ArmorRepairCooldownMultiplier.Bonus, 0))
                yield return ("$RepairRateCooldown", FormatPercent(stats.ArmorRepairCooldownMultiplier.Bonus));

            if (!Mathf.Approximately(stats.EnergyPoints, 0))
                yield return ("$Energy", FormatFloat(stats.EnergyPoints));

            if (stats.EnergyRechargeRate < 0)
                yield return ("$EnergyConsumption", FormatFloat(-stats.EnergyRechargeRate));
            if (stats.EnergyRechargeRate > 0)
                yield return ("$RechargeRate", FormatFloat(stats.EnergyRechargeRate));

            if (!Mathf.Approximately(stats.EnergyRechargeCooldownMultiplier.Bonus, 0))
                yield return ("$RechargeRateCooldown", FormatPercent(stats.EnergyRechargeCooldownMultiplier.Bonus));

            if (!Mathf.Approximately(stats.ShieldPoints, 0))
                yield return ("$ShieldPoints", FormatFloat(stats.ShieldPoints));
            if (!Mathf.Approximately(stats.ShieldRechargeRate, 0))
                yield return ("$ShieldRechargeRate", FormatFloat(stats.ShieldRechargeRate));
            if (!Mathf.Approximately(stats.ShieldRechargeCooldownMultiplier.Bonus, 0))
                yield return ("$ShieldRechargeRateCooldown",
                    FormatPercent(stats.ShieldRechargeCooldownMultiplier.Bonus));

            if (stats.EnginePower != 0)
                yield return ("$Velocity", FormatFloat(stats.EnginePower));
            if (stats.TurnRate != 0)
                yield return ("$TurnRate", FormatFloat(stats.TurnRate));

            if (stats.WeaponDamageMultiplier.HasValue)
                yield return ("$DamageModifier", stats.WeaponDamageMultiplier.ToString());
            if (stats.WeaponFireRateMultiplier.HasValue)
                yield return ("$FireRateModifier", stats.WeaponFireRateMultiplier.ToString());
            if (stats.WeaponRangeMultiplier.HasValue)
                yield return ("$RangeModifier", stats.WeaponRangeMultiplier.ToString());
            if (stats.WeaponEnergyCostMultiplier.HasValue)
                yield return ("$EnergyModifier", stats.WeaponEnergyCostMultiplier.ToString());

            if (stats.RammingDamage != 0)
                yield return ("$RamDamage", FormatFloat(stats.RammingDamage));
            if (stats.EnergyAbsorption != 0)
                yield return ("$DamageAbsorption", FormatFloat(stats.EnergyAbsorption));

            if (stats.KineticResistance != 0)
                yield return ("$KineticDamageResistance", FormatFloat(stats.KineticResistance));
            if (stats.ThermalResistance != 0)
                yield return ("$ThermalDamageResistance", FormatFloat(stats.ThermalResistance));
            if (stats.EnergyResistance != 0)
                yield return ("$EnergyDamageResistance", FormatFloat(stats.EnergyResistance));

            if (stats.DroneDamageMultiplier.HasValue)
                yield return ("$DroneDamageModifier", stats.DroneDamageMultiplier.ToString());
            if (stats.DroneDefenseMultiplier.HasValue)
                yield return ("$DroneDefenseModifier", stats.DroneDefenseMultiplier.ToString());
            if (stats.DroneRangeMultiplier.HasValue)
                yield return ("$DroneRangeModifier", stats.DroneRangeMultiplier.ToString());
            if (stats.DroneSpeedMultiplier.HasValue)
                yield return ("$DroneSpeedModifier", stats.DroneSpeedMultiplier.ToString());
            if (stats.DroneReconstructionSpeed > 0)
                yield return ("$DroneReconstructionTime", (1f / stats.DroneReconstructionSpeed).ToString("N1"));
            if (stats.DroneReconstructionTimeMultiplier.HasValue)
                yield return ("$DroneReconstructionTime", stats.DroneReconstructionTimeMultiplier.ToString());

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
                yield return ("$Weight", Mathf.RoundToInt(stats.Weight).ToString());
        }

        public override bool Selected { get; set; }

        private static IEnumerable<(string, string)> GetWeaponDescription(
            KeyValuePair<WeaponStats, AmmunitionObsoleteStats> weapon, ILocalization localization)
        {
            yield return ("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));

            var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            if (weapon.Key.FireRate > 0)
            {
                yield return ("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
                yield return ("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
                yield return ("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            }
            else
            {
                yield return ("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
                yield return ("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            }

            if (weapon.Value.Range > 0)
                yield return ("$WeaponRange", weapon.Value.Range.ToString(_floatFormat));
            if (weapon.Value.Velocity > 0)
                yield return ("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
            if (weapon.Value.Impulse > 0)
                yield return ("$WeaponImpulse", (weapon.Value.Impulse * 1000).ToString(_floatFormat));
            if (weapon.Value.AreaOfEffect > 0)
                yield return ("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));
        }

        private static IEnumerable<(string, string)> GetWeaponDescription(Constructor.Component.WeaponData data,
            ILocalization localization)
        {
            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (data.Weapon.Stats.WeaponClass == )
            //{
            //    yield return ("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return ("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return ("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return ("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return ("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}

            var ecMult = data.StatModifier.EnergyCostMultiplier.Value;
            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
            {
                yield return ("$WeaponEPS", (ecMult * data.Ammunition.Body.EnergyCost).ToString(_floatFormat));
            }
            else
            {
                yield return ("$WeaponEnergy", (ecMult * data.Ammunition.Body.EnergyCost).ToString(_floatFormat));
                yield return ("$WeaponCooldown",
                    (1.0f / (data.Weapon.Stats.FireRate * data.StatModifier.FireRateMultiplier.Value)).ToString(
                        _floatFormat));
            }

            foreach (var item in GetWeaponDamageText(data.Ammunition, data.StatModifier, localization))
                yield return item;

            //yield return ("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));

            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (weapon.Key.FireRate > 0)
            //{
            //    yield return ("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return ("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return ("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return ("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return ("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}

            //if (data.Ammunition.Body.Range > 0)
            //    yield return ("$WeaponRange", data.Ammunition.Body.Range.ToString(_floatFormat));
            //if (data.Ammunition.Body.Velocity > 0)
            //    yield return ("$WeaponRange", data.Ammunition.Body.Velocity.ToString(_floatFormat));
            //if (weapon.Value.Velocity > 0)
            //    yield return ("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
            //if (weapon.Value.Impulse > 0)
            //    yield return ("$WeaponImpulse", (weapon.Value.Impulse * 1000).ToString(_floatFormat));
            //if (weapon.Value.AreaOfEffect > 0)
            //    yield return ("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));
        }

        private static IEnumerable<(string, string)> GetWeaponDamageText(Ammunition ammunition,
            WeaponStatModifier statModifier, ILocalization localization)
        {
            var effect = ammunition.Effects.FirstOrDefault(item =>
                item.Type == ImpactEffectType.Damage || item.Type == ImpactEffectType.SiphonHitPoints);
            if (effect != null && effect.Power > 0)
            {
                yield return ("$DamageType", localization.GetString(effect.DamageType.Name()));
                var damage = effect.Power * statModifier.DamageMultiplier.Value;
                yield return (ammunition.ImpactType == BulletImpactType.DamageOverTime ? "$WeaponDPS" : "$WeaponDamage",
                    damage.ToString(_floatFormat));
                yield break;
            }

            var trigger = ammunition.Triggers.OfType<BulletTrigger_SpawnBullet>().FirstOrDefault();
            if (trigger?.Ammunition != null)
                foreach (var item in GetWeaponDamageText(trigger.Ammunition, statModifier, localization))
                    yield return item;
        }

        private static IEnumerable<(string, string)> GetDroneBayDescription(
            KeyValuePair<DroneBayStats, ShipBuild> droneBay, ILocalization localization)
        {
            yield return ("$DroneBayCapacity", droneBay.Key.Capacity.ToString());
            if (!Mathf.Approximately(droneBay.Key.DamageMultiplier, 1f))
                yield return ("$DroneDamageModifier", FormatPercent(droneBay.Key.DamageMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.DefenseMultiplier, 1f))
                yield return ("$DroneDefenseModifier", FormatPercent(droneBay.Key.DefenseMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.SpeedMultiplier, 1f))
                yield return ("$DroneSpeedModifier", FormatPercent(droneBay.Key.SpeedMultiplier - 1f));

            yield return ("$DroneRangeModifier", droneBay.Key.Range.ToString("N"));

            var weapon = droneBay.Value.Components
                .Select(ComponentExtensions.FromDatabase)
                .FirstOrDefault(item => item.Info.Data.Weapon != null);
            if (weapon != null)
                yield return ("$WeaponType", localization.GetString(weapon.Info.Data.Name));
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
            return (value >= 0 ? "+" : "") + Mathf.RoundToInt(100 * value) + "%";
        }

        private ComponentInfo _component;
        private const string _floatFormat = "0.##";
        private const string HideValue = "@hide";
        private static readonly Regex FormatterRegex = new Regex(@"@(\S+)");
    }
}
