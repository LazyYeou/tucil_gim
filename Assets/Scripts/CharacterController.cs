using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Interaction")]
    public float interactionRange = 1.5f;
    public LayerMask farmPlotLayer;
    public Transform interactionPoint;

    public enum Tool { None, Seed, WateringCan, Hoe }
    [Header("Tools")]
    public Tool currentTool = Tool.None;

    [Header("UI")]
    public GameObject toolIndicator;
    public TMPro.TextMeshProUGUI toolText;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;
    private FarmManager farmManager;

    void Start()
    {
        farmManager = FarmManager.Instance;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (interactionPoint == null)
        {
            GameObject point = new GameObject("InteractionPoint");
            point.transform.SetParent(transform);
            point.transform.localPosition = new Vector3(0, -0.5f, 0);
            interactionPoint = point.transform;
        }

        UpdateToolUI();
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Track last move direction for interaction
        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement.normalized;
        }

        // Update interaction point position based on facing direction
        UpdateInteractionPoint();

        // Handle animations
        UpdateAnimations();

        // Tool switching
        HandleToolSwitching();

        // Interaction
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Set movement parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Set last move direction for idle animations
        if (movement != Vector2.zero)
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);
        }
    }

    void UpdateInteractionPoint()
    {
        // Position interaction point in front of player
        interactionPoint.localPosition = lastMoveDirection * 0.8f;
    }

    void HandleToolSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTool = Tool.Hoe;
            UpdateToolUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTool = Tool.Seed;
            UpdateToolUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentTool = Tool.WateringCan;
            UpdateToolUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentTool = Tool.None;
            UpdateToolUI();
        }
    }

    void UpdateToolUI()
    {
        if (toolText != null)
        {
            switch (currentTool)
            {
                case Tool.Hoe:
                    toolText.text = "Tool: Hoe";
                    break;
                case Tool.Seed:
                    toolText.text = farmManager != null && farmManager.selectedCrop != null
                        ? $"Tool: Seeds ({farmManager.selectedCrop.cropName})"
                        : "Tool: Seeds (None selected)";
                    break;
                case Tool.WateringCan:
                    toolText.text = "Tool: Watering Can";
                    break;
                default:
                    toolText.text = "Tool: None";
                    break;
            }
        }

        if (toolIndicator != null)
        {
            toolIndicator.SetActive(currentTool != Tool.None);
        }
    }

    void TryInteract()
    {
        // Check for farm plots in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRange, farmPlotLayer);

        if (hits.Length > 0)
        {
            // Get the closest farm plot
            FarmPlot closestPlot = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                FarmPlot plot = hit.GetComponent<FarmPlot>();
                if (plot != null)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlot = plot;
                    }
                }
            }

            if (closestPlot != null)
            {
                InteractWithPlot(closestPlot);
            }
        }
    }

    void InteractWithPlot(FarmPlot plot)
    {
        switch (currentTool)
        {
            case Tool.Seed:
                // Plant crop
                if (plot.currentCrop == null && farmManager.selectedCrop != null)
                {
                    plot.PlantCrop(farmManager.selectedCrop);
                    PlayPlantAnimation();
                    Debug.Log($"Planted {farmManager.selectedCrop.cropName}!");
                }
                else if (plot.currentCrop != null)
                {
                    Debug.Log("Plot already has a crop!");
                }
                else
                {
                    Debug.Log("No seed selected! Select a crop first.");
                }
                break;

            case Tool.WateringCan:
                // Water crop
                if (plot.currentCrop != null && !plot.currentCrop.isWatered)
                {
                    plot.WaterCrop();
                    PlayWaterAnimation();
                    Debug.Log("Watered the crop!");
                }
                else if (plot.currentCrop == null)
                {
                    Debug.Log("No crop to water!");
                }
                else
                {
                    Debug.Log("Crop is already watered!");
                }
                break;

            case Tool.None:
                // Harvest crop
                if (plot.currentCrop != null && plot.currentCrop.isGrown)
                {
                    plot.HarvestCrop();
                    PlayHarvestAnimation();
                    Debug.Log("Harvested crop!");
                }
                else if (plot.currentCrop != null)
                {
                    Debug.Log("Crop is not ready yet!");
                }
                break;

            case Tool.Hoe:
                // Could be used to till ground or remove crops
                if (plot.currentCrop != null && !plot.currentCrop.isWatered)
                {
                    plot.currentCrop = null;
                    plot.UpdateVisuals();
                    Debug.Log("Removed crop!");
                }
                break;
        }
    }

    void PlayPlantAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Plant");
        }
    }

    void PlayWaterAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Water");
        }
    }

    void PlayHarvestAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Harvest");
        }
    }

    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRange);
        }
    }
}