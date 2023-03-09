using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    GROUND = 1,
    WALL = -1,
}

public class GroundUnit : MonoBehaviour
{
    public TextMesh gTxt, hTxt, fTxt;
    public UnitType unitType;
    public SpriteRenderer model;

    [Header("Path info")]
    public int x;
    public int y;

    public int g, h, f;
    public GroundUnit parentUnit;

    private void Start()
    {
        model = GetComponent<SpriteRenderer>();
    }

    public void PosInit(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetCost(GroundUnit endUnit, int fromStart)
    {
        g = fromStart;
        h = Mathf.Abs(endUnit.x - x) + Mathf.Abs(endUnit.y - y);
        f = g + h;

        gTxt.text = g.ToString();
        hTxt.text = h.ToString();
        fTxt.text = f.ToString();
    }

    public void SetParent(GroundUnit parent)
    {
        parentUnit = parent;
    }

    public void ReverseUnitType()
    {
        unitType = (UnitType)((int)unitType * -1);

        switch (unitType)
        {
            case UnitType.GROUND:
                model.color = Color.white;
                break;
            case UnitType.WALL:
                model.color = Color.black;
                break;
        }
    }
}
