using UnityEngine;
using GameDatabase.DataModel;

namespace ViewModel
{
	public class WeaponInfoViewModel : MonoBehaviour
	{
		public TextFieldViewModel Damage;
		public TextFieldViewModel DPS;
		public TextFieldViewModel Energy;
		public TextFieldViewModel EPS;
		public TextFieldViewModel Range;
		public TextFieldViewModel FireRate;
		public TextFieldViewModel Velocity;
		public TextFieldViewModel Area;
		public TextFieldViewModel Impulse;

		public void SetWeapon(WeaponStats weapon, AmmunitionObsoleteStats ammunition)
		{
			Damage.gameObject.SetActive(weapon.FireRate > 0);
			DPS.gameObject.SetActive(Mathf.Approximately(weapon.FireRate, 0));
			Damage.Value.text = DPS.Value.text = ammunition.Damage.ToString();

			Energy.gameObject.SetActive(weapon.FireRate > 0);
			EPS.gameObject.SetActive(Mathf.Approximately(weapon.FireRate, 0));
			Energy.Value.text = EPS.Value.text = ammunition.EnergyCost.ToString();

			FireRate.gameObject.SetActive(weapon.FireRate > 0);
			FireRate.Value.text = weapon.FireRate.ToString();

			Range.gameObject.SetActive(ammunition.Range > 0);
			Range.Value.text = ammunition.Range.ToString();

			Velocity.gameObject.SetActive(ammunition.Velocity > 0);
			Velocity.Value.text = ammunition.Velocity.ToString();

			Impulse.gameObject.SetActive(ammunition.Impulse > 0);
			Impulse.Value.text = (ammunition.Impulse*1000).ToString();

			Area.gameObject.SetActive(ammunition.AreaOfEffect > 0);
			Area.Value.text = ammunition.AreaOfEffect.ToString();
		}
	}
}
