using UnityEngine;

public class PlayerHiding : MonoBehaviour
{
    public Camera Cam;
    public GameObject Player;
    private Ray ray;
    private RaycastHit hit;

    public void Start()
    {
        Cam = GetComponent<Camera>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        // Get the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Create a ray from the camera through the center of the screen
        ray = Cam.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Hideable"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Player.transform.position = hit.transform.position;
                    Player.transform.rotation = hit.transform.rotation;
                    //Player.GetComponent<PlayerManager>().IsHidden = true;
                }
            }
            else
            {
                //Player.GetComponent<PlayerManager>().IsHidden = false;
            }
        }
    }
}