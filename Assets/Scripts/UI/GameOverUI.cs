using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI recipesDeliveredText;

	private void Start()
	{
		KitchenGameManager.Instance.OnStateChanged += KitchenManager_OnStateChanged;
		Hide();
	}

	private void KitchenManager_OnStateChanged(object sender, EventArgs e)
	{
		if(KitchenGameManager.Instance.IsGameOver()) {
			Show();
			recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
		}
		else {
			Hide();
		}
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
