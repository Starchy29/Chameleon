using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{

    //[SerializeField] private FieldOfView fieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Vector3 position = transform.rotation.eulerAngles;
        //Debug.Log(position);

        Rotate();

       // fieldOfView.SetAimDirection(position);
       // fieldOfView.SetOrigin(transform.position);
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, 20 * Time.deltaTime);
    }

}
