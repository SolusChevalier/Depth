using UnityEngine;

public class Player_Diver_Animation : MonoBehaviour
{

    private Animator anim;

    bool swim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            swim = true;

        }
        else { swim = false; }

        anim.SetBool("IsSwimming", swim);
        
    }
}
