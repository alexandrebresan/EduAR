using UnityEngine;

[CreateAssetMenu(
    fileName = "NewQuestion",
    menuName = "EduAR/Question"
)]
public class Question : ScriptableObject
{
    public string questionText;

    public string[] alternatives = new string[4];

    public int correctAnswer;
}