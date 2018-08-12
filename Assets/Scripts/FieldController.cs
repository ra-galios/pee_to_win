using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
	private struct HexMetrics
	{
		public float InnerRadius;
		public float OuterRadius;
	}

	private readonly List<HexCell> _cellsForBonus = new List<HexCell>();
	private List<HexCell>[] directionEdgeCells = new List<HexCell>[(int) HexCell.Directions.Size];
	private readonly List<HexCell> _destroyebleCells = new List<HexCell>();
	private readonly List<HexCell> _cellsForSpawn = new List<HexCell>();

	public int Side = 3;
	public HexCell CellPrefab;
	public Spaceman SpacemanPrefab;
	public Hamster HamsterPrefab;
	public Canvas Canvas;
	
	private HexMetrics _hexMetrics;
	private HexCell[,] _field;

	// Use this for initialization
	void Start () {
		for (var i = 0; i < (int) HexCell.Directions.Size; i++)
		{
			directionEdgeCells[i] = new List<HexCell>();
		}
		
		var cellRect = CellPrefab.GetComponent<RectTransform>();
		_hexMetrics.InnerRadius = cellRect.rect.height / 2f;
		_hexMetrics.OuterRadius = cellRect.rect.width / 2f;
		
		var canvasRect = Canvas.GetComponent<RectTransform>();

		var fieldSide = canvasRect.rect.height;
		
		var hexHeight = (Side + 0.5f) * (_hexMetrics.InnerRadius * 2f);
		var scale = fieldSide / hexHeight;
		Canvas.GetComponent<CanvasScaler>().scaleFactor = scale;
		
		var baseTransform = new Vector2();
		baseTransform.x = - canvasRect.rect.width / 2 / scale;
		baseTransform.y = - canvasRect.rect.height / 2 / scale;

		var width = Side;
		var height = width * 2;
		_field = new HexCell[width, height];
		
		for(int level = 0; level < width; level++)
		{
			int validRowNumber = 0;
			for (int idx = 0; idx < height; idx++)
			{
				HexCell cell = CreateCell(baseTransform, level, idx);
				if (cell != null)
				{
					cell.SetIj(this, _field, level, validRowNumber);
					_field[level, validRowNumber] = cell;
					validRowNumber++;
				}
			}
		}

		var hamster = Instantiate(HamsterPrefab);
		_field[2, 2].SpawnHamster(hamster);
	}

	private HexCell CreateCell(Vector2 baseTransform, int i, int j)
	{
		if (i % 2 != j % 2) return null;
		
		var position = new Vector3(baseTransform.x, baseTransform.y);
		position.x += (i  / 2f * 3f + 1) * _hexMetrics.OuterRadius;
		position.y += (j + 1) * _hexMetrics.InnerRadius;

		var cell = Instantiate(CellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		return cell;
	}

	public void CellAdded(HexCell cell)
	{
		_cellsForBonus.Add(cell);
	    _cellsForSpawn.Add(cell);
	}

	public void CellBonusSpawned(HexCell cell)
	{
		_cellsForBonus.Remove(cell);
	}

	public void CellHamsterSpawend(HexCell cell)
	{
		_cellsForSpawn.Remove(cell);
	}

	public void CellDestroyed(HexCell cell)
	{
		_cellsForBonus.Remove(cell);
		_cellsForSpawn.Remove(cell);
		_destroyebleCells.Remove(cell);
		for (var i = 0; i < (int) HexCell.Directions.Size; i++)
		{
			directionEdgeCells[i].Remove(cell);
		}
	}

	public void СellBecameDirectionEdge(HexCell cell, HexCell.Directions direction)
	{
		directionEdgeCells[(int) direction].Add(cell);
	}

	public void RemoveCellFromDirectionEdges(HexCell cell, HexCell.Directions directions)
	{
		directionEdgeCells[(int) directions].Remove(cell);
	}

	public void СellBecameDestroyeble(HexCell cell)
	{
		_destroyebleCells.Add(cell);
	}

	public void CellHamsterLeft(HexCell hexCell)
	{
		_cellsForSpawn.Add(hexCell);
	}

	public void CellBonusLeft(HexCell hexCell)
	{
		_cellsForBonus.Add(hexCell);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Q))
			Move(HexCell.Directions.Topleft);
		else if(Input.GetKeyUp(KeyCode.W))
			Move(HexCell.Directions.Top);
		else if (Input.GetKeyUp(KeyCode.E))
			Move(HexCell.Directions.Topright);
		else if (Input.GetKeyUp(KeyCode.A))
			Move(HexCell.Directions.Botleft);
		else if (Input.GetKeyUp(KeyCode.S))
			Move(HexCell.Directions.Bot);
		else if (Input.GetKeyUp(KeyCode.D))
			Move(HexCell.Directions.Botright);
	}
	
	private void Move(HexCell.Directions direction)
	{
		List<HexCell> directionalEdges = directionEdgeCells[(int) direction];
		foreach (var cell in directionalEdges)
		{
			cell.MoveHamster(direction);
		}
	}
}
