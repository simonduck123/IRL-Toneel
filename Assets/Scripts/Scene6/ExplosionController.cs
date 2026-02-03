using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject tntOne;
    [SerializeField] private GameObject tntTwo;
    [SerializeField] private GameObject tntExplosionOne;
    [SerializeField] private GameObject tntExplosionTwo;

    public void PlantTntOne()
    {
        tntOne.SetActive(true);    
    }
    
    public void PlantTntTwo()
    {
        tntTwo.SetActive(true);    
    }

    public void PlayExplosionOne()
    {
        tntExplosionOne.SetActive(true);
        tntOne.SetActive(false); 
    }
    
    public void PlayExplosionTwo()
    {
        tntExplosionTwo.SetActive(true);
        tntTwo.SetActive(false); 
    }
}
