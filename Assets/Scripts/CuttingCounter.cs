using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) {
			// No kitchen object on counter
			if (player.HasKitchenObject()) {
				// Player carrying something sliceable
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					player.GetKitchenObject().SetKitchenObjectParent(this);
				}
			}
		} else {
			// Kitchen obj present on counter
			if (player.HasKitchenObject()) {
				// Player is already carrying something
			} else {
				GetKitchenObject().SetKitchenObjectParent(player);
			}
		}
	}

	public override void InteractAlternate(Player player)
	{
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
			KitchenObjectSO outputKitchObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
			GetKitchenObject().DestroySelf();
			KitchenObject.SpawnKitchenObject(outputKitchObjectSO, this);
		}
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
			if (cuttingRecipeSO.input == inputKitchenObjectSO) {
				return true;
			}
		}
		return false;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
			if (cuttingRecipeSO.input == inputKitchenObjectSO) {
				return cuttingRecipeSO.output;
			}
		}
		return null;
	}
}