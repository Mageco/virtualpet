using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItem : MonoBehaviour
{
    public TextMesh text;
    // Start is called before the first frame update    
    public void Load(int n){
        text.text = n.ToString();
    }
}
