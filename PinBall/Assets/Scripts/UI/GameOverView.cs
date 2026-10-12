using UnityEngine;
using Pinball.Core;

namespace Pinball.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private void Start()
        {
            if (panel != null) panel.SetActive(false);

            var gm = GameManager.Instance;
            if (gm != null) gm.OnGameOver += Show;
        }

        private void OnDestroy()
        {
            var gm = GameManager.Instance;
            if (gm != null) gm.OnGameOver -= Show;
        }

        private void Show()
        {
            if (panel != null) panel.SetActive(true);
        }
    }
}