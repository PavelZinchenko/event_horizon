using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Settings;
using Session;
using UnityEngine.Assertions;
using Utils;
using Zenject;
using Component = GameDatabase.DataModel.Component;

namespace Services.Localization
{
	public class LocalizationManager : ILocalization
	{
		[Inject] private ISessionData _sessionData;
		[Inject] private IDatabase _database;
        [Inject]
	    public LocalizationManager(GameSettings gameSettings)
        {
            _defaultLocalization = new LocalizationManager();
	        _gameSettings = gameSettings;
	        Reload();
	    }

	    public LocalizationManager()
	    {
	        LoadDefault();
	    }

	    public string GetString(string key, params object[] parameters)
	    {
		    var data = GetRawString(key, parameters);
		    
		    ApplyInjections(ref data);
		    return data;
	    }
		
		private string GetRawString(string key, params object[] parameters)
		{
		    try
		    {
		        if (string.IsNullOrEmpty(key) || key[0] != SpecialChar)
		            return key;

		        if (!_keys.TryGetValue(key.Substring(1), out var value))
		        {
		            if (_defaultLocalization != null)
		                return _defaultLocalization.GetString(key, parameters);

		            OptimizedDebug.Log("key not found: '" + key + "'");
	                return key;
		        }

		        value = value.Replace("\\n", "\n").Replace("\\", string.Empty);
                return ApplyParameters(value, parameters);
            }
            catch (Exception e)
		    {
			    OptimizedDebug.LogException(e);
                return key;
		    }
		}

		public void Reload(GameDatabase.IDatabase database = null)
		{
            var language = _gameSettings != null ? _gameSettings.Language : string.Empty;

		    if (string.IsNullOrEmpty(language))
				language = Application.systemLanguage.ToString();
		    if (!TryLoadResources(language))
		    {
		        language = _defaultLanguage;
		        TryLoadResources(language);
		    }

            if (database != null)
                LoadResourcesFromDatabase(database, language);

            if (_gameSettings != null)
		        _gameSettings.Language = language;

			InitPluralForms();
	    }

	    private void LoadResourcesFromDatabase(GameDatabase.IDatabase database, string language)
	    {
            if (database == null)
                return;

	        try
	        {
	            var data = database.GetLocalization(language);
	            if (string.IsNullOrEmpty(data))
	                return;

	            var serializer = new XmlSerializer(typeof(XmlLocalization));
	            LoadResourcesFromString(serializer, data);
	        }
            catch (Exception e)
	        {
	            Debug.LogError("Unable to load localization from database: " + e.Message);
	        }
        }

        private void LoadDefault()
	    {
	        TryLoadResources(_defaultLanguage);
            InitPluralForms();
        }

        private bool TryLoadResources(string language)
		{
    		_keys.Clear();

			var assets = Resources.LoadAll<TextAsset>("Localization/" + language);
			if (assets.Length == 0)
				return false;

		    var serializer = new XmlSerializer(typeof(XmlLocalization));
			foreach (var resource in assets)
			{
			    try
			    {
			        LoadResourcesFromString(serializer, resource.text);
			    }
                catch (Exception e)
			    {
                    Debug.LogError("Unable to load localization file: " + e.Message);
			    }
			}

			return true;
		}

	    private void LoadResourcesFromString(XmlSerializer serializer, string data)
	    {
	        XmlLocalization localization;
            using (var reader = new System.IO.StringReader(data))
                localization = serializer.Deserialize(reader) as XmlLocalization;

            foreach (var item in localization.items)
	        {
	            if (_keys.ContainsKey(item.name))
	            {
	                UnityEngine.Debug.Log("LocalizationManager: duplicate name - " + item.name);
                    continue;
                }

	            _keys.Add(item.name, item.value);
	        }
	    }

        private string ApplyParameters(string value, object[] parameters)
		{
			var index = value.IndexOf(SpecialChar);
			if (index < 0)
				return value;

			var builder = new StringBuilder(value.Substring(0, index));

			while (true)
			{
				++index;
			    var ch = value[index];
				if (ch == '{')
				{
					var end = value.IndexOf('}', index + 1);
					Assert.IsTrue(end > index);
					AddPluralForm(builder, value.Substring(index + 1, end - index - 1), parameters);
					index = end + 1;
				}
				else if (char.IsDigit(ch))
				{
				    var id = 0;
				    while (index < value.Length && char.IsDigit(value[index]))
				    {
				        id = id*10 + (value[index] - '0');
				        ++index;
				    }

				    if (id > 0 && id <= parameters.Length)
				        builder.Append(parameters[id - 1]);
				    else
				        AddInvalidParameter(builder, id);
				}
				else if (char.IsLetter(ch))
				{
                    var start = index;
                    while (index < value.Length && char.IsLetterOrDigit(value[index]))
                        ++index;

				    var key = value.Substring(start, index - start);
				    string parameter;
				    if (_keys.TryGetValue(key, out parameter))
				        builder.Append(parameter);
				    else
				        AddInvalidParameter(builder, key);
				}

                var old = index;
				index = value.IndexOf(SpecialChar, index);
				if (index < 0)
				{
					builder.Append(value.Substring(old));
					break;
				}
				else
				{
					builder.Append(value.Substring(old, index - old));
				}
			}

			return builder.ToString();
		}

