using UnityEngine;

public class LaunchProjectileOnClick : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float launchVelocity = 700f;
    public float projectileLifetime = 10f; // Lifetime of the projectile in seconds


    void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Perform a raycast from the camera to the mouse position on the ground plane
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Calculate the direction from the player to the click position
                Vector3 direction = hit.point - transform.position;
                direction.y = 0f; // Ensure the direction is horizontal
                
                // Instantiate the projectile at the player's position and launch it in the calculated direction
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().velocity = direction.normalized * launchVelocity;

                Destroy(projectile, projectileLifetime);

            }
        }
    }
}
