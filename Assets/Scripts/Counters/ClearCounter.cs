using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

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
			}
			else {
				GetKitchenObject().SetKitchenObjectParent(player);
			}
		}
	}
}