		private void AddPluralForm(System.Text.StringBuilder builder, string format, object[] parameters)
		{
			var items = format.Split('|');
			var paramId = System.Convert.ToInt32(items[0]);
			if (paramId <= 0 || paramId > parameters.Length)
			{
				AddInvalidParameter(builder, paramId);
				return;
			}

			var count = System.Convert.ToInt32(parameters[paramId-1]);
	        var id = 0;
			foreach (var item in _pluralForms)
			{
	            if (item.IsMatch(count))
				{
					id = item.Id;
					break;
				}
			}
			if (id > 0)
				builder.Append(ApplyParameters(items[id], parameters));
			else
				AddInvalidParameter(builder, id);
	    }

		private void AddInvalidParameter(System.Text.StringBuilder builder, int id)
		{
			builder.Append('[');
			builder.Append(SpecialChar);
			builder.Append(id);
			builder.Append(']');
	    }

        private void AddInvalidParameter(System.Text.StringBuilder builder, string key)
        {
            builder.Append('[');
            builder.Append(SpecialChar);
            builder.Append(key);
            builder.Append(']');
        }

        private void InitPluralForms()
		{
			_pluralForms.Clear();

			string format;
			if (!_keys.TryGetValue("PluralFroms", out format) || string.IsNullOrEmpty(format))
				return;

			foreach (var item in format.Split(' '))
				_pluralForms.Add(PluralForm.FromString(item));
		}


        private void ApplyInjections(ref string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_sessionData == null)
	        {
		        text = FormatterRegex.Replace(text, "!!!Session is not initialized!!!");
		        return;
	        }
	        if (_database == null)
	        {
		        text = FormatterRegex.Replace(text, "!!!Database is not initialized!!!");
		        return;
	        }
	        text = FormatterRegex.Replace(text, FormatOneGroup);
        }

        private string FormatOneGroup(Match match)
        {
	        var type = match.Groups[1].Value;
	        var id = int.Parse(match.Groups[2].Value);
	        switch (type)
	        {
		        case "rep":
			        if (_database.GetCharacter(new ItemId<Character>(id)) == Character.DefaultValue)
			        {
				        return $"!!!Character with id {id} was not found!!!";
			        }
			        return _sessionData.Quests.GetCharacterRelations(id).ToString();
		        case "item":
			        if (_database.GetQuestItem(new ItemId<QuestItem>(id)) == QuestItem.DefaultValue)
			        {
				        return $"!!!Quest item with id {id} was not found!!!";
			        }
			        return _sessionData.Resources.Resources.GetQuantity(id).ToString();
		        case "comp":
			        if (_database.GetComponent(new ItemId<Component>(id)) == Component.DefaultValue)
			        {
				        return $"!!!Component with id {id} was not found!!!";
			        }
			        return _sessionData.Inventory.Components.GetQuantity(id).ToString();
		        default:
			        throw new Exception(
				        $"Invalid request type encountered, got {type} where rep, item or comp was expected");
	        }
        }
		private void Awake()
		{
			Reload();
	    }
	    
		public class PluralForm
		{
			public readonly int Id;
			public readonly int RangeMin;
			public readonly int RangeMax;
			public readonly int Basis;

			public static PluralForm FromString(string format)
			{
				var items = format.Split(':');
				var id = System.Convert.ToInt32(items[1]);
				items = items[0].Split('%');
				var basis = items.Length > 1 ? System.Convert.ToInt32(items[1]) : 0;
				items = items[0].Split('-');

				int min = int.MinValue;
				int max = int.MaxValue;
				if (items[0] != "*")
				{
					min = System.Convert.ToInt32(items[0]);
	                max = items.Length > 1 ? System.Convert.ToInt32(items[1]) : min;
				}

				return new PluralForm(id, min, max, basis);
			}

			public bool IsMatch(int value)
			{
				if (Basis > 0)
					value %= Basis;
				return value >= RangeMin && value <= RangeMax;
			}

			private PluralForm(int id, int min, int max, int basis)
			{
				Id = id;
				RangeMin = min;
				RangeMax = max;
				Basis = basis;
			}
		}

		public class XmlKeyValuePair
		{
			[XmlAttribute]
			public string name = string.Empty;
            [XmlText]
            public string value = string.Empty;
		}
	     
		[XmlRootAttribute("resources")]
		public class XmlLocalization
		{
			[XmlElement("string")]
			public List<XmlKeyValuePair> items = new List<XmlKeyValuePair>();
	    }

		private List<PluralForm> _pluralForms = new List<PluralForm>();
		private Dictionary<string, string> _keys = new Dictionary<string, string>();
		private const char SpecialChar = '$';
	    private GameSettings _gameSettings;
	    
	    private static readonly Regex FormatterRegex = new Regex(@"\$(rep|item|comp)\{(\d+)\}");
	    private const string _defaultLanguage = "English";
        private readonly LocalizationManager _defaultLocalization;
	}
}
