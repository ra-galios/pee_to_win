using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FieldController : MonoBehaviour
{
	private struct HexMetrics
	{
		public float innerRadius;
		public float outerRadius;
	}

	private List<HexCell> cellsForBonus = new List<HexCell>();
	private List<HexCell>[] directionEdgeCells = new List<HexCell>[(int) HexCell.Directions.Size];
	private List<HexCell> destroyebleCells = new List<HexCell>();
	private List<HexCell> cellsForSpawn = new List<HexCell>();

	public int side = 3;
	public HexCell cellPrefab;
	public Spaceman spacemanPrefab;
	public Hamster hamsterPrefab;
	public Canvas canvas;
	

	private HexMetrics _hexMetrics;
	private HexCell[,] _field;

	private void Awake()
	{
		
	}

	// Use this for initialization
	void Start () {
		RectTransform cellRect = cellPrefab.GetComponent<RectTransform>();
		_hexMetrics.innerRadius = cellRect.rect.height / 2f;
		_hexMetrics.outerRadius = cellRect.rect.width / 2f;
		
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		float fieldSide = canvasRect.rect.width;
		
		float hexHeight = (1.5f * side - 0.5f) * (_hexMetrics.outerRadius * 2);
		float scale = fieldSide / hexHeight;
		canvas.GetComponent<CanvasScaler>().scaleFactor = scale;
		
		Vector2 baseTransform = new Vector2();
		baseTransform.x = - canvasRect.rect.width / 2 / scale;
		baseTransform.y = - canvasRect.rect.height / 2 / scale;

		int width = side * 2 - 1;
		int height = width * 2;
		_field = new HexCell[width, height];
		
		for(int level = 0; level < width; level++)
		{
			int validRowNumber = 0;
			for (int idx = 0; idx < height; idx++)
			{
				HexCell cell = createCell(baseTransform, level, idx);
				if (cell != null)
				{
					cell.SetIj(this, _field, level, validRowNumber);
					_field[level, validRowNumber] = cell;
					validRowNumber++;
				}
			}
		}

		Hamster hamster = Instantiate(hamsterPrefab);
		_field[5, 5].SpawnHamster(hamster);

		int i = 1 + 2;
	}

	private HexCell createCell(Vector2 baseTransform, int i, int j)
	{
		if (i % 2 == j % 2)
		{
			Vector3 position = new Vector3(baseTransform.x, baseTransform.y);
			position.x += (i  / 2f * 3f + 1) * _hexMetrics.outerRadius;
			position.y += (j + 1) * _hexMetrics.innerRadius;

			HexCell cell = Instantiate<HexCell>(cellPrefab);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			return cell;
		}

		return null;
	}

	public void cellAdded(HexCell cell)
	{
		cellsForBonus.Add(cell);
	    cellsForSpawn.Add(cell);
	}

	public void cellBonusSpawned(HexCell cell)
	{
		cellsForBonus.Remove(cell);
	}

	public void cellHamsterSpawend(HexCell cell)
	{
		cellsForSpawn.Remove(cell);
	}

	public void cellDestroyed(HexCell cell)
	{
		cellsForBonus.Remove(cell);
		cellsForSpawn.Remove(cell);
		destroyebleCells.Remove(cell);
		for (int i = 0; i < (int) HexCell.Directions.Size; i++)
		{
			directionEdgeCells[i].Remove(cell);
		}
	}

	public void cellBecameDirectionEdge(HexCell cell, HexCell.Directions direction)
	{
		directionEdgeCells[(int) direction].Add(cell);
	}

	public void cellBecameDestroyeble(HexCell cell)
	{
		destroyebleCells.Add(cell);
	}

	public void cellHamsterLeft(HexCell hexCell)
	{
		cellsForSpawn.Add(hexCell);
	}

	public void cellBonusLeft(HexCell hexCell)
	{
		cellsForBonus.Add(hexCell);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
