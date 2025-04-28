using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public string puzzleID; // ex. "PuzzleColor"

    private void OnEnable()
    {
        // Quan el puzzle es fa actiu...
        StatsManager.Instance.StartPuzzle(puzzleID);
    }

    public void OnSolved()
    {
        // Quan el jugador resol el puzzle...
        StatsManager.Instance.CompletePuzzle(puzzleID);
        // La resta de la teva lògica de puzzle resolt...
    }
}
