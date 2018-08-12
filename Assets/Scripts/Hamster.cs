using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamster : MonoBehaviour
{
	private HexCell _hexCell;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Q))
			move(HexCell.Directions.Topright);
		else if(Input.GetKeyUp(KeyCode.W))
			move(HexCell.Directions.Top);
		else if (Input.GetKeyUp(KeyCode.E))
			move(HexCell.Directions.Topleft);
		else if (Input.GetKeyUp(KeyCode.A))
			move(HexCell.Directions.Botright);
		else if (Input.GetKeyUp(KeyCode.S))
			move(HexCell.Directions.Bot);
		else if (Input.GetKeyUp(KeyCode.D))
			move(HexCell.Directions.Botleft);
	}

	public void setCell(HexCell hexCell)
	{
		_hexCell = hexCell;
		if (hexCell != null)
		{
			Vector3 vector3 = new Vector3(0, 0, 0);
			transform.SetParent(hexCell.transform);
			transform.localPosition = vector3;
		}
	}

	public void move(HexCell.Directions direction)
	{
		if (_hexCell != null)
		{
			HexCell cell = _hexCell.GetNeighbour(direction);
			if (cell != null && cell.CanSpawnHamster())
			{
				_hexCell.HamsterLeft();
				cell.SpawnHamster(this);
			}
		}
	}
}
