using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;

    public int Score
    {
        get { return score; }
    }

    public void AddPoints(int points)
    {
        score += points;

        Debug.Log($"Pontuação atual: {score}");
    }

    public void ResetScore()
    {
        score = 0;

        Debug.Log("Pontuação reiniciada.");
    }
}