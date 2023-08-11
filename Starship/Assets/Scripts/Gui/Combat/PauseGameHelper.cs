using GameServices;
using UnityEngine;
using Zenject;

namespace Gui.Combat
{
    public class PauseGameHelper : MonoBehaviour
    {
        [Inject] private readonly GameFlow _gameFlow;

        public void PauseGame()
        {
            _gameFlow.Pause(gameObject);
        }

        public void ResumeGame()
        {
            _gameFlow.Resume(gameObject);
        }
    }
}
