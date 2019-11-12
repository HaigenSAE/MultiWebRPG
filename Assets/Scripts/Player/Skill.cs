using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int level;
    public int curExp;

    public int reqXP()
    {
        int xp = 0;

        for (int i = 1; i < level; i++)
            xp += (int)Mathf.Floor(i + 300 * Mathf.Pow(2, i / 7));

        return (int)Mathf.Floor(xp / 4);
    }

    public void IncreaseExp(int incExp)
    {
        curExp += incExp;
        if (curExp >= reqXP())
            LevelUp();
    }

    public void LevelUp()
    {
        level++;
    }
}
