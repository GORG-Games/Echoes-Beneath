using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstAidKitUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject firstAidKitPrefab;       // Medkit prefab
    private Transform firstAidKitContainer;     // Container for medkits

    [Header("Settings")]
    [SerializeField] private int maxKits;            // Max quantity of medkits
    [SerializeField] private Sprite activeFirstAidKitSprite;     // Active medkit sprite
    [SerializeField] private Sprite inactiveFirstAidKitSprite;   // Inactive medkit sprite

    private List<Image> firstAidKitIcons = new List<Image>();
    private Image firstAidKitImage;
    private GameObject firstAidKitIcon;

    void Start()
    {
        firstAidKitContainer = gameObject.GetComponent<Transform>();
        // Creating medkit cells in UI
        for (int i = 0; i < maxKits; i++)
        {
            firstAidKitIcon = Instantiate(firstAidKitPrefab, firstAidKitContainer);
            firstAidKitImage = firstAidKitIcon.GetComponent<Image>();
            firstAidKitImage.sprite = inactiveFirstAidKitSprite;
            firstAidKitIcons.Add(firstAidKitImage);
        }
    }

    // Updating UI
    public void UpdateMedkitUI(int currentKits)
    {
        for (int i = 0; i < firstAidKitIcons.Count; i++)
        {
            if (i < currentKits)
            {
                firstAidKitIcons[i].sprite = activeFirstAidKitSprite;
            }
            else
            {
                firstAidKitIcons[i].sprite = inactiveFirstAidKitSprite;
            }
        }
    }
}