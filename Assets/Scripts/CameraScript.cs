using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject follow;
    int scope = 1;

    // Update is called once per frame
    //test;
    void Start(){

    }
    void Awake() {
        
    }
    void Update()
    {
        if (follow != null){
            transform.position = new Vector3(follow.transform.position.x, follow.transform.position.y , follow.transform.position.z - 10);
            if (scope != 2 && follow.GetComponent<Player>().GetInventory(8) == 2)
            {
                scope = 2;
                GetComponent<Camera>().orthographicSize = 8;
            }
        }
        
    }
}
