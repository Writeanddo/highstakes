using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomGrid : MonoBehaviour {
	public static CustomGrid Instance;
	public Tilemap groundGrid;
	public Vector2Int Size;
	Unit[,] units;

	// Prioritized being called before anything else
	void Awake() {
		units = new Unit[Size.x, Size.y];
		for (int i = 0; i < Size.x; i++)
		for (int j = 0; j < Size.y; j++)
			units[i,j] = null;
		Instance = this;
	}

	public bool ValidSquare(int x, int y) => (x >= 0 && x < Size.x && y >= 0 && y < Size.y);
	public bool ValidSquare(Vector2Int pos) => ValidSquare(pos.x, pos.y);

	public bool CanMoveTo(int x, int y) => ValidSquare(x,y); // for now
	public bool CanMoveTo(Vector2Int pos) => ValidSquare(pos);

	public bool CanSeeThrough(int x, int y) => ValidSquare(x, y) && !HasUnitAt(x, y); // TODO: Change after implementing obstacles;
	public bool CanSeeThrough(Vector2Int pos) => CanSeeThrough(pos.x, pos.y);

	public bool HasUnitAt(int x, int y) => ValidSquare(x, y) ? units[x,y] : false;
	public bool HasUnitAt(Vector2Int pos) => HasUnitAt(pos.x, pos.y);

	public Unit GetUnitAt(int x, int y) => ValidSquare(x, y) ? units[x,y] : null;
	public Unit GetUnitAt(Vector2Int pos) => GetUnitAt(pos.x, pos.y);

	public void AddUnit(Vector2Int pos, Unit unit) {
		if (!ValidSquare(pos)) throw new Exception(pos + " not in range of grid of size" + Size);
		units[pos.x, pos.y] = unit;
	}
	public void RemoveUnit(Vector2Int pos) => units[pos.x, pos.y] = null;
	public void RemoveUnit(Unit unit) => RemoveUnit(unit.pos);

	public IEnumerator MoveUnit(Vector2Int src, Vector2Int dest) {
		if (!ValidSquare(src)) throw new Exception(src + " not in range of grid of size" + Size);
		if (!ValidSquare(dest)) throw new Exception(dest + " not in range of grid of size" + Size);

		Unit unit = GetUnitAt(src);
		if (unit == null) {
			yield break;
		}
		if (HasUnitAt(dest)) yield return StartCoroutine(unit.Capture(GetUnitAt(dest)));
		else yield return StartCoroutine(unit.MoveTo(dest));

		units[src.x, src.y] = null;
		unit.pos = dest;
		units[dest.x, dest.y] = unit;
	}

	// Raycast Detection
	public Vector3 GetMouseWorldPosition() {
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		Plane plane = new Plane();
		plane.SetNormalAndPosition(Vector3.up, transform.position);
		float dist;
		if (plane.Raycast(ray, out dist)) {
			Vector3 hitPoint = ray.GetPoint(dist);
			return hitPoint;
		}
		return Vector3.zero;
	}

	public Vector2Int GetMouseGridPosition() => SnapCoordinate(GetMouseWorldPosition());

	public Vector2Int SnapCoordinate(Vector3 position) {
		return (Vector2Int) groundGrid.WorldToCell(position);
	}

	public Vector3 GridToWorld(Vector3Int pos) {
		return groundGrid.CellToWorld(pos);
	}
}