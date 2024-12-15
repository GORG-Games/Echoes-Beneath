using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GameObject _bulletImagePrefab; // Prefab for the bullet Image
    [SerializeField] private Transform _ammoContainer;      // Container holding the bullet images

    private List<GameObject> bulletImages = new List<GameObject>();

    // Initialize or update the ammo display based on maxAmmo
    public void UpdateAmmoDisplay(int maxAmmo, int currentAmmo)
    {
        // Clear existing bullet images
        ClearAmmoDisplay();

        // Create bullet images based on maxAmmo
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bulletImage = Instantiate(_bulletImagePrefab, _ammoContainer);
            bulletImages.Add(bulletImage);

            // Deactivate the bullet if it's beyond the current ammo count
            bulletImage.SetActive(i < currentAmmo);
        }
    }

    // Clear all existing bullet images
    private void ClearAmmoDisplay()
    {
        foreach (GameObject bullet in bulletImages)
        {
            Destroy(bullet);
        }
        bulletImages.Clear();
    }
}