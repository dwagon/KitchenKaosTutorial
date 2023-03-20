using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
	public static Player Instance { get; private set;}

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public class OnSelectedCounterChangedEventArgs: EventArgs {
		public ClearCounter selectedCounter;
	}

	[SerializeField] private float moveSpeed = 7.0f;
	[SerializeField] private float rotateSpeed = 10.0f;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private Transform kitchenObjectHoldPoint;
	private bool isWalking;
	private Vector3 lastInteractDir;
	private ClearCounter selectedCounter;
	private KitchenObject kitchenObject;


	private void Awake()
	{
		if (Instance != null) {
			Debug.LogError("There is more than one Player instance");
		}
		Instance = this;
	}

	private void Start()
	{
		gameInput.OnInteractAction += GameInput_OnInteractAction;
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (selectedCounter != null) {
			selectedCounter.Interact(this);
		}
	}

	private void Update() {
		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking() {
		return isWalking;
	}

	private void HandleInteractions() {
		float interactDistance = 2.0f;
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

		if (moveDir!=Vector3.zero) {
			lastInteractDir = moveDir;
		}
		if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
			if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
				if (clearCounter != selectedCounter) {
					SetSelectedCounter(clearCounter);
				}
			}
			else {
				SetSelectedCounter(null);
			}
		}
		else {
			SetSelectedCounter(null);
		}
	}

	private void SetSelectedCounter(ClearCounter selectedCounter) {
		this.selectedCounter = selectedCounter;
		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
			selectedCounter = selectedCounter
		});
	}

	private void HandleMovement() {
		float playerRadius = 0.7f;
		float playerHeight = 2.0f;
		float moveDistance = moveSpeed * Time.deltaTime;
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0.0f, inputVector.y);
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
		if (!canMove) {
			// Attempt only X movement
			Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
			canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
			if (canMove) {
				moveDir = moveDirX;
			} else {
				// Attempt only Z movement
				Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
				canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
				if (canMove) {
					moveDir = moveDirZ;
				}
			}

		}
		if (canMove) {
			transform.position += moveDir * moveSpeed * Time.deltaTime;
		}
		transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
		isWalking = moveDir != Vector3.zero;
	}


	public Transform GetKitchenObjectFollowTransform()
	{
		return kitchenObjectHoldPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}
}
