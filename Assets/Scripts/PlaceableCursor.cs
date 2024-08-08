using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceableCursor : MonoBehaviour
{
    [SerializeField] float placementRadius = 3;
    [SerializeField] float placementOffset = 1;

    private InputAction pointAction;
    private InputAction clickAction;

    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
    }
    bool clicked;
    private void Update()
    {
        if(clickAction.WasPerformedThisFrame())
        {
            clicked = true;
        }
    }
    void FixedUpdate()
    {
        var point = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
        point.z = 0;

        var pointXY = new Vector2(point.x, point.y);

        Collider2D[] colliders = new Collider2D[5];
        var collidedCount = Physics2D.OverlapCircle(point, placementRadius, new ContactFilter2D { }, colliders);
        
        for(int i = 0; i < collidedCount; i++)
        {
            var collider = colliders[i];

            if (collider.gameObject.GetComponent<FloorSnappable>() == null) continue;

            var closest = collider == null ? pointXY : Physics2D.ClosestPoint(point, collider);
            if(collider != null && closest != pointXY)
            {
                Debug.DrawLine(point, closest, Color.red);
                if(collider.OverlapPoint(pointXY))
                {
                    var mirrored = Mirror(pointXY, closest);
                    pointXY.x = mirrored.x;
                    pointXY.y = mirrored.y;
                }
                Vector2 direction = closest - pointXY;
                var hit = Physics2D.Raycast(pointXY, direction, direction.magnitude + .2f);

                transform.position = hit.point + hit.normal * placementOffset;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal)));
            }
            else
            {
                transform.rotation = Quaternion.identity;
                transform.position = pointXY;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, placementRadius);
    }
    public static Vector2 Mirror(in Vector2 mirrored, in Vector2 around)
    {
        return mirrored + 2 * (around - mirrored);
    }
}
