using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
	[SerializeField] private RecipeListSO recipeListSO;
	private List<RecipeSO> waitingRecipeSOList;

	private float spawnRecipeTimer;
	private float spawnRecipeTimerMax = 4f;

	private int waitingRecipesMax = 4;
	private int successfulRecipesAmount;

	public static DeliveryManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		waitingRecipeSOList = new List<RecipeSO>();
	}

	private void Update()
	{
		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0f) {
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (waitingRecipeSOList.Count < waitingRecipesMax) {
				RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
				waitingRecipeSOList.Add(waitingRecipeSO);
			}
		}
	}

	public void DeliveryRecipe(PlateKitchenObject plateKitchenObject) {
		for(int i =0; i< waitingRecipeSOList.Count; i++) {
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

			if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
				// Simple smoke check of number of objects
				bool plateContentsMatchRecipe = true;
				foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
					// Cycle through all the ingredients in the recipe
					bool ingredientFound = false;

					foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
						// Cycle through all the ingredients in the plate
						if (plateKitchenObjectSO == recipeKitchenObjectSO) {
							ingredientFound = true;
							break;
						}
					}
					if(!ingredientFound) {
						plateContentsMatchRecipe = false;
					}
				}

				if(plateContentsMatchRecipe) {
					successfulRecipesAmount++;
					waitingRecipeSOList.RemoveAt(i);
					return;
				}
			}
		}
		// Plater did not deliver a correct recipe
	}

	public List<RecipeSO> GetWaitingRecipeSOList() {
		return waitingRecipeSOList;
	}

	public int GetSuccessfulRecipesAmount() {
		return successfulRecipesAmount;
	}
}
