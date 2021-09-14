using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Data")]
    public int gridSize = 5;
    [SerializeField] private Image unitImage;
    public List<Unit> units = new List<Unit>();
    
    public Dictionary<int, Unit> _units = new Dictionary<int, Unit>();

    [Header("3D")] 
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject blockFinishPrefab;
    [SerializeField] private GameObject mazeParent;
    [SerializeField] private float prefabXSize = 30;
    [SerializeField] private float prefabYSize = 70;

    [Header("Player")] [SerializeField] private GameObject player; 

    private void Start()
    {
        CreateGrid();
    }

    #region Create Maze
    
    private void CreateGrid()
    {
        int index = 0;
        
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Unit unit = new Unit(UnitType.Wall, false);
                unit.Walls = new Walls();
                unit.Walls.wallBottom = true;
                unit.Walls.wallLeft = true;
                unit.Walls.wallRight = true;
                unit.Walls.wallTop = true;
                _units.Add(index, unit);
                index++;
            }
        }
        // for (int i = 0; i < GetPossibleDirections(6).Count; i++)
        // {
        //
        //     Debug.Log(GetPossibleDirections(6)[i]);
        // }
        // StartCoroutine(CreateMaze());
        CreateMaze();
    }

    private void CreateMaze()
    {
        int currentPos = 0;
        List<int> stack = new List<int>();
        stack.Add(currentPos);
        _units.ElementAt(currentPos).Value.UnitType = UnitType.Walkable;

        
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            List<Directions> possibleDirs = new List<Directions>();
            possibleDirs = GetPossibleDirections(currentPos);
            if (possibleDirs.Count > 0)
            {
                int dirIndex = Random.Range(0, possibleDirs.Count);
                Directions dir = possibleDirs[dirIndex];
                
                CreateWall(currentPos, dir);
                
                if (dir == Directions.UP)
                {
                    currentPos+=gridSize;
                }
                if (dir == Directions.DOWN)
                {
                    currentPos-=gridSize;
                }
                if (dir == Directions.RIGHT)
                {
                    currentPos+=1;
                }
                if (dir == Directions.LEFT)
                {
                    currentPos -= 1;
                }
                
                stack.Add(currentPos);
                _units.ElementAt(currentPos).Value.UnitType = UnitType.Walkable;
                    //Debug.Log("currentPos: " + currentPos + " lastPos: " + lastIndex);
                

            }
            else
            {
                // Debug.Log("No directions left for: " + currentPos);
                // Debug.Log(stack.Count);
                stack.Remove(stack.Last());
                if (stack.Count == 0)
                    break;
                currentPos = stack.Last();
                i--;
            }

            //yield return new WaitForSeconds(.1F);            
        }

        for (int i = 0; i < _units.Count; i++)
        {
            units.Add(_units.ElementAt(i).Value);
        } 
        CreateFinish();
        VisualizeMaze();
    }

    private void CreateWall(int indexNew, Directions directionCommingFrom)
    {
        switch (directionCommingFrom)
        {
            case Directions.UP:
                _units.ElementAt(indexNew).Value.Walls.wallTop = false;
                _units.ElementAt(indexNew + gridSize).Value.Walls.wallBottom = false;
                return;
            case Directions.DOWN:
                _units.ElementAt(indexNew - gridSize).Value.Walls.wallTop = false;
                _units.ElementAt(indexNew).Value.Walls.wallBottom = false;
                return;
            case Directions.RIGHT:
                _units.ElementAt(indexNew).Value.Walls.wallRight = false;
                _units.ElementAt(indexNew  + 1).Value.Walls.wallLeft = false;
                return;
            case Directions.LEFT:
                _units.ElementAt(indexNew).Value.Walls.wallLeft = false;
                _units.ElementAt(indexNew - 1).Value.Walls.wallRight = false;
                return;
        }
    }
     
    private List<Directions> GetPossibleDirections(int currentPosIndex)
    {
        List<Directions> possibledirections = Enum.GetValues(typeof(Directions)).OfType<Directions>().ToList();
        List<int> noRight = new List<int>();
        List<int> noLeft = new List<int>();
        for (int i = 0; i < gridSize; i++)
        {
            noRight.Add((i * gridSize) - 1);
        }
        for (int i = 0; i < gridSize; i++)
        {
            noLeft.Add((i * gridSize));
        }
        
        if (noRight.Contains(currentPosIndex))
            possibledirections.Remove(Directions.RIGHT);
        if (noLeft.Contains(currentPosIndex))
            possibledirections.Remove(Directions.LEFT);
        if (!_units.TryGetValue(currentPosIndex + gridSize, out Unit unit) || unit.UnitType == UnitType.Walkable)
            possibledirections.Remove(Directions.UP);
        if (!_units.TryGetValue(currentPosIndex - gridSize, out Unit unit1) || unit1.UnitType == UnitType.Walkable)
            possibledirections.Remove(Directions.DOWN);
        if (!_units.TryGetValue(currentPosIndex + 1, out Unit unit2) || unit2.UnitType == UnitType.Walkable)
            possibledirections.Remove(Directions.RIGHT);
        if (!_units.TryGetValue(currentPosIndex - 1, out Unit unit3) || unit3.UnitType == UnitType.Walkable)
            possibledirections.Remove(Directions.LEFT);


        return possibledirections;
    }
    
    

    #endregion

    #region Vizualize Maze 3D
    
    public void VisualizeMaze()
    {
        int count = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject mazeObj;
                if (units[count].isFinish)
                    mazeObj= Instantiate(blockFinishPrefab, new Vector3(x * prefabXSize, 0, y * prefabYSize), Quaternion.identity);
                else
                    mazeObj = Instantiate(blockPrefab, new Vector3(x * prefabXSize, 0, y * prefabYSize), Quaternion.identity);
                
                
                var nbLoader = mazeObj.GetComponent<LoadNeighbours>();
                nbLoader.index = count;
                nbLoader.mazeGenerator = this;
                // if(count != 0)                
                //     mazeObj.gameObject.SetActive(false);
                mazeObj.transform.SetParent(mazeParent.transform);
                mazeObj.name = count.ToString();
                var Walls = units[count].Walls;
                
                if (!units[count].isFinish)
                {

                    if (units[count].Walls.wallRight)
                        for (int i = 0; i < nbLoader.toDestroyRight.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyRight[i]);
                            nbLoader.toDestroyRight[i].SetActive(false);
                        }
                    if (!units[count].Walls.wallRight)
                        for (int i = 0; i < nbLoader.toDestroyRight1.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyRight1[i]);
                            nbLoader.toDestroyRight1[i].SetActive(false);
                        }

                    if (units[count].Walls.wallLeft)
                        for (int i = 0; i < nbLoader.toDestroyLeft.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyLeft[i]);
                            nbLoader.toDestroyLeft[i].SetActive(false);
                        }
                    if (!units[count].Walls.wallLeft)
                    {
                        for (int i = 0; i < nbLoader.toDestroyLeft1.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyLeft1[i]);
                            nbLoader.toDestroyLeft1[i].SetActive(false);
                        }
                    }
                    if (units[count].Walls.wallTop)
                        for (int i = 0; i < nbLoader.toDestroyUp.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyUp[i]);
                            nbLoader.toDestroyUp[i].SetActive(false);
                        }
                    if (!units[count].Walls.wallTop)
                        for (int i = 0; i < nbLoader.toDestroyUp1.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyUp1[i]);
                            nbLoader.toDestroyUp1[i].SetActive(false);
                        }
                    if (units[count].Walls.wallBottom)
                        for (int i = 0; i < nbLoader.toDestroyBottom.Count; i++)
                        {
                            //Destroy(nbLoader.toDestroyBottom[i]);
                            nbLoader.toDestroyBottom[i].SetActive(false);
                        }
                    if (!units[count].Walls.wallBottom)
                        for (int i = 0; i < nbLoader.toDestroyBottom1.Count; i++)
                        {
                            // Destroy(nbLoader.toDestroyBottom1[i]);
                            nbLoader.toDestroyBottom1[i].SetActive(false);
                        }
                }

                // if(count != 0)
                //     mazeObj.SetActive(false);
                count++;
            }
        }        
    }
    
    
    #endregion

    #region makefinish

    private void CreateFinish()
    {
        // for (int i = 0; i < GetPossibleDirectionsWalkable(0).Count; i++)
        // {
        //     Debug.Log(GetPossibleDirectionsWalkable(0)[i]);
        // }

        
        int currentPos = 0;
        List<int> stack = new List<int>();
        stack.Add(currentPos);
        _units.ElementAt(currentPos).Value.UnitType = UnitType.Walkable;
        int lastpos = 0;
        
        for (int i = 0; i < 10000; i++)
        {
            List<Directions> possibleDirs = new List<Directions>();
            possibleDirs = GetPossibleDirectionsWalkable(currentPos, stack);
            
            if (possibleDirs.Count > 0)
            {
                List<int> testStack = stack;
                testStack.Add(currentPos);

                int dirIndex = Random.Range(0, possibleDirs.Count);
                Directions dir = possibleDirs[dirIndex];
                
                int newPos = currentPos;
                
                if (dir == Directions.UP)
                {
                    newPos+=gridSize;
                }
                if (dir == Directions.DOWN)
                {
                    newPos-=gridSize;
                }
                if (dir == Directions.RIGHT)
                {
                    newPos+=1;
                }
                if (dir == Directions.LEFT)
                {
                    newPos -= 1;
                }
                
                currentPos = newPos;
                stack.Add(currentPos);
                //_units.ElementAt(currentPos).Value.UnitType = UnitType.Walkable;
                //Debug.Log("currentPos: " + currentPos + " lastPos: " + lastIndex);
            }
            else
            {
                Debug.Log(GetPossibleDirectionsWalkable(currentPos, stack).Count);
                _units.ElementAt(currentPos).Value.isFinish = true;
                Debug.Log("No dirs left end pos: " + currentPos);
            }
        }
        Debug.Log("for loop done fu");
        
        player.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private List<Directions> GetPossibleDirectionsWalkable(int currentPosIndex, List<int> stack)
    {
        List<Directions> possibledirections = Enum.GetValues(typeof(Directions)).OfType<Directions>().ToList();
        List<int> noRight = new List<int>();
        List<int> noLeft = new List<int>();

        for (int i = 0; i < gridSize; i++)
        {
            noRight.Add((i * gridSize) - 1);
        }
        for (int i = 0; i < gridSize; i++)
        {
            noLeft.Add((i * gridSize));
        }
        
        if(stack.Contains(currentPosIndex + 1))
            possibledirections.Remove(Directions.RIGHT);

        if(stack.Contains(currentPosIndex - 1))
            possibledirections.Remove(Directions.LEFT);
        
        if(stack.Contains(currentPosIndex + gridSize))
            possibledirections.Remove(Directions.UP);
        
        if(stack.Contains(currentPosIndex - gridSize))
            possibledirections.Remove(Directions.DOWN);
        
        if (noRight.Contains(currentPosIndex))
            possibledirections.Remove(Directions.RIGHT);
        
        if (noLeft.Contains(currentPosIndex))
            possibledirections.Remove(Directions.LEFT);
        
        if(_units.ElementAt(currentPosIndex).Value.Walls.wallTop)
            possibledirections.Remove(Directions.UP);
        
        if(_units.ElementAt(currentPosIndex).Value.Walls.wallBottom)
            possibledirections.Remove(Directions.DOWN);
        
        if(_units.ElementAt(currentPosIndex).Value.Walls.wallRight)
            possibledirections.Remove(Directions.RIGHT);
        
        if(_units.ElementAt(currentPosIndex).Value.Walls.wallLeft)
            possibledirections.Remove(Directions.LEFT);

        
        return possibledirections;
    }

    #endregion

 
    
    
    
}
public enum Directions
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

[Serializable]
public class Unit
{
    //private UnitPos unitPos;
    public UnitType UnitType;
    public Walls Walls;
    public bool isFinish = false;
    
    public Unit(/*UnitPos unitPos,*/  UnitType unitType, bool isFinish)
    {
        //this.unitPos = unitPos;
        this.UnitType = unitType;
        this.isFinish = isFinish;
    }
}
[Serializable]
public enum UnitType
{
    Wall,
    Walkable
}

[Serializable]
public class Walls
{
    public bool wallLeft = true;
    public bool wallRight = true;
    public bool wallTop = true;
    public bool wallBottom = true;
    
}

public class UnitPos
{
    public int X;
    public int Y;
}
