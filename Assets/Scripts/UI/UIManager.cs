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

        [SerializeField]
        private TMP_InputField _playerName;

        public void SetExplanationText(string text)
        {
            _explanationText.text = text;
        }

        public string PlayerName => string.IsNullOrWhiteSpace(_playerName.text) ? "Unnamed Pirate" : _playerName.text;
    }
}
