using TMPro;
using UnityEngine;

namespace MultiplayerGameJam.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private TMP_Text _explanationText;

        public void SetExplanationText(string text)
        {
            _explanationText.text = text;
        }
    }
}
