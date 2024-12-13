using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GameObject bulletImagePrefab; // Prefab for the bullet Image
    [SerializeField] private Transform ammoContainer; // Container holding the bullet images
    [SerializeField] private float spacing = 5f; // Spacing between bullet images

    private List<GameObject> bulletImages = new List<GameObject>();

    void Start()
    {
        // Add a Horizontal Layout Group component if it's not already present
        if (ammoContainer.GetComponent<HorizontalLayoutGroup>() == null)
        {
            HorizontalLayoutGroup layoutGroup = ammoContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
        }
    }

    // Initialize the ammo display based on maxAmmo
    public void InitializeAmmoDisplay(int maxAmmo)
    {
        // Clear existing bullet images
        foreach (GameObject bullet in bulletImages)
        {
            Destroy(bullet);
        }
        bulletImages.Clear();

        // Create bullet images based on maxAmmo
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bulletImage = Instantiate(bulletImagePrefab, ammoContainer);
            bulletImages.Add(bulletImage);
        }
    }

    // Update the ammo display based on the current ammo count
    public void UpdateAmmoDisplay(int currentAmmo)
    {
        for (int i = 0; i < bulletImages.Count; i++)
        {
            bulletImages[i].SetActive(i < currentAmmo);
        }
    }
}