using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image imageTimer;

    void Update()
    {
        imageTimer.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();        
    }
}
