using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
	protected enum NBRS
	{
		TOP,TOPRIGHT,BOTRIGHT,BOT,BOTLEFT,TOPLEFT,SIZE,INVALID
	}
	
	private int _i, _j;
	private HexCell[] _neighbours = new HexCell[(int)NBRS.SIZE];
	private Spaceman _spaceman;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setIJ(HexCell[,] field, int i, int j)
	{
		this._i = i;
		this._j = j;

		for (int d = 0; d < (int) NBRS.SIZE; d++)
		{
			Vector2 nbrPos = desideIJ((NBRS) d);
			if (nbrPos.x >= 0 && nbrPos.x < field.Rank)
			{
				if (nbrPos.y >= 0 && nbrPos.y < field.GetLength((int) nbrPos.x))
				{
					HexCell cell = field[(int) nbrPos.x, (int) nbrPos.y];
					if (cell != null)
					{
						cell.addNeighbour(this);
					}
				}
			}
		}
		
		transform.GetChild(0).gameObject.GetComponent<Text>().text = i + ";" + j;
	}

	protected Vector2 desideIJ(NBRS direction)
	{
		Vector2 ret = new Vector2(-1, -1);
		int top = _j + _i % 2;
		int bot = top - 1;
		
		switch (direction)
		{
			case NBRS.TOP:
				ret.x = _i;
				ret.y = _j + 1;
				break;
			case NBRS.TOPRIGHT:
				ret.x = _i + 1;
				ret.y = top;
				break;
			case NBRS.BOTRIGHT:
				ret.x = _i + 1;
				ret.y = bot;
				break;
			case NBRS.BOT:
				ret.x = _i;
				ret.y = _j - 1;
				break;
			case NBRS.BOTLEFT:
				ret.x = _i - 1;
				ret.y = bot;
				break;
			case NBRS.TOPLEFT:
				ret.x = _i - 1;
				ret.y = top;
				break;
		}

		return ret;
	}
	
	
	protected NBRS desideNBR(HexCell cell)
	{
		if (cell.getI() == _i)
		{
			int vDelta = cell.getJ() - _j;
			switch (vDelta)
			{
					case 1:
						return NBRS.TOP;
					case 2:
						return NBRS.BOT;
			}
		}
		
		int top = _j + _i % 2;
		int bot = top - 1;
		if (cell.getJ() == top)
		{
			int horDelta = _i - cell.getI();
			if (horDelta == -1)
				return NBRS.TOPLEFT;
			if (horDelta == 1)
				return NBRS.TOPRIGHT;
		
			return NBRS.INVALID;
		}

		if (cell.getJ() == bot)
		{
			int horDelta = _i - cell.getI();
			if (horDelta == -1)
				return NBRS.BOTLEFT;
			
			if (horDelta == 1)
				return NBRS.BOTRIGHT;
			
			return NBRS.INVALID;
		}

		return NBRS.INVALID;
	}

	protected int getI()
	{
		return _i;
	}

	protected int getJ()
	{
		return _j;
	}

	protected void addNeighbour(HexCell neighbour)
	{
		if (neighbour != null && !_neighbours.Contains(neighbour))
		{
			NBRS direction = desideNBR(neighbour);
			if (direction < NBRS.SIZE)
			{
				_neighbours[(int) direction] = neighbour;
				neighbour.addNeighbour(this);
			}
		}
	}

	protected void remove(HexCell neighbour)
	{
		if (neighbour != null && _neighbours.Contains(neighbour))
		{
			NBRS direction = desideNBR(neighbour);
			_neighbours[(int) direction] = null;
			neighbour.remove(this);
		}
	}

	public void addMonster()
	{
		
	}

	public void addSpacemnan(Spaceman spaceman)
	{
		Vector3 pos = new Vector3(0, 0, 0);
		_spaceman = Instantiate(spaceman);
		_spaceman.transform.SetParent(transform);
		_spaceman.transform.localPosition = pos;
	}

	public bool canSpawnSpaceman()
	{
		return _spaceman == null;
	}
}
