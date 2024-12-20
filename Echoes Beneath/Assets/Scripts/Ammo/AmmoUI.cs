using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GameObject _bulletImagePrefab; // Prefab for the bullet Image
    [SerializeField] private Transform _ammoContainer;      // Container holding the bullet images

    private List<Image> bulletImages = new List<Image>();
    [SerializeField] private Shotgun _shotgun;
    private int _maxAmmo;
    private GameObject _ammoIcon;
    private Image _ammoImage;
    [SerializeField] private Sprite activeAmmoSprite;        // Спрайт для активного патрона
    [SerializeField] private Sprite inactiveAmmoSprite;      // Спрайт для неактивного патрона

    private void Start()
    {
        _maxAmmo = _shotgun.MaxAmmo;
        for (int i = 0; i < _maxAmmo; i++)
        {
            _ammoIcon = Instantiate(_bulletImagePrefab, _ammoContainer);
            _ammoImage = _ammoIcon.GetComponent<Image>();
            _ammoImage.sprite = activeAmmoSprite;
            bulletImages.Add(_ammoImage);
        }
    }
    // Update the ammo display based on maxAmmo
    public void UpdateAmmoDisplay(int maxAmmo, int currentAmmo)
    {
        for (int i = 0; i < maxAmmo; i++)
        {
            if (i < currentAmmo)
            {
                bulletImages[i].sprite = activeAmmoSprite;
            }
            else
            {
                bulletImages[i].sprite = inactiveAmmoSprite;
            }
        }
    }
}