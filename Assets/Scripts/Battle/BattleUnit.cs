using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;
    [SerializeField] private bool isPlayerUnit;
    public Pokemon Pokemon { get; private set; }
    private RectTransform rectTransform;
    private Image imageComponent;

    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        imageComponent = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        imageComponent.sprite = Pokemon.Base.BackSprite;

        // Set initial position off the canvas
        rectTransform.anchoredPosition3D = isPlayerUnit
            ? new Vector3(-Screen.width, -159f, 2.86f)
            : new Vector3(Screen.width, -574f, 2.86f);

        StartCoroutine(MoveAndRotateImage(isPlayerUnit));
    }

    private IEnumerator MoveAndRotateImage(bool isPlayerUnit)
    {
        Vector3 startPosition = rectTransform.anchoredPosition3D;
        Vector3 endPosition = isPlayerUnit
            ? new Vector3(-292f, -159f, 2.86f)
            : new Vector3(-1050f, -574f, 2.86f);
        float duration = 2f;
        float elapsed = 0f;
        float rotationAmount = isPlayerUnit ? -5f : 5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;

            rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, endPosition, normalizedTime);

            rectTransform.Rotate(0f, 0f, rotationAmount * Time.deltaTime / duration * 360f);

            yield return null;
        }

        rectTransform.anchoredPosition3D = endPosition;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        imageComponent.sprite = Pokemon.Base.FrontSprite;
    }
}
