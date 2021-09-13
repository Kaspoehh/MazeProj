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
    
    public List<GameObject> toDestroyUp1 = new List<GameObject>();
    public List<GameObject> toDestroyBottom1 = new List<GameObject>();
    public List<GameObject> toDestroyRight1 = new List<GameObject>();
    public List<GameObject> toDestroyLeft1 = new List<GameObject>();

    
}
