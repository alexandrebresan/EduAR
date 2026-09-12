using UnityEngine;

public class QRChallengeManager : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private GameObject marsObject;
    [SerializeField] private ARObjectPlacement arObjectPlacement;

    [Header("Quiz")]
    [SerializeField] private QuizManager quizManager;

    private void Start()
    {
        // TESTE TEMPORÁRIO
        ProcessQRCode("MARTE_01");
    }

    public void ProcessQRCode(string qrContent)
    {
        Debug.Log($"Desafio recebido: {qrContent}");

        switch (qrContent)
        {
            case "MARTE_01":
                OpenMarsChallenge();
                break;

            default:
                Debug.LogWarning($"QR Code desconhecido: {qrContent}");
                break;
        }
    }

    private void OpenMarsChallenge()
    {
        Debug.Log("Desafio de Marte iniciado!");

        if (marsObject != null)
        {
            marsObject.SetActive(false);
        }

        if (arObjectPlacement != null)
        {
            arObjectPlacement.EnablePlacement();
        }

        if (quizManager != null)
        {
            quizManager.ShowMarsQuestion();
        }
    }
}