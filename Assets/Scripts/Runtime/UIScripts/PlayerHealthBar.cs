using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private List<Image> healthPips = new List<Image>();
    [SerializeField] private Sprite healthPipFilled;
    [SerializeField] private Sprite healthPipEmpty;

    [SerializeField]
    Image bkgImage;

    [SerializeField]
    Transform pipContainer;


    public void RerenderPips(int currenHealth, int maxHealth)
    {

        bkgImage.rectTransform.sizeDelta = new Vector2(5.62f*maxHealth - 5.03f, bkgImage.rectTransform.sizeDelta.y);
        for (var i = 0; i < maxHealth; i++)
        {

            if (healthPips.Count - 1 < i)
            {
                healthPips.Add(new GameObject().AddComponent<Image>());
                healthPips[i].transform.SetParent(pipContainer);
                healthPips[i].transform.localScale = Vector3.one;
                healthPips[i].rectTransform.sizeDelta = new Vector2(7, 7);

            }
            healthPips[i].sprite = i < currenHealth 
                ? healthPipFilled 
                : healthPipEmpty;
        }
    }
}
