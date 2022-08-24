using UnityEngine;

namespace Services.Localization
{
	public interface ILocalization
	{
		string GetString(string key, params object[] parameters);
        void Reload(GameDatabase.IDatabase database);
	}
}
