using System.Collections.Generic;
using UnityEngine;

public class CasinoAnimationController : MonoBehaviour
{
    public List<GameObject> casinoPeople = new List<GameObject>();
    
    List<Animator> animators = new List<Animator>();

    public void SetDanger()
    {
        foreach (GameObject d in casinoPeople)
        {
            d.GetComponent<NPCAnimationManager>().SetScared();
        }
    }
        
        

}
