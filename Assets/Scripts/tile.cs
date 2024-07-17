using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public sealed class tile : MonoBehaviour
{
	public int x;
	public int y;

	private  item _item;

	public item item
	{
		get => _item;

		set
		{
			if (_item == value) return;
			_item = value;

			icon.sprite = _item.sprite;
		}
	}


	public Image icon;


	public Button button;

	public tile Left => x > 0 ? board.Instance.tiles[x - 1, y] : null;
    public tile Top => y > 0 ? board.Instance.tiles[x, y - 1] : null;
    public tile Right => x < board.Instance.width -1  ? board.Instance.tiles[x + 1, y] : null;
    public tile Bottom => y < board.Instance.width - 1 ? board.Instance.tiles[x, y + 1] : null;

	public tile[] Neighbours => new[]
	{
		Left, 
		Top, 
		Right, 
		Bottom,
	};

    private void Start() => button.onClick.AddListener(() => board.Instance.Select(this));

	public List<tile> GetConnectedTiles(List<tile> exclude = null)
	{
		var result = new List<tile> { this, };

		if (exclude == null) 
		{
			exclude = new List<tile> { this, };
		}
		else
		{
			exclude.Add(this);
		}

		foreach (var neighbour in Neighbours)
		{
            if ((neighbour == null) || exclude.Contains(neighbour) || neighbour.item != item) continue;

            result.AddRange(neighbour.GetConnectedTiles(exclude));
        }
		return result;	
	}


}
