using System.Collections.Generic;
using UnityEngine;

public class QRChallengeManager : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private GameObject sunObject;
    [SerializeField] private GameObject mercuryObject;
    [SerializeField] private GameObject venusObject;
    [SerializeField] private GameObject earthObject;
    [SerializeField] private GameObject marsObject;
    [SerializeField] private GameObject jupiterObject;
    [SerializeField] private GameObject saturnObject;
    [SerializeField] private GameObject neptuneObject;
    [SerializeField] private GameObject moonObject;

    [SerializeField] private ARObjectPlacement arObjectPlacement;

    [Header("Quiz")]
    [SerializeField] private QuizManager quizManager;

    // Armazena os desafios concluídos durante a sessão atual.
    // O HashSet facilita a verificação e impede que um mesmo
    // desafio seja contabilizado mais de uma vez.
    private readonly HashSet<string> completedChallenges = new();

    /// <summary>
    /// Recebe o texto lido pelo scanner de QR Code e identifica
    /// qual desafio deve ser iniciado.
    /// </summary>
    public void ProcessQRCode(string qrContent)
    {
        Debug.Log($"Desafio recebido: {qrContent}");

        // Cada QR Code possui um identificador único.
        // O switch associa esse identificador ao modelo 3D
        // e ao tipo de desafio correspondente.
        switch (qrContent)
        {
            case "SOL_01":
                OpenChallenge(
                    "SOL_01",
                    sunObject,
                    "Sun"
                );
                break;

            case "MERCURIO_01":
                OpenChallenge(
                    "MERCURIO_01",
                    mercuryObject,
                    "Mercury"
                );
                break;

            case "VENUS_01":
                OpenChallenge(
                    "VENUS_01",
                    venusObject,
                    "Venus"
                );
                break;

            case "TERRA_01":
                OpenChallenge(
                    "TERRA_01",
                    earthObject,
                    "Earth"
                );
                break;

            case "MARTE_01":
                OpenChallenge(
                    "MARTE_01",
                    marsObject,
                    "Mars"
                );
                break;

            case "JUPITER_01":
                OpenChallenge(
                    "JUPITER_01",
                    jupiterObject,
                    "Jupiter"
                );
                break;

            case "SATURNO_01":
                OpenChallenge(
                    "SATURNO_01",
                    saturnObject,
                    "Saturn"
                );
                break;

            case "NETUNO_01":
                OpenChallenge(
                    "NETUNO_01",
                    neptuneObject,
                    "Neptune"
                );
                break;

            case "LUA_01":
                OpenChallenge(
                    "LUA_01",
                    moonObject,
                    "Moon"
                );
                break;

            default:
                Debug.LogWarning(
                    $"QR Code desconhecido: {qrContent}"
                );
                break;
        }
    }

    /// <summary>
    /// Prepara o desafio correspondente ao QR Code detectado.
    /// </summary>
    private void OpenChallenge(
        string challengeId,
        GameObject challengeObject,
        string challengeType)
    {
        // Evita iniciar um desafio cujo modelo não tenha
        // sido configurado no Inspector.
        if (challengeObject == null)
        {
            Debug.LogWarning(
                "Objeto do desafio não foi configurado."
            );

            return;
        }

        // Impede que desafios já concluídos sejam novamente
        // utilizados para obter pontos.
        if (completedChallenges.Contains(challengeId))
        {
            Debug.Log(
                $"Desafio já concluído: {challengeId}"
            );

            return;
        }

        // Mantém apenas um corpo celeste ativo por vez.
        DisableAllObjects();

        if (arObjectPlacement != null)
        {
            // Define qual modelo será colocado na superfície detectada.
            arObjectPlacement.SetObjectToPlace(
                challengeObject
            );

            // Informa qual pergunta deverá ser exibida depois
            // que o modelo for posicionado.
            arObjectPlacement.SetChallengeType(
                challengeType
            );

            // Inicia a busca por uma superfície válida em AR.
            arObjectPlacement.EnablePlacement();
        }
    }

    /// <summary>
    /// Desativa todos os modelos para garantir que somente
    /// o objeto do desafio atual seja exibido.
    /// </summary>
    private void DisableAllObjects()
    {
        if (sunObject != null)
        {
            sunObject.SetActive(false);
        }

        if (mercuryObject != null)
        {
            mercuryObject.SetActive(false);
        }

        if (venusObject != null)
        {
            venusObject.SetActive(false);
        }

        if (earthObject != null)
        {
            earthObject.SetActive(false);
        }

        if (marsObject != null)
        {
            marsObject.SetActive(false);
        }

        if (jupiterObject != null)
        {
            jupiterObject.SetActive(false);
        }

        if (saturnObject != null)
        {
            saturnObject.SetActive(false);
        }

        if (neptuneObject != null)
        {
            neptuneObject.SetActive(false);
        }

        if (moonObject != null)
        {
            moonObject.SetActive(false);
        }
    }

    /// <summary>
    /// Marca um desafio como concluído após o jogador
    /// responder corretamente à pergunta.
    /// </summary>
    public void CompleteChallenge(string challengeId)
    {
        if (string.IsNullOrWhiteSpace(challengeId))
        {
            return;
        }

        if (completedChallenges.Contains(challengeId))
        {
            return;
        }

        completedChallenges.Add(challengeId);

        Debug.Log(
            $"Desafio concluído: {challengeId}"
        );
    }

    /// <summary>
    /// Permite verificar se um desafio já foi concluído.
    /// </summary>
    public bool IsChallengeCompleted(string challengeId)
    {
        return completedChallenges.Contains(challengeId);
    }
}