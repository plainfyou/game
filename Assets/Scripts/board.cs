using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;
using DG.Tweening;
using System;

public sealed class board : MonoBehaviour
{
    public static board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;

    [SerializeField] private AudioSource audioSource;


    public Row[] rows;

    public tile[,] tiles { get; private set; }
    
    public int width => tiles.GetLength(0);
    public int height => tiles.GetLength(1);

    private readonly List<tile> _selection = new List<tile>();

    private const float TweenDuration = 0.25f;

    private void Awake() => Instance = this;

    private void Start()
    {
        tiles = new tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                tile.item = itemDatabase.items[UnityEngine.Random.Range(0, itemDatabase.items.Length)];
                //tile.item = itemDatabase.items[Random.Range(0, itemDatabase.items.Length)];


                tiles[x, y] = tile;
            }
        }
    }

    public async void Select(tile tile)
    { 
        if (!_selection.Contains(tile))

        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        }

        if (_selection.Count > 2) return;

        Debug.Log(message:$"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }

        _selection.Clear();
    }

    public async Task Swap(tile tile1,tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration).SetEase(Ease.OutBack))
                    .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration).SetEase(Ease.OutBack));

        await sequence.Play()
                      .AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.item;

        tile1.item = tile2.item;

        tile2.item = tile1Item;
    }

    private bool CanPop()
    {
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                if (tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                    return true;

        return false; 
    }

    private async void Pop()
    {
        for (var y = 0; y < height; y++)
        {
            for (var x  = 0; x < width; x++)
            {
                var tile = tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles) deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                audioSource.PlayOneShot(collectSound);

                Score_Counter.Instance.Score += tile.item.value * connectedTiles.Count;

                await deflateSequence.Play()
                                     .AsyncWaitForCompletion();



                var inflatesequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.item = itemDatabase.items[UnityEngine.Random.Range(0,itemDatabase.items.Length)];
                    //connectedTile.item = itemDatabase.items[Random.Range(0, itemDatabase.items.Length)];

                    inflatesequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }


                await inflatesequence.Play() 
                                     .AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
    }
    
}
