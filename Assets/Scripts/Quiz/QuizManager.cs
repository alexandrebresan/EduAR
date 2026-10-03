using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button[] answerButtons;

    [Header("Navegação")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    [Header("AR")]
    [SerializeField] private ARObjectPlacement arObjectPlacement;

    [Header("Tela Inicial")]
    [SerializeField] private HomeScreenManager homeScreenManager;

    [Header("Desafio")]
    [SerializeField] private QRChallengeManager challengeManager;

    [Header("Perguntas")]
    [SerializeField] private Question sunQuestion;
    [SerializeField] private Question mercuryQuestion;
    [SerializeField] private Question venusQuestion;
    [SerializeField] private Question earthQuestion;
    [SerializeField] private Question marsQuestion;
    [SerializeField] private Question jupiterQuestion;
    [SerializeField] private Question saturnQuestion;
    [SerializeField] private Question neptuneQuestion;
    [SerializeField] private Question moonQuestion;

    [Header("Pontuação")]
    [SerializeField] private ScoreManager scoreManager;

    // Impede que a mesma pergunta seja respondida mais de uma vez.
    private bool questionAnswered = false;

    // Guarda a pergunta atualmente exibida.
    private Question currentQuestion;

    // Guarda o identificador do desafio atual para que ele
    // possa ser marcado como concluído após uma resposta correta.
    private string currentChallengeId;

    // Cores utilizadas para indicar o resultado das respostas.
    private Color normalColor = Color.white;
    private Color correctColor = new Color(0.3f, 0.8f, 0.3f);
    private Color wrongColor = new Color(1f, 0.4f, 0.4f);

    private void Awake()
    {
        // O painel do quiz começa oculto e só é exibido
        // depois que um modelo 3D é posicionado.
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        HideNavigationButtons();
    }

    /// <summary>
    /// Define o desafio do Sol e exibe sua pergunta.
    /// </summary>
    public void ShowSunQuestion()
    {
        currentChallengeId = "SOL_01";
        ShowQuestion(sunQuestion);
    }

    /// <summary>
    /// Define o desafio de Mercúrio e exibe sua pergunta.
    /// </summary>
    public void ShowMercuryQuestion()
    {
        currentChallengeId = "MERCURIO_01";
        ShowQuestion(mercuryQuestion);
    }

    /// <summary>
    /// Define o desafio de Vênus e exibe sua pergunta.
    /// </summary>
    public void ShowVenusQuestion()
    {
        currentChallengeId = "VENUS_01";
        ShowQuestion(venusQuestion);
    }

    /// <summary>
    /// Define o desafio da Terra e exibe sua pergunta.
    /// </summary>
    public void ShowEarthQuestion()
    {
        currentChallengeId = "TERRA_01";
        ShowQuestion(earthQuestion);
    }

    /// <summary>
    /// Define o desafio de Marte e exibe sua pergunta.
    /// </summary>
    public void ShowMarsQuestion()
    {
        currentChallengeId = "MARTE_01";
        ShowQuestion(marsQuestion);
    }

    /// <summary>
    /// Define o desafio de Júpiter e exibe sua pergunta.
    /// </summary>
    public void ShowJupiterQuestion()
    {
        currentChallengeId = "JUPITER_01";
        ShowQuestion(jupiterQuestion);
    }

    /// <summary>
    /// Define o desafio de Saturno e exibe sua pergunta.
    /// </summary>
    public void ShowSaturnQuestion()
    {
        currentChallengeId = "SATURNO_01";
        ShowQuestion(saturnQuestion);
    }

    /// <summary>
    /// Define o desafio de Netuno e exibe sua pergunta.
    /// </summary>
    public void ShowNeptuneQuestion()
    {
        currentChallengeId = "NETUNO_01";
        ShowQuestion(neptuneQuestion);
    }

    /// <summary>
    /// Define o desafio da Lua e exibe sua pergunta.
    /// </summary>
    public void ShowMoonQuestion()
    {
        currentChallengeId = "LUA_01";
        ShowQuestion(moonQuestion);
    }

    /// <summary>
    /// Prepara a interface do quiz com a pergunta
    /// e as alternativas do desafio atual.
    /// </summary>
    private void ShowQuestion(Question question)
    {
        // Libera uma nova resposta e guarda a pergunta atual.
        questionAnswered = false;
        currentQuestion = question;

        if (quizPanel == null || question == null)
        {
            return;
        }

        quizPanel.SetActive(true);

        // Exibe o texto da pergunta.
        questionText.text = question.questionText;

        // Copia as alternativas armazenadas no objeto Question
        // para os textos dos botões da interface.
        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = question.alternatives[i];
        }

        ResetButtonColors();

        /*
         * Os eventos dos botões são configurados dinamicamente.
         * A variável local index garante que cada botão envie
         * seu próprio índice para AnswerQuestion().
         */
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;

            answerButtons[i].interactable = true;

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(
                () => AnswerQuestion(index)
            );
        }

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        HideNavigationButtons();
        UpdateScoreText();
    }

    /// <summary>
    /// Verifica a alternativa escolhida pelo jogador,
    /// apresenta o feedback e atualiza a pontuação.
    /// </summary>
    public void AnswerQuestion(int answerIndex)
    {
        // Evita múltiplos cliques após a primeira resposta.
        if (questionAnswered)
        {
            return;
        }

        if (currentQuestion == null)
        {
            return;
        }

        // Garante que o índice recebido corresponde
        // a um botão de resposta existente.
        if (answerIndex < 0 || answerIndex >= answerButtons.Length)
        {
            return;
        }

        questionAnswered = true;

        // Depois da resposta, todos os botões são bloqueados
        // para impedir novas seleções.
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
        }

        /*
         * O Question armazena o índice da alternativa correta.
         * Portanto, basta comparar o índice escolhido pelo usuário
         * com currentQuestion.correctAnswer.
         */
        if (answerIndex == currentQuestion.correctAnswer)
        {
            Image selectedImage =
                answerButtons[answerIndex].GetComponent<Image>();

            // Destaca a alternativa correta em verde.
            if (selectedImage != null)
            {
                selectedImage.color = correctColor;
            }

            if (feedbackText != null)
            {
                feedbackText.text =
                    "✓ Resposta correta!\n\n" +
                    GetCorrectFeedback() +
                    "\n\n+10 pontos";
            }

            // Adiciona os pontos da resposta correta.
            if (scoreManager != null)
            {
                scoreManager.AddPoints(10);
            }

            // Marca o desafio como concluído para impedir
            // que ele conceda pontos novamente.
            if (challengeManager != null)
            {
                challengeManager.CompleteChallenge(
                    currentChallengeId
                );
            }
        }
        else
        {
            Image selectedImage =
                answerButtons[answerIndex].GetComponent<Image>();

            // Destaca em vermelho a alternativa selecionada incorretamente.
            if (selectedImage != null)
            {
                selectedImage.color = wrongColor;
            }

            // Localiza e destaca em verde a alternativa correta.
            Image correctImage =
                answerButtons[currentQuestion.correctAnswer]
                    .GetComponent<Image>();

            if (correctImage != null)
            {
                correctImage.color = correctColor;
            }

            if (feedbackText != null)
            {
                string correctAnswer =
                    currentQuestion.alternatives[
                        currentQuestion.correctAnswer
                    ];

                feedbackText.text =
                    "✗ Resposta incorreta!\n\n" +
                    "A resposta correta é:\n" +
                    correctAnswer;
            }
        }

        UpdateScoreText();
        ShowNavigationButtons();
    }

    /// <summary>
    /// Retorna uma explicação educativa correspondente
    /// à pergunta respondida corretamente.
    /// </summary>
    private string GetCorrectFeedback()
    {
        if (currentQuestion == sunQuestion)
        {
            return
                "O hidrogênio é o elemento mais abundante no Sol " +
                "e é fundamental para as reações de fusão nuclear " +
                "que liberam sua energia.";
        }

        if (currentQuestion == mercuryQuestion)
        {
            return
                "Mercúrio é o planeta mais próximo do Sol " +
                "e também possui o período de translação mais curto " +
                "entre os planetas.";
        }

        if (currentQuestion == venusQuestion)
        {
            return
                "A atmosfera densa de Vênus, rica em dióxido de carbono, " +
                "provoca um intenso efeito estufa que mantém sua " +
                "superfície extremamente quente.";
        }

        if (currentQuestion == earthQuestion)
        {
            return
                "O Oceano Pacífico é o maior oceano da Terra, " +
                "ocupando uma grande parte da superfície do planeta.";
        }

        if (currentQuestion == marsQuestion)
        {
            return
                "Marte possui óxido de ferro em sua superfície, " +
                "que dá ao planeta sua aparência avermelhada.";
        }

        if (currentQuestion == jupiterQuestion)
        {
            return
                "Júpiter é o maior planeta do Sistema Solar " +
                "e possui uma massa muito superior à dos demais planetas.";
        }

        if (currentQuestion == saturnQuestion)
        {
            return
                "Saturno possui um extenso sistema de anéis formado " +
                "principalmente por partículas de gelo, rocha e poeira.";
        }

        if (currentQuestion == neptuneQuestion)
        {
            return
                "Netuno possui alguns dos ventos mais rápidos " +
                "do Sistema Solar, que podem ultrapassar 2.000 km/h.";
        }

        if (currentQuestion == moonQuestion)
        {
            return
                "As fases da Lua acontecem devido à posição relativa " +
                "entre o Sol, a Terra e a Lua.";
        }

        return "";
    }

    /// <summary>
    /// Restaura a cor padrão de todos os botões antes
    /// de apresentar uma nova pergunta.
    /// </summary>
    private void ResetButtonColors()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage =
                answerButtons[i].GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = normalColor;
            }
        }
    }

    /// <summary>
    /// Atualiza na interface o valor atual da pontuação.
    /// </summary>
    private void UpdateScoreText()
    {
        if (scoreText != null && scoreManager != null)
        {
            scoreText.text =
                $"Pontuação: {scoreManager.Score}";
        }
    }

    /// <summary>
    /// Oculta os botões de navegação enquanto
    /// a pergunta ainda não foi respondida.
    /// </summary>
    private void HideNavigationButtons()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }

        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Exibe os botões de navegação depois da resposta.
    /// </summary>
    private void ShowNavigationButtons()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = true;
        }

        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(true);
            exitButton.interactable = true;
        }
    }

    /// <summary>
    /// Fecha o quiz atual e prepara a aplicação
    /// para a leitura de outro QR Code.
    /// </summary>
    public void ContinueChallenge()
    {
        HideNavigationButtons();

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        if (arObjectPlacement != null)
        {
            arObjectPlacement.ContinueChallenge();
        }
    }

    /// <summary>
    /// Fecha o desafio atual e retorna à tela inicial.
    /// </summary>
    public void ExitChallenge()
    {
        HideNavigationButtons();

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        if (arObjectPlacement != null)
        {
            arObjectPlacement.ExitChallenge();
        }

        if (homeScreenManager != null)
        {
            homeScreenManager.ShowHome();
        }
    }
}