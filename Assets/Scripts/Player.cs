using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.Application;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Inventory))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    const float epsilon = 0.0001f;
    
    [Header("Movement")]
    public float accelerationRate = 20f;
    public float rotationAccelerationRate = .2f;
    public float decelerationRate = 0.1f;
    public float maxRotationSpeed = 120;
    public float rotationDecelerationRate = 0.1f;
    [Header("Quick Turn")]
    public float quickTurnForce = 2f;
    public float quickTurnBufferTimeout = .2f;
    public float quickTurnTransferAngle = 70;
    public float quickTurnFinishRate = .7f;
    [Header("Mining")]
    public LayerMask miningLayerMask;
    public float regectionForce = 80;
    public float maxRegectionSpeed = 5;
    public float impactForce = 10;
    public float impactTimout = .2f;

    [Header("Events")]
    public UnityEvent OnDig;
    public UnityEvent OnMinedResource;
    public UnityEvent OnDigStart;
    public UnityEvent OnDigStop;
    public UnityEvent OnImpact;
    public UnityEvent OnThrust;
    public UnityEvent OnThrustStop;

    public Collider2D drillCollider;

    new private Rigidbody2D rigidbody;
    private Inventory inventory;
    private RecipeManager recipeManager;

    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    private InputAction moveAction;
    private InputAction turnAction;
    private InputAction quickTurnLeftAction;
    private InputAction quickTurnRightAction;

    bool digging = false;
    bool thrusting = false;
    bool itemsChanged;
    Vector2 Position => transform.position;
    void Awake()
    {
        playerActionMap = InputSystem.actions.FindActionMap("Player");
        uiActionMap = InputSystem.actions.FindActionMap("UI");

        moveAction = InputSystem.actions.FindAction("Move");
        turnAction = InputSystem.actions.FindAction("Turn");
        quickTurnLeftAction = InputSystem.actions.FindAction("QuickTurnLeft");
        quickTurnRightAction = InputSystem.actions.FindAction("QuickTurnRight");
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();

        recipeManager = FindFirstObjectByType<RecipeManager>();
    }
    private void OnEnable()
    {
        inventory.ItemsAdded += Inventory_ItemsAdded;
    }

    private void OnDisable()
    {
        inventory.ItemsAdded -= Inventory_ItemsAdded;
    }

    private void Inventory_ItemsAdded(object sender, ItemSet e)
    {
        itemsChanged = true;
    }

    float moveInput;
    float turnInput;
    bool doQuickTurn;
    float quickTurnDirection;
    void Update()
    {
        moveInput = moveAction.ReadValue<float>();
        turnInput = turnAction.ReadValue<float>();
        if(!digging)
        {
            if(quickTurnLeftAction.WasPerformedThisFrame())
            {
                doQuickTurn = true;
                quickTurnDirection = -1;
            }if(quickTurnRightAction.WasPerformedThisFrame())
            {
                doQuickTurn = true;
                quickTurnDirection = 1;
            }
        }
    }
    void FixedUpdate()
    {
        Turn();
        Move();
        Drill();
        collided = false;

        if(itemsChanged)
        {
            craftables.Clear();
            foreach (var (name, recipe) in recipeManager.Recipes)
            {
                int minCraftingCount = -1;
                foreach (var recipeInput in recipe.RecipeItems)
                {
                    int craftableCount = inventory.GetItemCount(recipeInput.Item) / recipeInput.Count;
                    if(craftableCount <= 0)
                    {
                        minCraftingCount = -1;
                        break;
                    }
                    if(minCraftingCount == -1 || craftableCount < minCraftingCount)
                    {
                        minCraftingCount = craftableCount;
                    }
                }
                if(minCraftingCount > 0)
                {
                    craftables.Add(new Craftable(recipe, minCraftingCount));
                }
            }
            itemsChanged = false;
        }
    }

    private void Move()
    {
        if (moveInput > epsilon)
        {
            if (!thrusting)
            {
                thrusting = true;
                OnThrust.Invoke();
            }
            var acceleration = transform.up * moveInput * accelerationRate;
            rigidbody.AddForceAtPosition(acceleration, Position, ForceMode2D.Force);
        }
        else
        {

            if (thrusting)
            {
                thrusting = false;
                OnThrustStop.Invoke();
            }
            if (rigidbody.linearVelocity.magnitude > epsilon)
            {
                rigidbody.AddForce(-rigidbody.linearVelocity * decelerationRate);
            }
            else
            {
                rigidbody.linearVelocity = new Vector2();
            }
        }
    }
    bool performingQuickTurn;
    float quickTurnInitialAngle;
    float quickTurnTarget;
    private void Turn()
    {
        if (doQuickTurn)
        {
            doQuickTurn = false;
            performingQuickTurn = true;
            quickTurnInitialAngle = Mathf.Repeat(rigidbody.rotation, 360);
            quickTurnBufferTime = Time.fixedTime + quickTurnBufferTimeout;
            quickTurnTarget = Mathf.Repeat(quickTurnInitialAngle + 180, 360);
            rigidbody.AddTorque(-quickTurnForce * quickTurnDirection, ForceMode2D.Impulse);
        }

        if (collided)
        {
            performingQuickTurn = false;
        }
        
        if (performingQuickTurn)
        {
            if(Mathf.Abs(Mathf.DeltaAngle(quickTurnTarget, rigidbody.rotation)) < quickTurnTransferAngle)
            {
                if(Mathf.Abs(turnInput) > epsilon)
                {
                    performingQuickTurn = false;
                }
                if(Mathf.Abs(Mathf.DeltaAngle(quickTurnTarget, rigidbody.rotation)) < epsilon)
                {
                    performingQuickTurn = false;
                    rigidbody.angularVelocity = 0;
                    rigidbody.rotation = quickTurnTarget;
                }
                else
                {
                    var nextAngle = Mathf.LerpAngle(rigidbody.rotation, quickTurnTarget, quickTurnFinishRate);
                    rigidbody.angularVelocity = Mathf.DeltaAngle(rigidbody.rotation, nextAngle) / Time.fixedDeltaTime;
                }
            }
            if(Time.fixedTime > quickTurnBufferTime)
            {
                if( Mathf.Abs(turnInput) > epsilon && Mathf.Sign(turnInput) == Mathf.Sign(rigidbody.angularVelocity))
                {
                    performingQuickTurn = false;
                }
            }
        }
        if(!performingQuickTurn)
        {
            if (Mathf.Abs(turnInput) > epsilon)
            {
                var acceleration = -turnInput * rotationAccelerationRate;
                rigidbody.AddTorque(acceleration);
            }
            else
            {
                if (Mathf.Abs(rigidbody.angularVelocity) > epsilon)
                {
                    rigidbody.AddTorque(-rigidbody.angularVelocity * rotationDecelerationRate * Time.fixedDeltaTime);
                }
                else
                {
                    rigidbody.angularVelocity = 0;
                }
            }
            if (Mathf.Abs(rigidbody.angularVelocity) > maxRotationSpeed)
            {
                rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity, -maxRotationSpeed, maxRotationSpeed);
            }
        }

    }

    float nextImpactTime;
    float drillTimer;
    float quickTurnBufferTime;
    Vector2 digPoint;
    MineableObject lastDugResource;
    private void Drill()
    {
        List<Collider2D> hits = new();
        Physics2D.OverlapCollider(drillCollider, new ContactFilter2D() { layerMask = miningLayerMask,  }, hits);
        if(hits.Count == 0)
        {
            if(digging)
            {
                digging = false;
                OnDigStop.Invoke();
            }
        }
        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody is null) continue;
            var resourceSource = hit.attachedRigidbody.gameObject.GetComponent<MineableObject>();
            if (resourceSource != null)
            {
                if(!digging)
                {
                    digging = true;
                    OnDigStart.Invoke();
                }
                OnDig.Invoke();
                drillTimer += Time.deltaTime;

                if (lastDugResource != resourceSource)
                {
                    lastDugResource = resourceSource;
                    drillTimer = 0;
                }

                if (drillTimer > 1)
                {
                    drillTimer = 0;
                    OnMinedResource.Invoke();
                    inventory.Add(resourceSource.GetDrillItems());
                    //inventory.Add(new ItemSet() { Item = resourceSource.Resource, Count = Mathf.CeilToInt(resourceSource.ResourceDensity) });
                }

                digPoint = Physics2D.ClosestPoint(Position, hit);
                Debug.DrawLine(transform.position, new Vector3(digPoint.x, digPoint.y, transform.position.z), Color.green);
                if (moveInput < epsilon)
                {
                    if (rigidbody.linearVelocity.magnitude < maxRegectionSpeed)
                    {
                        rigidbody.AddForce((Position - digPoint).normalized * regectionForce, ForceMode2D.Force);
                    }
                }
                else
                {
                    if (nextImpactTime < Time.fixedTime && Random.Range(0f, 1f) > .9f)
                    {
                        nextImpactTime = Time.fixedTime + impactTimout;
                        OnImpact.Invoke();
                        rigidbody.AddForce((Position - digPoint).normalized * impactForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    bool collided;
    void OnCollisionEnter(Collision collision)
    {
        collided = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }
    Vector2 scroll1Position;
    Vector2 scroll2Position;
    List<Craftable> craftables = new List<Craftable>();
    private void OnGUI()
    {
        GUILayout.Space(40);
        GUILayout.Label(@$"Speed: {rigidbody.linearVelocity.magnitude.ToString("0.00")}");
        GUILayout.Label(@$"Spin: {rigidbody.angularVelocity.ToString("0.00")}");
        scroll1Position = GUILayout.BeginScrollView(scroll1Position);
        foreach (var item in inventory.ItemSlots.Where(s => s.ItemSet != null))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(item.Item.Image, new GUIStyle() { fixedHeight = 20, fixedWidth = 20});
            GUILayout.Button($"{item.Item.DisplayName} ({item.ItemCount})");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        scroll2Position = GUILayout.BeginScrollView(scroll2Position);
        foreach (var craftable in craftables)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(craftable.Recipe.Result.Item.Image, new GUIStyle() { fixedHeight = 20, fixedWidth = 20});
            GUILayout.Label(craftable.Count.ToString());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            foreach(var item in craftable.Recipe.RecipeItems)
            {
                GUILayout.Box(item.Item.Image, new GUIStyle() { fixedHeight = 20, fixedWidth = 20 });
                GUILayout.Label(item.Count.ToString());
            }
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Craft"))
            {
                foreach (var item in craftable.Recipe.RecipeItems)
                {
                    inventory.Add(new ItemSet { Item = item.Item, Count = -item.Count });
                }
                inventory.Add(new ItemSet { Item = craftable.Recipe.Result.Item, Count = craftable.Recipe.Result.Count });
            }
        }
        GUILayout.EndScrollView();
    }
    class Craftable
    {
        public Recipe Recipe { get; }
        public int Count { get; }
        public Craftable(Recipe recipe, int count)
        {
            Recipe = recipe;
            Count = count;
        }

    }
}
