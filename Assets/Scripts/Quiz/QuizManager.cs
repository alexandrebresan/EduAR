using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Pergunta de Marte")]
    [SerializeField] private Question marsQuestion;

    [Header("Pontuação")]
    [SerializeField] private ScoreManager scoreManager;

    private bool questionAnswered = false;

    private void Awake()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

    }

    public void ShowMarsQuestion()
    {
        questionAnswered = false;

        if (quizPanel == null)
        {
            Debug.LogWarning("QuizPanel não foi configurado.");
            return;
        }

        quizPanel.SetActive(true);

        questionText.text = marsQuestion.questionText;

        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = marsQuestion.alternatives[i];
        }

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        Debug.Log("Pergunta de Marte exibida!");
    }

    public void AnswerQuestion(int answerIndex)
    {
        if (questionAnswered)
        {
            return;
        }

        questionAnswered = true;

        if (answerIndex == marsQuestion.correctAnswer)
        {
            Debug.Log("Resposta CORRETA!");

            if (feedbackText != null)
            {
                feedbackText.text = "Resposta correta!";
            }

            if (scoreManager != null)
            {
                scoreManager.AddPoints(10);
            }
        }
        else
        {
            Debug.Log("Resposta ERRADA!");

            if (feedbackText != null)
            {
                feedbackText.text = "Resposta errada!";
            }
        }
    }
}