using System.Collections.Generic;
using UnityEngine;

public class LoadNeighbours : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public int index;
    public List<GameObject> toDestroyUp = new List<GameObject>();
    public List<GameObject> toDestroyBottom = new List<GameObject>();
    public List<GameObject> toDestroyRight = new List<GameObject>();
    public List<GameObject> toDestroyLeft = new List<GameObject>();
}
