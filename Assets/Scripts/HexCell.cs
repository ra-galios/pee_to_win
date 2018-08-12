using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.Experimental.UIElements.Slider;

public class HexCell : MonoBehaviour
{
	public enum Directions
	{
		Top,Topright,Botright,Bot,Botleft,Topleft,Size,Invalid
	}
	
	private int _i, _j;
	private readonly HexCell[] _neighbours = new HexCell[(int)Directions.Size];
	private Spaceman _spaceman;
	private Hamster _hamster;
	private FieldController _fieldController;

	public void SetIj(FieldController fieldController, HexCell[,] field, int i, int j)
	{
		_fieldController = fieldController;
		
		_i = i;
		_j = j;
		_fieldController.CellAdded(this);

		for (var d = 0; d < (int) Directions.Size; d++)
		{
			var nbrPos = DesideIj((Directions) d);
			HexCell cell = null;
			if (nbrPos.x >= 0 && nbrPos.x < field.GetLength(0))
			{
				if (nbrPos.y >= 0 && nbrPos.y < field.GetLength(1))
				{
					cell = field[(int) nbrPos.x, (int) nbrPos.y];
					AddNeighbour(cell, (Directions) d);
				}
			}
			
			if (cell == null)
			{
				fieldController.СellBecameDirectionEdge(this, (Directions) d);
			}
		}
		
		transform.GetChild(0).gameObject.GetComponent<Text>().text = i + ";" + j;
	}

	private Directions GetOpositeDirection(Directions d)
	{
		switch (d)
		{
			case Directions.Top:
				return Directions.Bot;
			case Directions.Topleft:
				return Directions.Botright;
			case Directions.Topright:
				return Directions.Botleft;
			case Directions.Bot:
				return Directions.Top;
			case Directions.Botleft:
				return Directions.Topright;
			case Directions.Botright:
				return Directions.Topleft;
			default:
				return Directions.Invalid;
		}
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

	protected void AddNeighbour(HexCell neighbour, Directions direction)
	{
		if (neighbour != null && !_neighbours.Contains(neighbour))
		{
			if(_neighbours[(int) direction] == null)
				_fieldController.RemoveCellFromDirectionEdges(this, direction);
				
			_neighbours[(int) direction] = neighbour;
			neighbour.AddNeighbour(this, GetOpositeDirection(direction));
		}
	}	

	protected void Remove(HexCell neighbour)
	{
		if (neighbour != null && _neighbours.Contains(neighbour))
		{
			Directions direction = DesideNbr(neighbour);
			_neighbours[(int) direction] = null;
			neighbour.Remove(this);
			_fieldController.CellDestroyed(this);
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
			_fieldController.CellBonusSpawned(this);
		}
	}

	public bool CanSpawnSpaceman()
	{
		return _spaceman == null && CanSpawnHamster();
	}

	public void SpawnHamster(Hamster hamster)
	{
		_hamster = hamster;
		hamster.SetCell(this);
		_fieldController.CellHamsterSpawend(this);
	}

	public void HamsterLeft()
	{
		_fieldController.CellAdded(this);
		_hamster.transform.SetParent(null);
		_hamster.SetCell(null);
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

	public void MoveHamster(Directions direction)
	{
		if (_hamster != null)
			_hamster.Move(direction);

		HexCell next = GetNeighbour(GetOpositeDirection(direction));
		if(next != null)
			next.MoveHamster(direction);
	}
}
