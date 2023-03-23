using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public event EventHandler OnCut;
	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
	private int cuttingProgress;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) {
			// No kitchen object on counter
			if (player.HasKitchenObject()) {
				// Player carrying something sliceable
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					player.GetKitchenObject().SetKitchenObjectParent(this);
					cuttingProgress = 0;
					CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
						progressNormalized = (float) cuttingProgress / cuttingRecipeSO.cuttingProgressMax
					});
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
			// There is an object here that is sliceable
			cuttingProgress++;
			OnCut?.Invoke(this, EventArgs.Empty);
			CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());

			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
				progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
			});

			if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
				KitchenObjectSO outputKitchObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

				GetKitchenObject().DestroySelf();
				KitchenObject.SpawnKitchenObject(outputKitchObjectSO, this);
			}
		}
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);
		return cuttingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOForInput(inputKitchenObjectSO);
		if (cuttingRecipeSO != null) {
			return cuttingRecipeSO.output;
		}
		else {
			return null;
		}
	}

	private CuttingRecipeSO GetCuttingRecipeSOForInput(KitchenObjectSO inputKitchenObjectSO) {
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
			if (cuttingRecipeSO.input == inputKitchenObjectSO) {
				return cuttingRecipeSO;
			}
		}
		return null;
	}
}