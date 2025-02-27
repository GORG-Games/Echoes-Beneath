using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DynamicSpriteMask : MonoBehaviour
{
    [SerializeField] private Light2D light2D; // Фонарик
    [SerializeField] private SpriteRenderer playerSprite; // Спрайт игрока
    [SerializeField] private Transform playerTransform;

    private Texture2D maskTexture;

    void Start()
    {
        GenerateMaskFromSprite();
    }
    void Update()
    {
        // Обновляем позицию маски на игроке, но предотвращаем поворот
        transform.position = playerTransform.position;
        transform.rotation = Quaternion.identity; // Обнуляем поворот, чтобы маска не крутилась
    }
    void GenerateMaskFromSprite()
    {
        if (playerSprite == null || playerSprite.sprite == null)
        {
            Debug.LogError("Player Sprite is missing!");
            return;
        }

        // Получаем текстуру спрайта игрока
        Texture2D playerTexture = playerSprite.sprite.texture;
        Rect spriteRect = playerSprite.sprite.rect;
        Color[] spritePixels = playerTexture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);

        // Создаём новую текстуру
        maskTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.wrapMode = TextureWrapMode.Clamp;

        // Обнуляем все пиксели (фон делаем белым, силуэт игрока чёрным)
        Color[] maskPixels = new Color[spritePixels.Length];

        for (int i = 0; i < spritePixels.Length; i++)
        {
            maskPixels[i] = spritePixels[i].a > 0.1f ? Color.black : Color.white; // Игрок → Чёрный, фон → Белый
        }

        maskTexture.SetPixels(maskPixels);
        Debug.Log("Applying mask to texture: " + maskTexture.name);
        maskTexture.Apply();

        // Создаём спрайт для Light Cookie
        light2D.lightCookieSprite = Sprite.Create(maskTexture, new Rect(0, 0, maskTexture.width, maskTexture.height), new Vector2(0.5f, 0.5f));
    }
}