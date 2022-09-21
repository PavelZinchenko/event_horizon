using System;
using System.Collections.Generic;
using Constructor;
using GameDatabase.Model;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace ViewModel
{
    public class StatsPanelViewModel : MonoBehaviour
    {
        public Color NormalColor;
        public Color ErrorColor;

        public Text HitPointsSummaryText;
        public Text EnergySummaryText;
        public Text VelocitySummaryText;

        public CanvasGroup CanvasGroup;
        public LayoutGroup TargetLayout;

        public int DownscaleLimit = 19;
        public int LabelFontSize = 22;
        public int ValueFontSize = 24;
        public int DownscaledLabelFontSize = 18;
        public int DownscaledValueFontSize = 20;

        private IList<Func<IShipSpecification, int>> _updaters = null;
        private IList<Text> _labels = new List<Text>();
        private IList<Text> _values = new List<Text>();
        private int _oldCount = -1;

        private static readonly Func<float, IShipSpecification, bool> IsPositive = (f, _) => f > 0;
        private static readonly Func<float, IShipSpecification, bool> IsNonNegative = (f, _) => f >= 0;
        private static readonly Func<float, IShipSpecification, bool> IsNonZero = (f, _) => !Mathf.Approximately(f, 0);
        private static readonly Func<float, IShipSpecification, bool> Always = (f, _) => true;

        public void OnMoreInfoButtonClicked(bool isOn)
        {
            CanvasGroup.alpha = isOn ? 1 : 0;
        }

        private IEnumerable<BlockData> GetStats()
        {
            // HP
            yield return Block(
                Line("$HitPoints", spec => spec.Stats.ArmorPoints, Always, IsPositive),
                Line("$RepairRate", spec => spec.Stats.ArmorRepairRate),
                Line("$RepairRateCooldown", spec => spec.Stats.ArmorRepairCooldown,
                    (_, specs) => !Mathf.Approximately(specs.Stats.ArmorRepairRate, 0))
            );

            // Shield
            yield return Block(
                spec => spec.Stats.ShieldPoints > 0,
                true,
                Line("$ShieldPoints", spec => spec.Stats.ShieldPoints),
                Line("$ShieldRechargeRate", spec => spec.Stats.ShieldRechargeRate),
                Line("$ShieldRechargeRateCooldown", spec => spec.Stats.ShieldRechargeCooldown,
                    (_, specs) => !Mathf.Approximately(specs.Stats.ShieldRechargeRate, 0))
            );

            // Resistances
            yield return Block(
                ResistancesLine("$KineticDamageResistance",
                    spec => (spec.Stats.KineticResistance, spec.Stats.KineticResistancePercentage)),
                ResistancesLine("$ThermalDamageResistance",
                    spec => (spec.Stats.ThermalResistance, spec.Stats.ThermalResistancePercentage)),
                ResistancesLine("$EnergyDamageResistance",
                    spec => (spec.Stats.EnergyResistance, spec.Stats.EnergyResistancePercentage))
            );

            // Energy
            yield return Block(
                Line("$Energy", spec => spec.Stats.EnergyPoints, Always, IsPositive),
                Line("$RechargeRate", spec => spec.Stats.EnergyRechargeRate, Always, IsPositive),
                Line("$RechargeRateCooldown", spec => spec.Stats.EnergyRechargeCooldown, Always)
            );

            // Engine
            yield return Block(
                Line("$Velocity", spec => spec.Stats.EnginePower, Always, IsPositive, (f, _) => f.ToString("N")),
                Line("$TurnRate", spec => spec.Stats.TurnRate, Always, IsPositive, (f, _) => f.ToString("N"))
            );

            // Weapon boosts
            yield return Block(
                BonusLine("$DamageModifier", spec => spec.Stats.WeaponDamageMultiplier),
                BonusLine("$FireRateModifier", spec => spec.Stats.WeaponFireRateMultiplier),
                BonusLine("$RangeModifier", spec => spec.Stats.WeaponRangeMultiplier),
                BonusLine("$EnergyModifier", spec => spec.Stats.WeaponEnergyCostMultiplier),
                Line("$RamDamage", spec => spec.Stats.RammingDamageMultiplier, format: (f, _) => FormatPercent(f))
            );

            // Drone boosts
            yield return Block(
                BonusLine("$DroneDamageModifier", spec => spec.Stats.DroneDamageMultiplier),
                BonusLine("$DroneDefenseModifier", spec => spec.Stats.DroneDefenseMultiplier),
                BonusLine("$DroneRangeModifier", spec => spec.Stats.DroneRangeMultiplier),
                BonusLine("$DroneSpeedModifier", spec => spec.Stats.DroneSpeedMultiplier),
                Line("$DroneReconstructionTime", spec => spec.Stats.DroneBuildTime)
            );

            // Misc
            yield return Block(
                _ => true,
                false,
                Line("$Weight", spec => spec.Stats.Weight, Always,
                    format: (f, _) => Mathf.RoundToInt(f * 1000).ToString()),
                Line("$DamageAbsorption", spec => spec.Stats.EnergyAbsorption,
                    format: (_, spec) => Mathf.RoundToInt(spec.Stats.EnergyAbsorptionPercentage * 100) + "%")
            );
        }

        private LineData<(float, float)> ResistancesLine(string name,
            Func<IShipSpecification, (float, float)> valueFunc)
        {
            return new LineData<(float, float)>(
                name,
                valueFunc,
                (a, _) => a.Item1 > 0, (a, _) => NormalColor,
                (res, _) => $"{FloatToString(res.Item1)} ( {Mathf.RoundToInt(res.Item2 * 100)}% )"
            );
        }

        private LineData<StatMultiplier> BonusLine(string name,
            Func<IShipSpecification, StatMultiplier> valueFunc)
        {
            return new LineData<StatMultiplier>(name, valueFunc, (m, _) => m.HasValue, (m, _) => NormalColor,
                (m, _) => m.ToString());
        }

        private LineData<float> Line(string name, Func<IShipSpecification, float> value,
            Func<float, IShipSpecification, bool> visibility = null,
            Func<float, IShipSpecification, bool> validity = null,
            Func<float, IShipSpecification, string> format = null)
        {
            visibility = visibility ?? IsNonZero;
            validity = validity ?? Always;
            format = format ?? ((f, _) => f.ToString("N1"));
            Color Color(float f, IShipSpecification specs) => validity(f, specs) ? NormalColor : ErrorColor;
            return new LineData<float>(name, value, visibility, Color, format);
        }

        private BlockData Block(params ILineData[] content) => Block(_ => true, true, content);

        private BlockData Block(Func<IShipSpecification, bool> visible, bool separator, params ILineData[] content)
        {
            return new BlockData(content, visible, separator);
        }

        public void UpdateStats(IShipSpecification spec)
        {
            HitPointsSummaryText.text = Mathd.ToInGameString(spec.Stats.ArmorPoints);
            HitPointsSummaryText.color = spec.Stats.ArmorPoints > 0 ? NormalColor : ErrorColor;

            EnergySummaryText.text = Mathd.ToInGameString(spec.Stats.EnergyPoints) + " [" +
                                     Mathd.ToSignedInGameString(spec.Stats.EnergyRechargeRate) + "]";
            EnergySummaryText.color = spec.Stats.EnergyRechargeRate > 0 ? NormalColor : ErrorColor;
            VelocitySummaryText.text =
                spec.Stats.EnginePower.ToString("N1") + " / " + spec.Stats.TurnRate.ToString("N1");
            VelocitySummaryText.color = spec.Stats.EnginePower > 0 ? NormalColor : ErrorColor;

            if (_updaters == null)
            {
                _updaters = new List<Func<IShipSpecification, int>>();
                _labels.Clear();
                _values.Clear();
                TargetLayout.InitializeElements<StatsBlockViewModel, BlockData>(GetStats(), InitializeBlock);
            }

            var count = 0;
            foreach (var updater in _updaters)
            {
                count += updater(spec);
            }

            if (count != _oldCount)
            {
                _oldCount = count;
                var labelSize = count > DownscaleLimit ? DownscaledLabelFontSize : LabelFontSize;
                var valueSize = count > DownscaleLimit ? DownscaledValueFontSize : ValueFontSize;
                foreach (var label in _labels)
                {
                    label.fontSize = labelSize;
                }

                foreach (var value in _values)
                {
                    value.fontSize = valueSize;
                }
            }
        }

        private void InitializeBlock(StatsBlockViewModel view, BlockData data)
        {
            var lines = new List<BoundLineData>();
            view.GetComponent<LayoutGroup>()
                .InitializeElements<TextFieldViewModel, ILineData>(data.Content, (text, line) =>
                {
                    var bound = new BoundLineData(line, text);
                    lines.Add(bound);
                    _labels.Add(text.Label);
                    _values.Add(text.Value);
                });

            view.SeparatorHeight = data.SeparatorVisible ? 10 : 0;

            _updaters.Add(specs =>
            {
                if (!data.Visible(specs))
                {
                    view.gameObject.SetActive(false);
                    return 0;
                }

                var visible = 0;
                foreach (var line in lines)
                {
                    if (line.Update(specs)) visible++;
                }

                view.gameObject.SetActive(visible > 0);
                return visible;
            });
        }

        private string FormatPercent(float value)
        {
            if (value >= 1)
                return "+" + Mathf.RoundToInt(100 * (value - 1)) + "%";
            return "-" + Mathf.RoundToInt(100 * (1 - value)) + "%";
        }

        private static string RoundToInt(float value, bool showSign = false)
        {
            var intValue = Mathf.RoundToInt(value);
            if (showSign && intValue >= 0)
                return "+" + intValue;
            return intValue.ToString();
        }

        private static string FloatToString(float value, bool showSign = false)
        {
            if (showSign && value >= 0)
                return "+" + value.ToString("N");
            return value.ToString("N");
        }

        private interface ILineData
        {
            string GetName();
            string GetValue(IShipSpecification specs);
            Color GetColor(IShipSpecification specs);
            bool Visible(IShipSpecification specs);
        }

        private readonly struct LineData<T> : ILineData
        {
            public LineData(string name, Func<IShipSpecification, T> value,
                Func<T, IShipSpecification, bool> visibility,
                Func<T, IShipSpecification, Color> color,
                Func<T, IShipSpecification, string> format)
            {
                Name = name;
                Value = value;
                Color = color;
                Visibility = visibility;
                Format = format;
            }

            public readonly string Name;
            public readonly Func<IShipSpecification, T> Value;
            public readonly Func<T, IShipSpecification, Color> Color;
            public readonly Func<T, IShipSpecification, string> Format;
            public readonly Func<T, IShipSpecification, bool> Visibility;

            public string GetName()
            {
                return Name;
            }

            public string GetValue(IShipSpecification specs)
            {
                return Format(Value(specs), specs);
            }

            public Color GetColor(IShipSpecification specs)
            {
                return Color(Value(specs), specs);
            }

            public bool Visible(IShipSpecification specs)
            {
                return Visibility(Value(specs), specs);
            }
        }

        private readonly struct BoundLineData
        {
            public BoundLineData(ILineData data, TextFieldViewModel view)
            {
                Data = data;
                View = view;
                View.Label.text = Data.GetName();
            }

            public readonly ILineData Data;
            public readonly TextFieldViewModel View;

            /// <summary>
            /// Updates data of the bound line
            /// </summary>
            /// <param name="specs">Ship specification</param>
            /// <returns>true if data is visible, false otherwise</returns>
            public bool Update(IShipSpecification specs)
            {
                View.Value.text = Data.GetValue(specs);
                View.Color = Data.GetColor(specs);
                var visible = Data.Visible(specs);

                View.gameObject.SetActive(visible);
                return visible;
            }
        }

        private readonly struct BlockData
        {
            public BlockData(IEnumerable<ILineData> content, Func<IShipSpecification, bool> visible,
                bool separatorVisible)
            {
                Content = content;
                Visible = visible;
                SeparatorVisible = separatorVisible;
            }

            public readonly IEnumerable<ILineData> Content;
            public readonly Func<IShipSpecification, bool> Visible;
            public readonly bool SeparatorVisible;
        }
    }
}
