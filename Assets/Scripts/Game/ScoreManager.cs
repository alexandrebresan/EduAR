using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Pontuação acumulada durante a partida atual.
    private int score = 0;

    /*
     * Expõe a pontuação somente para leitura.
     * Outros scripts podem consultar o valor,
     * mas a alteração continua centralizada neste componente.
     */
    public int Score
    {
        get { return score; }
    }

    /// <summary>
    /// Adiciona pontos à pontuação atual.
    /// </summary>
    public void AddPoints(int points)
    {
        score += points;

        Debug.Log($"Pontuação atual: {score}");
    }

    /// <summary>
    /// Reinicia a pontuação ao começar uma nova partida.
    /// </summary>
    public void ResetScore()
    {
        score = 0;

        Debug.Log("Pontuação reiniciada.");
    }
}