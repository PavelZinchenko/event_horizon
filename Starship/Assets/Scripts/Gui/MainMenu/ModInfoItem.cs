using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gui.MainMenu
{
    public class ModInfoItem : MonoBehaviour
    {
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _activeModMark;

        public string Id { get; private set; }

        public void Initialize(string name, string id, bool active)
        {
            _name.text = name;
            Id = id;
            _activeModMark.gameObject.SetActive(active);
        }
    }
}
