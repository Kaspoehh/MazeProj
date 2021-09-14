using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class SaveLoadMaze : MonoBehaviour
{
    [SerializeField] private string saveName;
    [ContextMenu("Save")]
    public void Save()
    {
        MazeGenerator _mazeGenerator = this.GetComponent<MazeGenerator>();
        var setting = new Newtonsoft.Json.JsonSerializerSettings();
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        File.WriteAllText(string.Concat(Application.dataPath, saveName), JsonConvert.SerializeObject(_mazeGenerator._units, setting));
    }
        

    [ContextMenu("Load")]
    public void Load()
    {  
        MazeGenerator _mazeGenerator = this.GetComponent<MazeGenerator>();
        _mazeGenerator._units = JsonConvert.DeserializeObject<Dictionary<int, Unit>>
            (File.ReadAllText(string.Concat(Application.dataPath, saveName)));
        for (int i = 0; i < _mazeGenerator._units.Count; i++)
        {
            _mazeGenerator.units.Add(_mazeGenerator._units.ElementAt(i).Value);
        } 
        _mazeGenerator.VisualizeMaze();
    }
    
    [ContextMenu("Clear Dict")]
    public void Clear()
    {  
        MazeGenerator _mazeGenerator = this.GetComponent<MazeGenerator>();
        _mazeGenerator._units.Clear();
    }

    [ContextMenu("Print Size Of Dict")]
    public void Read()
    {
        MazeGenerator _mazeGenerator = this.GetComponent<MazeGenerator>();
        Debug.Log(_mazeGenerator._units.Count);
    }
    

    
    
}