using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs: EventArgs {
		public State state;
	}

	public enum State {
		Idle,
		Frying,
		Fried,
		Burned
	}

	[SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

	private State state;
	private float fryingTimer;
	private float burningTimer;
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;


	private void Start()
	{
		state = State.Idle;
	}

	private void Update() {
		if(!HasKitchenObject()) {
			return;
		}
		switch(state) {
			case State.Idle:
				break;
			case State.Frying:
				fryingTimer += Time.deltaTime;
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
					progressNormalized = (float)fryingTimer / fryingRecipeSO.fryingTimerMax
				});
				if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
					GetKitchenObject().DestroySelf();
					KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
					burningRecipeSO = GetBurningRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
					state = State.Fried;
					burningTimer = 0f;
					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
				}
				break;
			case State.Fried:
				burningTimer += Time.deltaTime;
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
					progressNormalized = (float)burningTimer / burningRecipeSO.burningTimerMax
				});
				if (burningTimer > burningRecipeSO.burningTimerMax) {
					GetKitchenObject().DestroySelf();
					KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
					state = State.Burned;
					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
						progressNormalized = 0f
					}) ;
				}
				break;
			case State.Burned:
				break;
		}
	}

	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) {
			// No kitchen object on counter
			if (player.HasKitchenObject()) {
				// Player carrying something fryable
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					player.GetKitchenObject().SetKitchenObjectParent(this);
					fryingRecipeSO = GetFryingRecipeSOForInput(GetKitchenObject().GetKitchenObjectSO());
					state = State.Frying;
					fryingTimer = 0f;
					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
						progressNormalized = (float)fryingTimer / fryingRecipeSO.fryingTimerMax
					});

				}
			}
		} else {
			// Kitchen obj present on counter
			if (player.HasKitchenObject()) {
				// Player is already carrying something
			} else {
				GetKitchenObject().SetKitchenObjectParent(player);
				state = State.Idle;
				OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
					progressNormalized = 0f
				});
			}
		}
	}


	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOForInput(inputKitchenObjectSO);
		return fryingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOForInput(inputKitchenObjectSO);
		if (fryingRecipeSO != null) {
			return fryingRecipeSO.output;
		} else {
			return null;
		}
	}

	private FryingRecipeSO GetFryingRecipeSOForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
			if (fryingRecipeSO.input == inputKitchenObjectSO) {
				return fryingRecipeSO;
			}
		}
		return null;
	}


	private BurningRecipeSO GetBurningRecipeSOForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
			if (burningRecipeSO.input == inputKitchenObjectSO) {
				return burningRecipeSO;
			}
		}
		return null;
	}

}
