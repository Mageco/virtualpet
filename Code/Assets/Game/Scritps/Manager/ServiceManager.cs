using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterService(ServiceType type)
    {

    }
}

public class ServiceJob
{
    public ServiceType type;
    public System.DateTime timeStart;
    public bool isActive = false;
    float time;
}
