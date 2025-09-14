using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI gameInfoText;
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] GameObject menu;

    void Start()
    {
        hostButton.onClick.AddListener(Host);
        joinButton.onClick.AddListener(Join);
    }

    void Host()
    {
        NetworkManager.Singleton.StartHost();
        menu.SetActive(false);
        gameInfoText.gameObject.SetActive(true);
        gameInfoText.text = "Press Enter to start";
    }
    void Join()
    {
        NetworkManager.Singleton.StartClient();
        menu.SetActive(false);
        gameInfoText.gameObject.SetActive(true);
        gameInfoText.text = "Waiting for host...";
    }
}
