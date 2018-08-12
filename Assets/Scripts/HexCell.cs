using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
	public enum Directions
	{
		Top,Topright,Botright,Bot,Botleft,Topleft,Size,Invalid
	}
	
	private int _i, _j;
	private HexCell[] _neighbours = new HexCell[(int)Directions.Size];
	private Spaceman _spaceman;
	private Hamster _hamster;
	private FieldController _fieldController;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetIj(FieldController fieldController, HexCell[,] field, int i, int j)
	{
		_fieldController = fieldController;
		
		_i = i;
		_j = j;
		_fieldController.cellAdded(this);

		for (var d = 0; d < (int) Directions.Size; d++)
		{
			Vector2 nbrPos = DesideIj((Directions) d);
			if (nbrPos.x >= 0 && nbrPos.x < field.GetLength(0))
			{
				if (nbrPos.y >= 0 && nbrPos.y < field.GetLength(1))
				{
					HexCell cell = field[(int) nbrPos.x, (int) nbrPos.y];
					AddNeighbour(cell);
				}
			}
		}
		
		transform.GetChild(0).gameObject.GetComponent<Text>().text = i + ";" + j;
	}

	private Vector2 DesideIj(Directions direction)
	{
		var ret = new Vector2(-1, -1);
		var top = _j + _i % 2;
		var bot = top - 1;
		
		switch (direction)
		{
			case Directions.Top:
				ret.x = _i;
				ret.y = _j + 1;
				break;
			case Directions.Topright:
				ret.x = _i + 1;
				ret.y = top;
				break;
			case Directions.Botright:
				ret.x = _i + 1;
				ret.y = bot;
				break;
			case Directions.Bot:
				ret.x = _i;
				ret.y = _j - 1;
				break;
			case Directions.Botleft:
				ret.x = _i - 1;
				ret.y = bot;
				break;
			case Directions.Topleft:
				ret.x = _i - 1;
				ret.y = top;
				break;
		}

		return ret;
	}


	private Directions DesideNbr(HexCell cell)
	{
		if (cell.GetI() == _i)
		{
			int vDelta = cell.GetJ() - _j;
			switch (vDelta)
			{
					case 1:
						return Directions.Top;
					case -1:
						return Directions.Bot;
			}
		}
		
		var top = _j + _i % 2;
		var bot = top - 1;
		int horDelta;
		if (cell.GetJ() == top)
		{
			horDelta = _i - cell.GetI();
			switch (horDelta)
			{
				case -1:
					return Directions.Topleft;
				case 1:
					return Directions.Topright;
			}

			return Directions.Invalid;
		}

		if (cell.GetJ() != bot) return Directions.Invalid;

	    horDelta = _i - cell.GetI();
		switch (horDelta)
		{
			case -1:
				return Directions.Botleft;
			case 1:
				return Directions.Botright;
		}

		return Directions.Invalid;

	}

	private int GetI()
	{
		return _i;
	}

	private int GetJ()
	{
		return _j;
	}

	protected void AddNeighbour(HexCell neighbour)
	{
		if (neighbour != null && !_neighbours.Contains(neighbour))
		{
			Directions direction = DesideNbr(neighbour);
			if (direction < Directions.Size)
			{
				_neighbours[(int) direction] = neighbour;
				neighbour.AddNeighbour(this);
			}
		}
	}	

	protected void remove(HexCell neighbour)
	{
		if (neighbour != null && _neighbours.Contains(neighbour))
		{
			Directions direction = DesideNbr(neighbour);
			_neighbours[(int) direction] = null;
			neighbour.remove(this);
			_fieldController.cellDestroyed(this);
		}
	}

	public void AddSpacemnan(Spaceman spaceman)
	{
		if (CanSpawnSpaceman())
		{
			Vector3 pos = new Vector3(0, 0, 0);
			_spaceman = Instantiate(spaceman);
			_spaceman.transform.SetParent(transform);
			_spaceman.transform.localPosition = pos;
			_fieldController.cellBonusSpawned(this);
		}
	}

	public bool CanSpawnSpaceman()
	{
		return _spaceman == null && CanSpawnHamster();
	}

	public void SpawnHamster(Hamster hamster)
	{
		_hamster = hamster;
		hamster.setCell(this);
		_fieldController.cellHamsterSpawend(this);
	}

	public void HamsterLeft()
	{
		_fieldController.cellAdded(this);
		_hamster.transform.SetParent(null);
		_hamster.setCell(null);
		_hamster = null;
	}
	
	public bool CanSpawnHamster()
	{
		return _hamster == null;
	}

	public HexCell GetNeighbour(Directions direction)
	{
		return _neighbours[(int) direction];
	}
}
