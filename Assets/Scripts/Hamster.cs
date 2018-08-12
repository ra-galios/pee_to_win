using UnityEngine;

public class Hamster : MonoBehaviour
{
	private HexCell _hexCell;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Q))
			Move(HexCell.Directions.Topright);
		else if(Input.GetKeyUp(KeyCode.W))
			Move(HexCell.Directions.Top);
		else if (Input.GetKeyUp(KeyCode.E))
			Move(HexCell.Directions.Topleft);
		else if (Input.GetKeyUp(KeyCode.A))
			Move(HexCell.Directions.Botright);
		else if (Input.GetKeyUp(KeyCode.S))
			Move(HexCell.Directions.Bot);
		else if (Input.GetKeyUp(KeyCode.D))
			Move(HexCell.Directions.Botleft);
	}

	public void SetCell(HexCell hexCell)
	{
		_hexCell = hexCell;
		if (hexCell == null) return;
		var vector3 = new Vector3(0, 0, 0);
		transform.SetParent(hexCell.transform);
		transform.localPosition = vector3;
	}

	private void Move(HexCell.Directions direction)
	{
		if (_hexCell != null)
		{
			var cell = _hexCell.GetNeighbour(direction);
			if (cell == null || !cell.CanSpawnHamster()) return;
			
			_hexCell.HamsterLeft();
			cell.SpawnHamster(this);
		}
	}
}
