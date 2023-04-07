using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
	private const string POPUP = "Popup";

	[SerializeField] private Image backgroundImage;
	[SerializeField] private Image iconImage;
	[SerializeField] TextMeshProUGUI messageText;
	[SerializeField] Color successColour;
	[SerializeField] Color failedColour;
	[SerializeField] Sprite successSprite;
	[SerializeField] Sprite failedSprite;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnReciepFailed;
		gameObject.SetActive(false);
	}

	private void DeliveryManager_OnReciepFailed(object sender, EventArgs e)
	{
		gameObject.SetActive(true);
		animator.SetTrigger(POPUP);
		backgroundImage.color = failedColour;
		iconImage.sprite = failedSprite;
		messageText.text = "DELIVERY\nFAILED";
	}

	private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
	{
		gameObject.SetActive(true);
		animator.SetTrigger(POPUP);

		backgroundImage.color = successColour;
		iconImage.sprite = successSprite;
		messageText.text = "DELIVERY\nSUCCESS";
	}
}