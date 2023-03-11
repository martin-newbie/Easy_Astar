using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitManager _instance = null;
    public static UnitManager Instance => _instance;

    [Header("Map")]
    public GroundUnit unitPrefab;
    public int height;
    public int width;

    public float unitSize;
    public int curState;
    Vector2 center;


    [Header("Path")]
    public GroundUnit[,] unitArray;
    public GroundUnit startUnit;
    public GroundUnit endUnit;

    public List<GroundUnit> openList = new List<GroundUnit>();
    public List<GroundUnit> closedList = new List<GroundUnit>();


    private void Start()
    {
        _instance = this;

        unitPrefab.transform.localScale = new Vector3(unitSize, unitSize, 1f);


        unitArray = new GroundUnit[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var temp = Instantiate(unitPrefab, transform);
                temp.transform.position = GetPosByIndex(y, x);
                temp.PosInit(x, y);
                temp.gameObject.name += string.Format("_({0}, {1})", x, y);
                unitArray[y, x] = temp;
            }
        }

        center = new Vector2(width / 2f, height / 2f) * unitSize;
    }

    public bool IsMinOutBoundX(float posX, float width)
    {
        return posX - width < center.x - (this.width / 2f * unitSize) - unitSize / 2f;
    }

    public bool IsMaxOutBoundX(float posX, float width)
    {
        return posX + width > center.x + (this.width / 2f * unitSize) - unitSize / 2f;
    }

    public bool IsMinOutBoundY(float posY, float height)
    {
        return posY - height < center.y - (this.height / 2f * unitSize) - unitSize / 2f;
    }

    public bool IsMaxOutBoundY(float posY, float height)
    {
        return posY + height > center.y + (this.height / 2f * unitSize) - unitSize / 2f;
    }

    Vector3 GetPosByIndex(int y, int x)
    {
        return new Vector3(x * unitSize, y * unitSize);
    }

    GroundUnit GetGroundUnitByPos(Vector3 inputPos)
    {
        int x = Mathf.RoundToInt(inputPos.x / unitSize);
        int y = Mathf.RoundToInt(inputPos.y / unitSize);

        return unitArray[y, x];
    }

    bool isPathFinding = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) curState = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) curState = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) curState = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) curState = 3;

        if (Input.GetMouseButtonDown(0) && curState == 1)
        {
            var unit = GetGroundUnitByPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (unit.unitType != UnitType.WALL)
            {
                if (startUnit != null) startUnit.model.color = Color.white;
                startUnit = unit;
                startUnit.model.color = new Color(1, 0.5f, 0f);
            }
        }
        if (Input.GetMouseButtonDown(0) && curState == 2)
        {
            var unit = GetGroundUnitByPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (unit.unitType != UnitType.WALL)
            {
                if (endUnit != null) endUnit.model.color = Color.white;
                endUnit = unit;
                endUnit.model.color = Color.blue;
            }
        }
        if (Input.GetMouseButtonDown(0) && curState == 3)
        {
            var unit = GetGroundUnitByPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (unit != startUnit && unit != endUnit)
                unit.ReverseUnitType();
        }

        if (Input.GetKeyDown(KeyCode.Space) && startUnit != null && endUnit != null && !isPathFinding)
        {
            StartCoroutine(PathFindCoroutine());
        }
    }

    IEnumerator PathFindCoroutine()
    {
        isPathFinding = true;
        GroundUnit curUnit = startUnit;
        closedList.Add(curUnit);

        while (curUnit != endUnit)
        {
            TryAddToOpenlist(curUnit.x + 1, curUnit.y, curUnit.g, curUnit);
            TryAddToOpenlist(curUnit.x - 1, curUnit.y, curUnit.g, curUnit);
            TryAddToOpenlist(curUnit.x, curUnit.y + 1, curUnit.g, curUnit);
            TryAddToOpenlist(curUnit.x, curUnit.y - 1, curUnit.g, curUnit);

            var lowest = FindLowestUnit();
            if (lowest == null)
            {
                // no way available
                yield break;
            }

            openList.Remove(lowest);
            closedList.Add(lowest);

            lowest.model.color = new Color(0.8f, 0.3f, 0.1f);
            curUnit = lowest;

            yield return new WaitForSeconds(0.1f);
        }

        while (curUnit != startUnit)
        {
            curUnit.model.color = new Color(1f, 1f, 0f);
            curUnit = curUnit.parentUnit;

            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    bool TryAddToOpenlist(int x, int y, int cost, GroundUnit parent)
    {

        if (x < 0 || y < 0 || x > width - 1 || y > height - 1)
        {
            return false;
        }
        var unit = unitArray[y, x];
        if (unit.unitType == UnitType.WALL)
        {
            return false;
        }
        if (closedList.Contains(unit) || openList.Contains(unit))
        {
            return false;
        }

        unit.SetCost(endUnit, ++cost);
        unit.SetParent(parent);
        unit.model.color = new Color(1, 1, 0.5f);
        openList.Add(unit);

        return true;
    }

    GroundUnit FindLowestUnit()
    {
        GroundUnit result = null;

        int cost = int.MaxValue;
        foreach (var item in openList)
        {
            if (item.f < cost)
            {
                cost = item.f;
                result = item;
            }
        }

        return result;
    }

}
