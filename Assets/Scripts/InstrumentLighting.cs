using UnityEngine;
using System.Collections;

public class InstrumentLighting : MonoBehaviour
{

    public GameObject Instrument;
    public Light InstrumentLight;

    public Camera mainCamera;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.name == Instrument.name)
            {
                InstrumentLight.enabled = !InstrumentLight.enabled;
            }
        }
    }
    }
}
