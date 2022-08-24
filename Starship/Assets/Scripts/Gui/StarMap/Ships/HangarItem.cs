using Constructor.Ships;
using GameDatabase.Enums;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    public class HangarItem : MonoBehaviour
    {
        [SerializeField] private Image _allowIcon;
        [SerializeField] private Image _denyIcon;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _lockIcon;
        [SerializeField] private Image _emptyImage1;
        [SerializeField] private Image _emptyImage2;
        [SerializeField] private Image _emptyImage3;
        [SerializeField] private Image _emptyImage4;
        [SerializeField] private Image _emptyImage5;

        [Inject] private readonly IResourceLocator _resourceLocator;

        public bool TryInstall(IShip ship)
        {
            if (ship != null && (_locked || ship.Model.SizeClass > _sizeClass))
                return false;

            Ship = ship;
            return true;
        }

        public SizeClass Size
        {
            get { return _sizeClass; }
            set
            {
                _sizeClass = value;
                Initialize();
            }
        }

        public Sprite EmptySprite
        {
            get
            {
                switch (_sizeClass)
                {
                    case SizeClass.Frigate:
                        return _emptyImage1.sprite;
                    case SizeClass.Destroyer:
                        return _emptyImage2.sprite;
                    case SizeClass.Cruiser:
                        return _emptyImage3.sprite;
                    case SizeClass.Battleship:
                        return _emptyImage4.sprite;
                    case SizeClass.Titan:
                        return _emptyImage5.sprite;
                    default:
                        return null;
                }
            }
        }

        public void Clear()
        {
            Ship = null;
        }

        public IShip Ship
        {
            get { return _ship; }
            private set
            {
                _ship = value;
                Initialize();
            }
        }

        public bool Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                Initialize();
            }
        }

        public void Highlight(bool enabled, SizeClass requiredClass = SizeClass.Frigate)
        {
            _allowIcon.gameObject.SetActive(enabled && requiredClass <= _sizeClass && !_locked);
            _denyIcon.gameObject.SetActive(enabled && (requiredClass > _sizeClass || _locked));
        }

        private void Initialize()
        {
            if (_locked)
            {
                _icon.gameObject.SetActive(false);
                _emptyImage1.gameObject.SetActive(false);
                _emptyImage2.gameObject.SetActive(false);
                _emptyImage3.gameObject.SetActive(false);
                _emptyImage4.gameObject.SetActive(false);
                _emptyImage5.gameObject.SetActive(false);
                _lockIcon.gameObject.SetActive(true);
                return;
            }

            _lockIcon.gameObject.SetActive(false);

            if (_ship != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.transform.localScale = new Vector3(_ship.Model.SizeClass.IconSize(), _ship.Model.SizeClass.IconSize(), 1.0f);

                _icon.sprite = _resourceLocator.GetSprite(_ship.Model.ModelImage);
                _icon.color = _ship.ColorScheme.HsvColor;
            }
            else
            {
                _icon.gameObject.SetActive(false);
            }

            _emptyImage1.gameObject.SetActive(!_icon.gameObject.activeSelf && _sizeClass == SizeClass.Frigate);
            _emptyImage2.gameObject.SetActive(!_icon.gameObject.activeSelf && _sizeClass == SizeClass.Destroyer);
            _emptyImage3.gameObject.SetActive(!_icon.gameObject.activeSelf && _sizeClass == SizeClass.Cruiser);
            _emptyImage4.gameObject.SetActive(!_icon.gameObject.activeSelf && _sizeClass == SizeClass.Battleship);
            _emptyImage5.gameObject.SetActive(!_icon.gameObject.activeSelf && _sizeClass == SizeClass.Titan);
        }

        private IShip _ship;
        private bool _locked;
        private SizeClass _sizeClass;
    }
}
