using UnityEngine;

public class Hamster : MonoBehaviour
{
	private HexCell _hexCell;

	public void SetCell(HexCell hexCell)
	{
		_hexCell = hexCell;
		if (hexCell == null) return;
		var vector3 = new Vector3(0, 0, 0);
		transform.SetParent(hexCell.transform);
		transform.localPosition = vector3;
	}

	public void Move(HexCell.Directions direction)
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
