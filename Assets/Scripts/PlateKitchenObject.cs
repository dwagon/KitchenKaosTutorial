using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
	public class OnIngredientAddedEventArgs: EventArgs {
		public KitchenObjectSO kitchenObjectSO;
	}
	[SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

	public List<KitchenObjectSO> kitchenObjectSOList;

	public void Awake()
	{
		kitchenObjectSOList = new List<KitchenObjectSO>();
	}

	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
		if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
			// Not a plateable item
			return false;
		}
		if(kitchenObjectSOList.Contains(kitchenObjectSO)) {
			// Already contains this type
			return false;
		}
		kitchenObjectSOList.Add(kitchenObjectSO);

		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
			kitchenObjectSO = kitchenObjectSO
		});
		return true;
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList() {
		return kitchenObjectSOList;
	}

}
