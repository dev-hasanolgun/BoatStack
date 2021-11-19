using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public PathCreator Path;
    
    [BoxGroup("Create Level")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentPath")] [OnValueChanged("ShowCurrentObstacles")]
    [PropertyRange(0,"_levelLastIndex")]
    [SuppressInvalidAttributeError]
    public int CurrentLevel;
    private int _levelLastIndex;
    
    [BoxGroup("Create Level")]
    [SerializeField]
    private int _levelID;
    
    [BoxGroup("Create Level")]
    [SerializeField] 
    private string _levelName;
    
    [BoxGroup("Create Level")]
    public LevelDatabase LevelDatabase;
    
    [BoxGroup("Slide Mesh Settings")]
    [OddEvenRange(3, 100, false)]
    public int Density = 9;
    
    [BoxGroup("Slide Mesh Settings")]
    public float Depth = 1;
    
    [BoxGroup("Slide Mesh Settings")]
    public float Width = 1;
    
    [BoxGroup("Obstacle Settings")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObstacles")]
    [PropertyRange(0,"_obstacleLastIndex")]
    public int CurrentObstacle;
    private int _obstacleLastIndex;
    
    [BoxGroup("Obstacle Settings")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObstacles")]
    [Range(0f,100f)]
    public float Percentage;
    
    private Mesh _waterSlideMesh;
    private WaterSlideData _slideData;
    private Vector3 _currentObstacle;
    private List<Vector3> _obstacleList = new List<Vector3>();
    private bool _isLevelExist;
    private bool _isObstacleExist;
    private bool _isCreatingNewLevel;
    
    [BoxGroup("Create Level")]
    [Button("New Level", ButtonSizes.Medium), HideIf("_isCreatingNewLevel")]
    [GUIColor(0,1,0)]
    private void NewLevel()
    {
        _isCreatingNewLevel = true;
        Path.EditorData.ResetBezierPath(transform.position);
        UpdateProperties();
    }
    
    [BoxGroup("Create Level")]
    [Button("Create Level", ButtonSizes.Medium), ShowIf("_isCreatingNewLevel")]
    [GUIColor(0,1,0)]
    private void CreateLevel()
    {
        UpdateSliderData();
        LevelDatabase.LevelDB.Add(new Level(_slideData, Path.bezierPath, _levelID,_levelName));
        CurrentLevel += LevelDatabase.LevelDB.Count > 1 ? 1 : 0;
        _isCreatingNewLevel = false;
        UpdateProperties();
        ShowCurrentObstacles();
    }
    
    [BoxGroup("Create Level")]
    [Button("Cancel", ButtonSizes.Medium), ShowIf("_isCreatingNewLevel")]
    [GUIColor(1,0,0)]
    private void CancelLevelCreation()
    {
        _isCreatingNewLevel = false;
        UpdateProperties();
        ShowCurrentPath();
    }
    
    [BoxGroup("Create Level")]
    [Button("Update Level", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false")]
    [GUIColor(0,0,1)]
    private void UpdateLevel()
    {
        LevelDatabase.LevelDB[CurrentLevel].SlideData.LocalPoints = Path.path.localPoints;
        LevelDatabase.LevelDB[CurrentLevel].SlideData.LocalTangents = Path.path.localTangents;
        LevelDatabase.LevelDB[CurrentLevel].SlideData.Density = Density;
        LevelDatabase.LevelDB[CurrentLevel].SlideData.Depth = Depth;
        LevelDatabase.LevelDB[CurrentLevel].SlideData.Width = Width;
        LevelDatabase.LevelDB[CurrentLevel].BezierPath = Path.bezierPath;
        UpdateProperties();
    }
    
    [BoxGroup("Create Level")]
    [Button("Delete Level", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false")]
    [GUIColor(1,0,0)]
    private void DeleteLevel()
    {
        LevelDatabase.LevelDB.RemoveAt(CurrentLevel);
        CurrentLevel -= 1;
        UpdateProperties();
        ShowCurrentPath();
        ShowCurrentObstacles();
    }
    
    [BoxGroup("Obstacle Settings")]
    [Button("Add Obstacle", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false")]
    [GUIColor(0,1,0)]
    private void AddObstacle()
    {
        LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Add(_currentObstacle);
        CurrentObstacle += LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentObstacles();
    }
    
    [BoxGroup("Obstacle Settings")]
    [Button("Delete Obstacle", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this._isObstacleExist")]
    [GUIColor(1,0,0)]
    private void DeleteObstacle()
    {
        LevelDatabase.LevelDB[CurrentLevel].ObstacleList.RemoveAt(CurrentObstacle);
        CurrentObstacle -= 1;
        UpdateProperties();
        ShowCurrentObstacles();
    }
    
    private void UpdateProperties()
    {
        _isLevelExist = LevelDatabase.LevelDB.Count != 0;

        if (_isLevelExist)
        {
            _isObstacleExist = LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Count != 0;
            
            _levelLastIndex = Mathf.Clamp(LevelDatabase.LevelDB.Count - 1,0,int.MaxValue);
            CurrentLevel = Mathf.Clamp(CurrentLevel, 0, _levelLastIndex);
            _obstacleLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Count-1,0,int.MaxValue);
            CurrentObstacle = Mathf.Clamp(CurrentObstacle, 0, _obstacleLastIndex);
        }
        else
        {
            _levelLastIndex = 0;
            CurrentLevel = 0;
            _obstacleLastIndex = 0;
            CurrentObstacle = 0;
        }
        
        #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    private void ShowCurrentPath()
    {
        if (_isLevelExist)
        {
            Path.bezierPath = LevelDatabase.LevelDB[CurrentLevel].BezierPath;
            Path.EditorData.Initialize(false);
        }
        else
        {
            Path.EditorData.ResetBezierPath(transform.position);
        }
    }
    private void ShowCurrentObstacles()
    {
        _obstacleList.Clear();
        
        if (_isLevelExist && _isObstacleExist)
        {
            var obstacles = LevelDatabase.LevelDB[CurrentLevel].ObstacleList;

            for (int i = 0; i < obstacles.Count; i++)
            {
                _obstacleList.Add(obstacles[i]);
            }
        }
    }

    private void UpdateSliderData()
    {
        _slideData.LocalPoints = Path.path.localPoints;
        _slideData.LocalTangents = Path.path.localTangents;
        _slideData.Density = Density;
        _slideData.Depth = Depth;
        _slideData.Width = Width;
    }
    private Mesh GenerateMesh()
    {
        _waterSlideMesh = new Mesh();
        
        var positions = Path.path.localPoints;
        var pathNormals = Path.path.localNormals;
        var posAmount = positions.Length;
        
        var vertices = new Vector3[posAmount*Density];
        for (int a = 0, i = 0; i < posAmount; i++)
        {
            var pos = positions[i];
            var vec = i == posAmount-1 ? pos - positions[i-1] : positions[i+1] - pos;
            var nor = Vector3.up;
            var cross = Vector3.Cross(vec.normalized, nor);
            for (int j = (Density - 1)/2; j > 0; j--)
            {
                var dis = Width/2f / ((Density - 1) / 2);
                var vel = Mathf.SmoothStep(0, 1, j / ((Density - 1) / 2f + Density/3f));
                var deltaPos = new Vector3(pathNormals[i].x,0,pathNormals[i].z);
                vertices[a] = pos + cross * (dis * j) + Vector3.up * (vel * Depth);
                a++;
            }
            vertices[a] = pos;
            a++;
            for (int j = 1; j <= (Density - 1)/2; j++)
            {
                var dis = Width/2f / ((Density - 1) / 2);
                var vel = Mathf.SmoothStep(0, 1, j / ((Density - 1) / 2f + Density/3f));
                var deltaPos = new Vector3(pathNormals[i].x,0,pathNormals[i].z);
                vertices[a] = pos - cross * (dis * j) + Vector3.up * (vel * Depth);
                a++;
            }
        }
        _waterSlideMesh.vertices = vertices;
        var tris = new int[(vertices.Length - Density) / Density * (Density - 1) * 6];
        
        var k = 0;
        var m = 0;
        for (int i = 0; i < (posAmount - 1)*6; i++)
        {
            tris[k] = 0+Density*m;
            tris[k+1] = Density+Density*m;
            tris[k+2] = 1+Density*m;
            tris[k+3] = Density+Density*m;
            tris[k+4] = Density+1+Density*m;
            tris[k+5] = 1+Density*m;
            i += 5;
            k += 6;
            for (int j = 0; j < (Density - 2) * 6; j++)
            {
                tris[k] = tris[k - 6] + 1;
                k++;
            }

            m++;
        }
        _waterSlideMesh.triangles = tris;
        
        _waterSlideMesh.RecalculateNormals();

        return _waterSlideMesh;
    }

    private void OnDrawGizmos()
    {
        var mesh = GenerateMesh();
    
        Gizmos.color = Color.green;
        Gizmos.DrawMesh(mesh);

        Gizmos.color = new Color(0f, 0f, 1f, 0.20f);
        Gizmos.DrawWireMesh(mesh);

        _currentObstacle = Path.path.GetPointAtTime(Percentage/100f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_currentObstacle,0.05f);

        if (!_isCreatingNewLevel)
        {
            for (int i = 0; i < _obstacleList.Count; i++)
            {
                if (_obstacleList[i] == _obstacleList[CurrentObstacle])
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(_obstacleList[i],0.05f);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_obstacleList[i],0.05f);
                }
            }
        }
    }
}