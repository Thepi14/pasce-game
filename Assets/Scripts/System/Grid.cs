using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> onGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, Func<Grid<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        this.width = width;
        this.height = height;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }
    }
    public void GetXY(Vector2 position, out int x, out int y)
    {
        x = (int)position.x;
        y = (int)position.y;
    }
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (CheckIfInsideGrid(x, y))
            gridArray[x, y] = value;
        if (onGridValueChanged != null)
            onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
    public void SetGridObject(Vector2 position, TGridObject value)
    {
        if (CheckIfInsideGrid((int)position.x, (int)position.y))
            gridArray[(int)position.x, (int)position.y] = value;
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = (int)position.x, y = (int)position.y });
    }
    /*public void SetGridObject(Vector3 position, TGridObject value)
    {
        if (CheckIfInsideArrayLimits((int)position.x, (int)position.y))
            gridArray[(int)position.x, (int)position.y] = value;
        if (onGridValueChanged != null)
            onGridValueChanged(this, new OnGridValueChangedEventArgs { x = (int)position.x, y = (int)position.y });
    }*/
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
    public TGridObject GetGridObject(int x, int y)
    {
        if (CheckIfInsideGrid(x, y))
            return gridArray[x, y];
        else
            return default;
    }
    public TGridObject GetGridObject(Vector2 position)
    {
        GetXY(position, out int x, out int y);
        return GetGridObject(x, y);
    }
    public int GetWidth() => width;
    public int GetHeight() => height;
    public bool CheckIfInsideGrid(int x, int y) => (x >= 0 && y >= 0 && x < width && y < height);
}