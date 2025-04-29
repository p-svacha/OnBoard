using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthDisplay : MonoBehaviour
{
    [Header("Elements")]
    public GameObject HeartContainer;

    [Header("Prefabs")]
    public GameObject HeartPrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(HeartContainer);

        for (int i = 0; i < Game.Instance.MaxHealth; i += 2)
        {
            GameObject heart = GameObject.Instantiate(HeartPrefab, HeartContainer.transform);
            Image img = heart.GetComponent<Image>();

            int thisHeart = Game.Instance.Health - i;

            if (thisHeart < 1) img.sprite = ResourceManager.LoadSprite("Sprites/Health/EmptyHeart");
            else if (thisHeart == 1) img.sprite = ResourceManager.LoadSprite("Sprites/Health/HalfHeart");
            else if (thisHeart > 1) img.sprite = ResourceManager.LoadSprite("Sprites/Health/FullHeart");
        }
    }
}
