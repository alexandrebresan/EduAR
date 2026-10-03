using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectPlacement : MonoBehaviour
{
    [Header("Objeto")]
    [SerializeField] private GameObject objectToPlace;

    [Header("Quiz")]
    [SerializeField] private QuizManager quizManager;

    [Header("Scanner")]
    [SerializeField] private QrCodeScanner qrCodeScanner;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    private static readonly List<ARRaycastHit> hits = new();

    // Controla se o sistema está autorizado a procurar
    // uma superfície para posicionar o objeto atual.
    private bool placementEnabled = false;

    // Representa os diferentes tipos de desafio disponíveis.
    private enum ChallengeType
    {
        Sun,
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Neptune,
        Moon
    }

    private ChallengeType currentChallenge;

    private void Awake()
    {
        // Obtém os componentes de AR presentes no XR Origin.
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        // Garante que nenhum objeto seja exibido antes
        // da leitura de um QR Code válido.
        if (objectToPlace != null)
        {
            objectToPlace.SetActive(false);
        }

        // A detecção de planos permanece desativada até
        // que um desafio seja iniciado.
        if (planeManager != null)
        {
            planeManager.enabled = false;
        }
    }

    /// <summary>
    /// Define qual modelo 3D deverá ser posicionado no ambiente.
    /// </summary>
    public void SetObjectToPlace(GameObject newObject)
    {
        objectToPlace = newObject;

        // O objeto só será exibido depois que uma
        // superfície válida for encontrada.
        if (objectToPlace != null)
        {
            objectToPlace.SetActive(false);
        }
    }

    /// <summary>
    /// Define qual desafio está ativo para que a pergunta
    /// correspondente seja apresentada após o posicionamento.
    /// </summary>
    public void SetChallengeType(string challengeType)
    {
        switch (challengeType)
        {
            case "Sun":
                currentChallenge = ChallengeType.Sun;
                break;

            case "Mercury":
                currentChallenge = ChallengeType.Mercury;
                break;

            case "Venus":
                currentChallenge = ChallengeType.Venus;
                break;

            case "Earth":
                currentChallenge = ChallengeType.Earth;
                break;

            case "Mars":
                currentChallenge = ChallengeType.Mars;
                break;

            case "Jupiter":
                currentChallenge = ChallengeType.Jupiter;
                break;

            case "Saturn":
                currentChallenge = ChallengeType.Saturn;
                break;

            case "Neptune":
                currentChallenge = ChallengeType.Neptune;
                break;

            case "Moon":
                currentChallenge = ChallengeType.Moon;
                break;

            default:
                currentChallenge = ChallengeType.Mars;
                break;
        }
    }

    /// <summary>
    /// Habilita a detecção de planos e permite
    /// o posicionamento do objeto atual.
    /// </summary>
    public void EnablePlacement()
    {
        placementEnabled = true;

        if (planeManager != null)
        {
            planeManager.enabled = true;
        }
    }

    private void Update()
    {
        // Evita executar a lógica de posicionamento
        // quando nenhum desafio está aguardando um plano.
        if (!placementEnabled)
        {
            return;
        }

        if (planeManager == null || objectToPlace == null)
        {
            return;
        }

        // Percorre os planos atualmente detectados pelo AR Foundation.
        foreach (var plane in planeManager.trackables)
        {
            if (plane == null || !plane.gameObject.activeSelf)
            {
                continue;
            }

            /*
             * O centro do plano é fornecido em coordenadas locais.
             * Como o plano representa uma superfície, usamos seus
             * valores de centro para determinar onde o modelo será colocado.
             */
            Vector3 localCenter = new Vector3(
                plane.center.x,
                0f,
                plane.center.y
            );

            /*
             * Converte a posição local do centro do plano para
             * coordenadas globais da cena Unity.
             */
            Vector3 worldPosition =
                plane.transform.TransformPoint(localCenter);

            // Posiciona e exibe o modelo 3D no centro da superfície detectada.
            objectToPlace.transform.position = worldPosition;
            objectToPlace.SetActive(true);

            // Impede que o objeto seja reposicionado continuamente.
            placementEnabled = false;

            // Depois do posicionamento, a detecção de novos planos
            // deixa de ser necessária para o desafio atual.
            planeManager.enabled = false;

            // Oculta as representações visuais dos planos já detectados.
            HideDetectedPlanes();

            // Exibe a pergunta correspondente ao objeto posicionado.
            if (quizManager != null)
            {
                ShowCurrentQuestion();
            }

            // Apenas o primeiro plano válido é utilizado.
            break;
        }
    }

    /// <summary>
    /// Abre a pergunta correspondente ao desafio atual.
    /// </summary>
    private void ShowCurrentQuestion()
    {
        switch (currentChallenge)
        {
            case ChallengeType.Sun:
                quizManager.ShowSunQuestion();
                break;

            case ChallengeType.Mercury:
                quizManager.ShowMercuryQuestion();
                break;

            case ChallengeType.Venus:
                quizManager.ShowVenusQuestion();
                break;

            case ChallengeType.Earth:
                quizManager.ShowEarthQuestion();
                break;

            case ChallengeType.Mars:
                quizManager.ShowMarsQuestion();
                break;

            case ChallengeType.Jupiter:
                quizManager.ShowJupiterQuestion();
                break;

            case ChallengeType.Saturn:
                quizManager.ShowSaturnQuestion();
                break;

            case ChallengeType.Neptune:
                quizManager.ShowNeptuneQuestion();
                break;

            case ChallengeType.Moon:
                quizManager.ShowMoonQuestion();
                break;
        }
    }

    /// <summary>
    /// Finaliza o desafio atual e prepara o sistema
    /// para a leitura de um novo QR Code.
    /// </summary>
    public void ContinueChallenge()
    {
        RemoveCurrentObject();

        placementEnabled = false;

        if (planeManager != null)
        {
            planeManager.enabled = false;
        }

        if (qrCodeScanner != null)
        {
            qrCodeScanner.ResetScanner();
        }
    }

    /// <summary>
    /// Remove o objeto atual ao sair do desafio.
    /// </summary>
    public void ExitChallenge()
    {
        RemoveCurrentObject();

        placementEnabled = false;

        if (planeManager != null)
        {
            planeManager.enabled = false;
        }
    }

    /// <summary>
    /// Desativa o modelo 3D atualmente exibido.
    /// </summary>
    private void RemoveCurrentObject()
    {
        if (objectToPlace != null)
        {
            objectToPlace.SetActive(false);
        }

        HideDetectedPlanes();
    }

    /// <summary>
    /// Oculta todos os planos detectados pelo ARPlaneManager.
    /// </summary>
    private void HideDetectedPlanes()
    {
        if (planeManager == null)
        {
            return;
        }

        foreach (var plane in planeManager.trackables)
        {
            if (plane != null)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}