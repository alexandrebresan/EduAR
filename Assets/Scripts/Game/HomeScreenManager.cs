using UnityEngine;

public class HomeScreenManager : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject homePanel;

    [Header("Scanner")]
    [SerializeField] private QrCodeScanner qrCodeScanner;

    [Header("Pontuação")]
    [SerializeField] private ScoreManager scoreManager;

    private void Awake()
    {
        // Garante que a aplicação sempre inicie
        // exibindo a tela inicial.
        ShowHome();
    }

    /// <summary>
    /// Inicia uma nova partida.
    /// Oculta a tela inicial, reinicia a pontuação
    /// e libera o scanner para procurar novos QR Codes.
    /// </summary>
    public void PlayGame()
    {
        if (homePanel != null)
        {
            homePanel.SetActive(false);
        }

        // Cada nova partida começa com pontuação zerada.
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        if (qrCodeScanner != null)
        {
            /*
             * Garante que o componente do scanner esteja habilitado
             * e preparado para reconhecer um novo QR Code.
             */
            qrCodeScanner.enabled = true;
            qrCodeScanner.ResetScanner();
        }
    }

    /// <summary>
    /// Exibe a tela inicial e interrompe a leitura
    /// de QR Codes enquanto o usuário estiver no menu.
    /// </summary>
    public void ShowHome()
    {
        if (homePanel != null)
        {
            homePanel.SetActive(true);
        }

        if (qrCodeScanner != null)
        {
            // O scanner é desativado logicamente para que
            // os frames da câmera não iniciem desafios no menu.
            qrCodeScanner.DisableScanner();
        }
    }
}