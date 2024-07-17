
using TMPro;
using UnityEngine;

public sealed class Score_Counter : MonoBehaviour
{
    public static Score_Counter Instance { get; private set; }

    private int _score;

    public int Score
    {
        get => _score;

        set
        {
            if (_score == value) return; 
        
            _score = value;

            scoreText.SetText(sourceText: $"Score = {_score}");
        
        }
    }
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => Instance =  this;
   
}