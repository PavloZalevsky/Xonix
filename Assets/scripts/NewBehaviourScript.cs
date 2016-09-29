using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

    void Start()
    {

        var height = Camera.main.orthographicSize * 2.0;
        var width = height * Screen.width / Screen.height;
        transform.localScale = new Vector3((float)width, (float)height, 0.1f);
    }

}
