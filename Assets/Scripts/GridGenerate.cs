using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerate : MonoBehaviour
{
    private System.Random random;
    private HashSet<Vector3Int> selecteds = new HashSet<Vector3Int>();

    public int gridWidth;
    public int gridHeight;

    public Tilemap grid;
    public Tilemap grid2;
    public TileBase tile1;
    public TileBase tile2;
    public TileBase tile3;
    public TileBase tile4;
    public TileBase tile5;
    public TileBase tileSelection;

    void Start()
    {
        random = new System.Random();
        FillGrid();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selecteds.Count != 0)   // eski seçimi siler
            {
                selecteds.ToList().ForEach(t => grid2.SetTile(t, null));
                selecteds.Clear();
            }

            /*** etrafından 3 eleman seçer ***/
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x + 1.2f, pos.y, pos.z)));
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x - 1.2f, pos.y, pos.z)));
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x, pos.y + 1.3f, pos.z)));
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x, pos.y - 1.3f, pos.z)));

            /*** Harita dışındaysa seçimi sil ***/
            selecteds.ToList().ForEach(t => { if (grid.GetTile(t) == null) selecteds.Remove(t); });


            /*** istisnalar ***/
            if (selecteds.Count == 4)   // tam ortasına tıklamışsa olabiliyor.
                selecteds.Remove(selecteds.First());  // bir tanesini silerek düzelttik.
            else if (selecteds.Count < 3)    // köşelerde falan oluyor
                selecteds.Clear();
            else
                selecteds.ToList().ForEach(t => grid2.SetTile(t, tileSelection));  // seçimi uygula
        }
    }

    private void FillGrid()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = -5; j < gridWidth - 5; j++)
            {
                grid.SetTile(new Vector3Int(i, j, 0), GetRandTile());
            }
        }
    }

    private TileBase GetRandTile()
    {
        int a = random.Next(1, 6);
        if (a == 1) return tile1;
        if (a == 2) return tile2;
        if (a == 3) return tile3;
        if (a == 4) return tile4;
        return tile5;
    }
}
