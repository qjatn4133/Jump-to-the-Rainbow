using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePanel : MonoBehaviour {

    public GameObject[] icons;
    public GameObject[] featherIcons;

    // 라이프에 따라 스프라이트를 나누어 출력
    public void UpdateLife(int life)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i < life) icons[i].SetActive(true);
            else icons[i].SetActive(false);
        }
    }

    // 깃털에 따라 스프라이트를 나누어 출력
    public void UpdateFeather(int feather)
    {
        for (int i = 0; i < featherIcons.Length; i++)
        {
            if (i < feather) featherIcons[i].SetActive(true);
            else featherIcons[i].SetActive(false);
        }
    }
}
