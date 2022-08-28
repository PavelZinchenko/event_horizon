using System;
using GameDatabase;
using Services.Messenger;
using UnityEngine;
using Zenject;

namespace Gui.MainMenu
{
    public class ModDependentElement : MonoBehaviour
    {
        [SerializeField] public bool VisibleWhenVanilla = true;
        [SerializeField] public bool VisibleWhenModLoaded = true;
        [SerializeField] public bool VisibleWhenModEditable = true;
        [Inject] private readonly IDatabase _database;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener(EventType.DatabaseLoaded, DatabaseLoaded);
        }

        private void DatabaseLoaded()
        {
            UpdateState(_database);
        }

        private void Awake()
        {
            UpdateState(_database);
        }

        public void UpdateState(IDatabase database)
        {
            Debug.Log("Is database editable?");
            Debug.Log(database.IsEditable);
            if (database.IsEditable)
                gameObject.SetActive(VisibleWhenModEditable);
            else if (database.Id != ModInfo.Default.Id)
                gameObject.SetActive(VisibleWhenModLoaded);
            else
                gameObject.SetActive(VisibleWhenVanilla);
        }
    }
}
