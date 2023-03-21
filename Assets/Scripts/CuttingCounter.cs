using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO cutKitchenObjectSO;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) {
			// No kitchen object on counter
			if (player.HasKitchenObject()) {
				player.GetKitchenObject().SetKitchenObjectParent(this);
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
		if (HasKitchenObject()) {
			// There is an object here
			GetKitchenObject().DestroySelf();
			KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
		}
	}
}
