using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
	protected enum NBRS
	{
		TOP,TOPRIGHT,BOTRIGHT,BOT,BOTLEFT,TOPLEFT,SIZE,INVALID
	}
	
	private int i, j;
	private HexCell[] neighbours = new HexCell[(int)NBRS.SIZE];
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setIJ(HexCell[,] field, int i, int j)
	{
		this.i = i;
		this.j = j;

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
		int top = j + i % 2;
		int bot = top - 1;
		
		switch (direction)
		{
			case NBRS.TOP:
				ret.x = i;
				ret.y = j + 1;
				break;
			case NBRS.TOPRIGHT:
				ret.x = i + 1;
				ret.y = top;
				break;
			case NBRS.BOTRIGHT:
				ret.x = i + 1;
				ret.y = bot;
				break;
			case NBRS.BOT:
				ret.x = i;
				ret.y = j - 1;
				break;
			case NBRS.BOTLEFT:
				ret.x = i - 1;
				ret.y = bot;
				break;
			case NBRS.TOPLEFT:
				ret.x = i - 1;
				ret.y = top;
				break;
		}

		return ret;
	}
	
	
	protected NBRS desideNBR(HexCell cell)
	{
		if (cell.getI() == i)
		{
			int vDelta = cell.getJ() - j;
			switch (vDelta)
			{
					case 1:
						return NBRS.TOP;
					case 2:
						return NBRS.BOT;
			}
		}
		
		int top = j + i % 2;
		int bot = top - 1;
		if (cell.getJ() == top)
		{
			int horDelta = i - cell.getI();
			if (horDelta == -1)
				return NBRS.TOPLEFT;
			if (horDelta == 1)
				return NBRS.TOPRIGHT;
		
			return NBRS.INVALID;
		}

		if (cell.getJ() == bot)
		{
			int horDelta = i - cell.getI();
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
		return i;
	}

	protected int getJ()
	{
		return j;
	}

	protected void addNeighbour(HexCell neighbour)
	{
		if (neighbour != null && !neighbours.Contains(neighbour))
		{
			NBRS direction = desideNBR(neighbour);
			if (direction < NBRS.SIZE)
			{
				neighbours[(int) direction] = neighbour;
				neighbour.addNeighbour(this);
			}
		}
	}

	protected void remove(HexCell neighbour)
	{
		if (neighbour != null && neighbours.Contains(neighbour))
		{
			NBRS direction = desideNBR(neighbour);
			neighbours[(int) direction] = null;
			neighbour.remove(this);
		}
	}
}
