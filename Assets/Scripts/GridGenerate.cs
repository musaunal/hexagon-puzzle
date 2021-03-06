﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
// using UnityScript.Steps;

public class GridGenerate : MonoBehaviour
{
    private System.Random random;
    private HashSet<Vector3Int> selecteds = new HashSet<Vector3Int>();
    public SwipeController swipe;
    public TMP_Text score;
    public TMP_Text bombTime;
    public TMP_Text bombText;
    private bool isBomb = false;
    private int bombCount = 1;
    private int colorCount = 5;
    private bool colorChange = false;

    public int gridWidth;
    public int gridHeight;

    public Tilemap grid;
    public Tilemap grid2;
    public TileBase tile1;
    public TileBase tile2;
    public TileBase tile3;
    public TileBase tile4;
    public TileBase tile5;
    public TileBase tileReserve;
    public TileBase tileBomb;
    public TileBase tileSelection;

    void Start()
    {
        random = new System.Random();
        FillGrid();
    }

    void Update()
    {
        if (swipe.Tap)
        {
            if (selecteds.Count != 0)   // eski seçimi siler
            {
                ClearSelecteds();
            }

            /*** etrafından 3 eleman seçer ***/
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x - 1.2f, pos.y, pos.z)));
            selecteds.Add(grid.WorldToCell(new Vector3(pos.x + 1.2f, pos.y, pos.z)));
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

        else if (selecteds.Count > 0 && (swipe.SwipeLeft || swipe.SwipeRight))
        {
            StartCoroutine(SwapRoutine());
        }

        if (IsGameEnd())
        {
            back();
        }
    }

    void OnEnable()
    {
        gridWidth = PlayerPrefs.GetInt("width");
        gridHeight = PlayerPrefs.GetInt("height");
        colorChange = PlayerPrefs.GetInt("changeColor") == 1;
        colorCount = PlayerPrefs.GetInt("colorNum") +1;
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

        /*** açılışta patlama olmaması için Aynı renkli üçlüleri düzeltir ***/
        HashSet<Vector3Int> sameColorGroups = ChechkAllGrid();
        while (sameColorGroups.Count != 0)
        {
            sameColorGroups.ToList().ForEach(t => grid.SetTile(t, GetRandTile()));
            sameColorGroups = ChechkAllGrid();
        }
    }

    private TileBase GetRandTile()
    {
        if (Int32.Parse(score.text) > 1000 * bombCount)
        {
            bombCount++;
            bombTime.gameObject.SetActive(true);
            bombTime.text = "" + 5;
            bombText.gameObject.SetActive(true);
            isBomb = true;
            tileBomb.name = "Magma";
            return tileBomb;
        }
        int a = random.Next(1, colorCount);
        if (a == 1) return tile1;
        else if (a == 2) return tile2;
        else if (a == 3) return tile3;
        else if (a == 4) return tile4;
        else if (colorChange) return tileReserve;
        else return tile5;
    }


    private IEnumerator SwapRoutine()
    {
        bool isRight = false;
        bool isMatchOccur = false;
        if (swipe.SwipeRight)   // Yönü belirle
        {
            isRight = true;
        }

        for (int i = 0; i < 3; i++)     // döndürme işlemi
        {
            Swap(isRight);
            yield return new WaitForSeconds(1);         // 1 saniye bekletiyoruz ki oyuncu görsün 
            List<Vector3Int> matcheds = ChechkAllGrid().ToList();
            bool isBreak = false;
            while (matcheds.Count() != 0)
            {
                isMatchOccur = true;
                isBreak = true;
                ClearSelecteds();
                matcheds.ForEach(t => grid.SetTile(t, null));
                score.text = "" + (Int32.Parse(score.text) + 5 * matcheds.Count);
                while (StillBlanksLeft().Count > 0)
                    FillBlanks(matcheds);
                matcheds = ChechkAllGrid().ToList();
            }
            if (isBreak)
                break;
        }

        if (isMatchOccur && isBomb)
        {
            if (IsBombDestroyed())
            {
                bombText.gameObject.SetActive(false);
                bombTime.gameObject.SetActive(false);
                bombTime.text = "" + 0;
                isBomb = false;
            }
            else
            {
                int timeleft = Int32.Parse(bombTime.text);
                if (timeleft == 1)                          // Game Over
                    back();
                bombTime.text = "" + --timeleft;
            }
        }
    }

    private bool IsBombDestroyed()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = -5; j < gridWidth - 5; j++)
            {
                if (grid.GetTile(new Vector3Int(i, j, 0)) == tileBomb)
                    return false;
            }
        }
        return true;
    }

    private void Swap(bool isRight)
    {
        int[] indexes = DifferenciateHexagons(selecteds.ToList());
        TileBase temp = grid.GetTile(selecteds.ElementAt(indexes[0]));
        if (isRight)
        {
            if (indexes[3] == 1)
            {
                grid.SetTile(selecteds.ElementAt(indexes[0]), grid.GetTile(selecteds.ElementAt(indexes[1])));
                grid.SetTile(selecteds.ElementAt(indexes[1]), grid.GetTile(selecteds.ElementAt(indexes[2])));
                grid.SetTile(selecteds.ElementAt(indexes[2]), temp);
            }
            else
            {
                grid.SetTile(selecteds.ElementAt(indexes[0]), grid.GetTile(selecteds.ElementAt(indexes[2])));
                grid.SetTile(selecteds.ElementAt(indexes[2]), grid.GetTile(selecteds.ElementAt(indexes[1])));
                grid.SetTile(selecteds.ElementAt(indexes[1]), temp);
            }
        }
        else
        {
            if (indexes[3] == 1)
            {
                grid.SetTile(selecteds.ElementAt(indexes[0]), grid.GetTile(selecteds.ElementAt(indexes[2])));
                grid.SetTile(selecteds.ElementAt(indexes[2]), grid.GetTile(selecteds.ElementAt(indexes[1])));
                grid.SetTile(selecteds.ElementAt(indexes[1]), temp);
            }
            else
            {
                grid.SetTile(selecteds.ElementAt(indexes[0]), grid.GetTile(selecteds.ElementAt(indexes[1])));
                grid.SetTile(selecteds.ElementAt(indexes[1]), grid.GetTile(selecteds.ElementAt(indexes[2])));
                grid.SetTile(selecteds.ElementAt(indexes[2]), temp);
            }
        }

    }

    private void FillBlanks(List<Vector3Int> list)
    {
        ILookup<int, Vector3Int> lookup = list.ToLookup(t => t.y);  // boşlukları bulunduğu kolonlara göre grupladık

        foreach (var item in lookup)    // her kolon için ayrı ayrı aşağı kaydırma yaptık
        {
            List<Vector3Int> values = item.ToList();

            int smallest = 8;
            int index = 0;
            for (int i = 0; i < values.Count; i++)  // find smallest y valued index
            {
                if (values.ElementAt(i).x < smallest)
                {
                    index = i;
                    smallest = values.ElementAt(i).x;
                }
            }

            for (int i = values.ElementAt(index).x; i < gridHeight; i++)
            {
                grid.SetTile(new Vector3Int(i, item.Key, 0), grid.GetTile(new Vector3Int(i + values.Count, item.Key, 0)));
            }
        }

        foreach (var item in lookup)    // üstteki boşlukları rastgele hexagonlar ile doldur
        {
            List<Vector3Int> values = item.ToList();
            for (int i = 0; i < values.Count; i++)
            {
                grid.SetTile(new Vector3Int(gridHeight - i - 1, item.Key, 0), GetRandTile());
            }
        }
    }

    private List<Vector3Int> StillBlanksLeft()  // traverse grid search blank tiles
    {
        List<Vector3Int> blanks = new List<Vector3Int>();
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = -5; j < gridWidth - 5; j++)
            {
                var a = new Vector3Int(i, j, 0);
                if (grid.GetTile(a) == null)
                    blanks.Add(a);
            }
        }
        return blanks;
    }

    private void ClearSelecteds()
    {
        selecteds.ToList().ForEach(t => grid2.SetTile(t, null));
        selecteds.Clear();
    }

    /* 0: üstteki hex
     * 1: alttaki hex
     * 2: sağdaki veya soldaki hex
     * 3: "2" nolu hex sağda ise 1 solda ise 0
     */
    private int[] DifferenciateHexagons(List<Vector3Int> list)
    {
        int[] indexes = new int[4];
        if (list.ElementAt(0).y == list.ElementAt(1).y)
        {
            indexes[2] = 2;
            indexes[3] = list.ElementAt(2).y > list.ElementAt(0).y ? 1 : 0;
            if (list.ElementAt(0).x > list.ElementAt(1).x)
            {
                indexes[0] = 0;
                indexes[1] = 1;
            }
            else
            {
                indexes[0] = 1;
                indexes[1] = 0;
            }
        }
        else if (list.ElementAt(0).y == list.ElementAt(2).y)
        {
            indexes[2] = 1;
            indexes[3] = list.ElementAt(1).y > list.ElementAt(0).y ? 1 : 0;
            if (list.ElementAt(0).x > list.ElementAt(2).x)
            {
                indexes[0] = 0;
                indexes[1] = 2;
            }
            else
            {
                indexes[0] = 2;
                indexes[1] = 0;
            }
        }
        else
        {
            indexes[2] = 0;
            indexes[3] = list.ElementAt(0).y > list.ElementAt(1).y ? 1 : 0;
            if (list.ElementAt(1).x > list.ElementAt(2).x)
            {
                indexes[0] = 1;
                indexes[1] = 2;
            }
            else
            {
                indexes[0] = 2;
                indexes[1] = 1;
            }
        }
        return indexes;
    }

    /*
     * Aşağıdan Yukarıya traverse eder 
     * Eğer üst üste iki taş aynı renk ise
     * sağındaki ve solundakine bakar 
     * onlardan herhangi biri aynı ise 
     * liste ekleyip en son bunları döner
     * */
    private HashSet<Vector3Int> ChechkAllGrid()
    {
        String prev = null;
        String curr = null;

        Vector3Int right = new Vector3Int(0, 0, 0);
        Vector3Int left = new Vector3Int(0, 0, 0);
        HashSet<Vector3Int> trios = new HashSet<Vector3Int>();

        for (int j = -5; j < gridWidth - 5; j++)    // aşağıdan yukarıya traverse
        {
            prev = grid.GetTile(new Vector3Int(0, j, 0))?.name;
            for (int i = 1; i < gridHeight; i++)
            {
                curr = grid.GetTile(new Vector3Int(i, j, 0))?.name;
                if (prev == curr)
                {
                    if (j != gridWidth - 6 && j % 2 != 0)       // sağdakini taş
                        right.Set(i, j + 1, 0);
                    else
                        right.Set(i - 1, j + 1, 0);

                    if (j != -5 && j % 2 != 0)      // soldaki taş
                        left.Set(i, j - 1, 0);
                    else
                        left.Set(i - 1, j - 1, 0);

                    if (j != gridWidth - 6 && curr == grid.GetTile(right)?.name)
                    {
                        trios.Add(new Vector3Int(i, j, 0));
                        trios.Add(right);
                        trios.Add(new Vector3Int(i - 1, j, 0));
                    }

                    if (j != -5 && curr == grid.GetTile(left)?.name)
                    {
                        trios.Add(new Vector3Int(i, j, 0));
                        trios.Add(left);
                        trios.Add(new Vector3Int(i - 1, j, 0));
                    }

                }
                prev = curr;
            }
        }
        return trios;
    }

    private bool IsGameEnd()
    {
        String prev = null;
        String curr = null;

        Vector3Int right = new Vector3Int(0, 0, 0);
        Vector3Int left = new Vector3Int(0, 0, 0);

        for (int j = -5; j < gridWidth - 5; j++)    // aşağıdan yukarıya traverse
        {
            prev = grid.GetTile(new Vector3Int(0, j, 0))?.name;
            for (int i = 1; i < gridHeight; i++)
            {
                curr = grid.GetTile(new Vector3Int(i, j, 0))?.name;
                if (prev == curr)
                {
                    if (j != gridWidth - 6 && j % 2 != 0)       // sağdakini taş
                        right.Set(i, j + 1, 0);
                    else
                        right.Set(i - 1, j + 1, 0);

                    if (j != -5 && j % 2 != 0)      // soldaki taş
                        left.Set(i, j - 1, 0);
                    else
                        left.Set(i - 1, j - 1, 0);

                    if (j != gridWidth - 6)       // right'tın komşuları arasında istediğimiz taş var mı ?
                    {
                        if (grid.GetTile(new Vector3Int(right.x + 1, right.y, 0))?.name == curr) return false;
                        if (grid.GetTile(new Vector3Int(right.x - 1, right.y, 0))?.name == curr) return false;
                        if (grid.GetTile(new Vector3Int(right.x, right.y + 1, 0))?.name == curr) return false;
                        if (j % 2 != 0)         // Odd column 
                        {
                            if (grid.GetTile(new Vector3Int(right.x + 1, right.y + 1, 0))?.name == curr) return false;
                        }
                        else
                        {
                            if (grid.GetTile(new Vector3Int(right.x - 1, right.y + 1, 0))?.name == curr) return false;
                        }
                    }

                    if (j != -5 && curr == grid.GetTile(left)?.name)    // left'in komşuları arasında istediğimiz taş var mı ?
                    {
                        if (grid.GetTile(new Vector3Int(right.x + 1, right.y, 0))?.name == curr) return false;
                        if (grid.GetTile(new Vector3Int(right.x - 1, right.y, 0))?.name == curr) return false;
                        if (grid.GetTile(new Vector3Int(right.x, right.y - 1, 0))?.name == curr) return false;
                        if (j % 2 != 0)         // Odd column 
                        {
                            if (grid.GetTile(new Vector3Int(right.x + 1, right.y - 1, 0))?.name == curr) return false;
                        }
                        else
                        {
                            if (grid.GetTile(new Vector3Int(right.x - 1, right.y - 1, 0))?.name == curr) return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void setHeight(string s)
    {
        gridHeight = Int32.Parse(s);
    }

    public void setWidth(string s)
    {
        gridWidth = Int32.Parse(s);
    }

    public void back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
