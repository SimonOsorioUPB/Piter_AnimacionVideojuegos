using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject Denji;
    [SerializeField] private GameObject Baqir;
    [SerializeField] private GameObject Elena;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Baqir.SetActive(false);
        Elena.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Denji.SetActive(true);
            Baqir.SetActive(false);
            Elena.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Denji.SetActive(false);
            Baqir.SetActive(true);
            Elena.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Denji.SetActive(false);
            Baqir.SetActive(false);
            Elena.SetActive(true);
        }
    }
}
