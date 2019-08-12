using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public float CommonTimeout;
    public float FadeTime;
    public float PlayTime;
    
    [Header("Init")] 
    [SerializeField] private PlayerController mPlayer1;
    [SerializeField] private Rigidbody mPlayer1Rb;
    
    [SerializeField] private PlayerController mPlayer2;
    [SerializeField] private Rigidbody mPlayer2Rb;
    
    [SerializeField] private BallTrigger mBall;
    [SerializeField] private Rigidbody mBallRb;
    
    [SerializeField] private Collider mPlayer1Gate;
    [SerializeField] private Collider mPlayer2Gate;

    [SerializeField] private TextMeshProUGUI mTimeText;
    [SerializeField] private TextMeshProUGUI mPlayer1ScoreText;
    [SerializeField] private TextMeshProUGUI mPlayer2ScoreText;

    [SerializeField] private TextMeshProUGUI mPopText;

    [SerializeField] private GameObject mEndButtons;
    [SerializeField] private string mGameSceneName;
    [SerializeField] private string mMenuSceneName;

    private Vector3 mPlayer1StartPosition,mPlayer2StartPosition;
    private Vector3 mBallStartPosition;
    
    private int mPlayer1Score, mPlayer2Score;
    private float mStartTime;
    
    void Start()
    {
        mStartTime = Time.time;
        mPlayer1StartPosition = mPlayer1Rb.position;
        mPlayer2StartPosition = mPlayer2Rb.position;
        mBallStartPosition = mBallRb.position;
        mBall.OnCollisionEnter += OnBallCollision;

        StartCoroutine(OnRoundStart());
    }
    
    void Update()
    {
        var playTime = Time.time - mStartTime;
        int minutes = (int) playTime / 60;
        int seconds = (int) playTime % 60;
        mTimeText.text = $"{minutes:00}:{seconds:00}";

        if (playTime > PlayTime)
        {
            if (mPlayer1Score != mPlayer2Score)
            {
                enabled = false;
                EndGame();
            }
        }
    }
    
    private void OnDestroy()
    {
        mBall.OnCollisionEnter -= OnBallCollision;
    }

    private IEnumerator OnRoundStart()
    {
        var roundStartTime = Time.time;
        mPopText.color = Color.white;
        mPopText.enabled = true;
        
        while (Time.time < roundStartTime + CommonTimeout)
        {
            mPopText.text = (roundStartTime + CommonTimeout - Time.time).ToString("0.00");
            yield return new WaitForEndOfFrame();
        }

        mPlayer1.enabled = true;
        mPlayer2.enabled = true;
        mPopText.text = "Начали!";
        StartCoroutine(FadeOutText(mPopText));

        yield break;
    }
    
    private IEnumerator OnRoundEnd()
    {
        mPopText.color = Color.red;
        mPopText.text = "ГОЛ!";
        mPopText.enabled = true;
        yield return new WaitForSeconds(2);
        mPopText.color = Color.white;

        
        StartCoroutine(OnRoundStart());
        yield break;
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        var startTime = Time.time;
        while (Time.time < startTime + FadeTime)
        {
            text.color = new Color(1, 1, 1, 1 - Mathf.InverseLerp(startTime, startTime + FadeTime, Time.time));
            yield return new WaitForEndOfFrame();
        }
        text.color = Color.white;
        text.enabled = false;
        yield break;
    }

    private void OnBallCollision(Collider playersGate)
    {
        if (playersGate == mPlayer1Gate)
        {
            OnGoalScoredBy(Player.Player2);
        }

        if (playersGate == mPlayer2Gate)
        {
            OnGoalScoredBy(Player.Player1);
        }
    }

    private void OnGoalScoredBy(Player player)
    {
        ResetWorld();
        switch (player)
        {
            case Player.Player1:
                mPlayer1Score++;
                mPlayer1ScoreText.text = mPlayer1Score.ToString();
                break;
            case Player.Player2:
                mPlayer2Score++;
                mPlayer2ScoreText.text = mPlayer2Score.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(player), player, null);
        }

        StartCoroutine(OnRoundEnd());
    }

    private void ResetWorld()
    {
        mPlayer1.enabled = false;
        mPlayer1.Reset();
        mPlayer1Rb.position = mPlayer1StartPosition;
        mPlayer1Rb.velocity = Vector3.zero;
        mPlayer2.enabled = false;
        mPlayer2.Reset();
        mPlayer2Rb.position = mPlayer2StartPosition;
        mPlayer2Rb.velocity = Vector3.zero;
        mBallRb.position = mBallStartPosition;
        mBallRb.velocity = Vector3.zero;
        mBallRb.angularVelocity = Vector3.zero;
    }
    
    private void EndGame()
    {
        ResetWorld();
        StopAllCoroutines();
        mPopText.color = Color.white;
        mPopText.text = mPlayer1Score > mPlayer2Score ? "Победил синий игрок" : "Победил красный игрок";
        mPopText.enabled = true;
        mEndButtons.SetActive(true);
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(mGameSceneName);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mMenuSceneName);
    }
}

public enum Player
{
    Player1,
    Player2
}