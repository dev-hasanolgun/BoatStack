using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public PathCreator Path;
    
    [BoxGroup("Create Level")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentPath")] [OnValueChanged("ShowCurrentObjects")]
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
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.Obstacle)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [PropertyRange(0,"_obstacleLastIndex")]
    public int CurrentObstacle;
    private int _obstacleLastIndex;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.ExtraBoat)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [PropertyRange(0,"_extraBoatLastIndex")]
    public int CurrentExtraBoat;
    private int _extraBoatLastIndex;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.PointBonus)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [PropertyRange(0,"_pointBonusLastIndex")]
    public int CurrentPointBonus;
    private int _pointBonusLastIndex;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.SpeedBonus)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [PropertyRange(0,"_speedBonusLastIndex")]
    public int CurrentSpeedBonus;
    private int _speedBonusLastIndex;
    
    [BoxGroup("Object Settings")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [Range(0f,100f)]
    public float Percentage;
    
    [BoxGroup("Object Settings")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    [Range(0,2)]
    public int WidthLevel;
    
    [BoxGroup("Object Settings")] [EnumToggleButtons, HideLabel]
    public ObjectEnum ObjectEnumField;

    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.Obstacle)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    public Obstacle Obstacle;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.ExtraBoat)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    public ExtraBoatItem ExtraBoat;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.PointBonus)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    public PointBonusItem PointBonus;
    
    [BoxGroup("Object Settings")]
    [ShowIf("ObjectEnumField", ObjectEnum.SpeedBonus)]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentObjects")]
    public SpeedBonusItem SpeedBonus;
    
    public enum ObjectEnum
    {
        Obstacle, ExtraBoat, PointBonus, SpeedBonus
    }
    
    private Mesh _waterSlideMesh;
    private WaterSlideData _slideData;
    private Vector3 _currentObstaclePos;
    private Quaternion _currentObstacleRot;
    private List<Vector3> _obstaclePosList = new List<Vector3>();
    private List<Vector3> _extraBoatPosList = new List<Vector3>();
    private List<Vector3> _pointBonusPosList = new List<Vector3>();
    private List<Vector3> _speedBonusPosList = new List<Vector3>();
    private bool _isLevelExist;
    private bool _isObstacleExist;
    private bool _isExtraBoatExist;
    private bool _isPointBonusExist;
    private bool _isSpeedBonusExist;
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
        ShowCurrentObjects();
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
        LevelDatabase.LevelDB[CurrentLevel].SlideData.LocalNormals = Path.path.localNormals;
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
        ShowCurrentObjects();
    }
    
    [BoxGroup("Object Settings")]
    [Button("Add Obstacle", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this.ObjectEnumField == ObjectEnum.Obstacle")]
    [GUIColor(0,1,0)]
    private void AddObstacle()
    {
        LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList.Add(new ObstacleData(Obstacle, _currentObstaclePos, _currentObstacleRot));
        CurrentObstacle += LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentObjects();
    }
    
    [BoxGroup("Object Settings")]
    [Button("Delete Obstacle", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this._isObstacleExist && this.ObjectEnumField == ObjectEnum.Obstacle")]
    [GUIColor(1,0,0)]
    private void DeleteObstacle()
    {
        LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList.RemoveAt(CurrentObstacle);
        CurrentObstacle -= 1;
        UpdateProperties();
        ShowCurrentObjects();
    }
    [BoxGroup("Object Settings")]
    [Button("Add ExtraBoat", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this.ObjectEnumField == ObjectEnum.ExtraBoat")]
    [GUIColor(0,1,0)]
    private void AddExtraBoat()
    {
        LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList.Add(new ExtraBoatData(ExtraBoat, _currentObstaclePos, _currentObstacleRot));
        CurrentExtraBoat += LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentObjects();
    }
    
    [BoxGroup("Object Settings")]
    [Button("Delete ExtraBoat", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this._isExtraBoatExist && this.ObjectEnumField == ObjectEnum.ExtraBoat")]
    [GUIColor(1,0,0)]
    private void DeleteExtraBoat()
    {
        LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList.RemoveAt(CurrentExtraBoat);
        CurrentExtraBoat -= 1;
        UpdateProperties();
        ShowCurrentObjects();
    }
    [BoxGroup("Object Settings")]
    [Button("Add PointBonus", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this.ObjectEnumField == ObjectEnum.PointBonus")]
    [GUIColor(0,1,0)]
    private void AddPointBonus()
    {
        LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList.Add(new PointBonusData(PointBonus, _currentObstaclePos, _currentObstacleRot));
        CurrentPointBonus += LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentObjects();
    }
    
    [BoxGroup("Object Settings")]
    [Button("Delete PointBonus", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this._isPointBonusExist && this.ObjectEnumField == ObjectEnum.PointBonus")]
    [GUIColor(1,0,0)]
    private void DeletePointBonus()
    {
        LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList.RemoveAt(CurrentPointBonus);
        CurrentPointBonus -= 1;
        UpdateProperties();
        ShowCurrentObjects();
    }
    [BoxGroup("Object Settings")]
    [Button("Add SpeedBonus", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this.ObjectEnumField == ObjectEnum.SpeedBonus")]
    [GUIColor(0,1,0)]
    private void AddSpeedBonus()
    {
        LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList.Add(new SpeedBonusData(SpeedBonus, _currentObstaclePos, _currentObstacleRot));
        CurrentSpeedBonus += LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentObjects();
    }
    
    [BoxGroup("Object Settings")]
    [Button("Delete SpeedBonus", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false && this._isSpeedBonusExist && this.ObjectEnumField == ObjectEnum.SpeedBonus")]
    [GUIColor(1,0,0)]
    private void DeleteSpeedBonus()
    {
        LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList.RemoveAt(CurrentSpeedBonus);
        CurrentSpeedBonus -= 1;
        UpdateProperties();
        ShowCurrentObjects();
    }
    
    private void UpdateProperties()
    {
        _isLevelExist = LevelDatabase.LevelDB.Count != 0;

        if (_isLevelExist)
        {
            _isObstacleExist = LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList.Count != 0;
            _isExtraBoatExist = LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList.Count != 0;
            _isPointBonusExist = LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList.Count != 0;
            _isSpeedBonusExist = LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList.Count != 0;
            
            _levelLastIndex = Mathf.Clamp(LevelDatabase.LevelDB.Count - 1,0,int.MaxValue);
            CurrentLevel = Mathf.Clamp(CurrentLevel, 0, _levelLastIndex);
            _obstacleLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList.Count-1,0,int.MaxValue);
            CurrentObstacle = Mathf.Clamp(CurrentObstacle, 0, _obstacleLastIndex);
            _extraBoatLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList.Count-1,0,int.MaxValue);
            CurrentExtraBoat = Mathf.Clamp(CurrentExtraBoat, 0, _extraBoatLastIndex);
            _pointBonusLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList.Count-1,0,int.MaxValue);
            CurrentPointBonus = Mathf.Clamp(CurrentPointBonus, 0, _pointBonusLastIndex);
            _speedBonusLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList.Count-1,0,int.MaxValue);
            CurrentSpeedBonus = Mathf.Clamp(CurrentSpeedBonus, 0, _speedBonusLastIndex);
        }
        else
        {
            _levelLastIndex = 0;
            CurrentLevel = 0;
            _obstacleLastIndex = 0;
            CurrentObstacle = 0;
            _extraBoatLastIndex = 0;
            CurrentExtraBoat = 0;
            _pointBonusLastIndex = 0;
            CurrentPointBonus = 0;
            _speedBonusLastIndex = 0;
            CurrentSpeedBonus = 0;
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
    private void ShowCurrentObjects()
    {
        _obstaclePosList.Clear();
        _extraBoatPosList.Clear();
        _pointBonusPosList.Clear();
        _speedBonusPosList.Clear();
        
        if (_isLevelExist)
        {
            if (_isObstacleExist)
            {
                var obstacles = LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList;

                for (int i = 0; i < obstacles.Count; i++)
                {
                    _obstaclePosList.Add(obstacles[i].Position);
                }
            }
            if (_isExtraBoatExist)
            {
                var extraBoats = LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList;

                for (int i = 0; i < extraBoats.Count; i++)
                {
                    _extraBoatPosList.Add(extraBoats[i].Position);
                }
            }
            if (_isPointBonusExist)
            {
                var pointBonuses = LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList;

                for (int i = 0; i < pointBonuses.Count; i++)
                {
                    _pointBonusPosList.Add(pointBonuses[i].Position);
                }
            }
            if (_isSpeedBonusExist)
            {
                var speedBonuses = LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList;

                for (int i = 0; i < speedBonuses.Count; i++)
                {
                    _speedBonusPosList.Add(speedBonuses[i].Position);
                }
            }
        }
    }

    private void UpdateSliderData()
    {
        _slideData.LocalPoints = Path.path.localPoints;
        _slideData.LocalNormals = Path.path.localNormals;
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

    private void OnDrawGizmosSelected()
    {
        var mesh = GenerateMesh();
    
        Gizmos.color = Color.green;
        Gizmos.DrawMesh(mesh);

        Gizmos.color = new Color(0f, 0f, 1f, 0.20f);
        Gizmos.DrawWireMesh(mesh);

        var pos = Path.path.GetPointAtTime(Percentage/100f);
        var vec = Path.path.GetPointAtTime((Percentage + 1f) / 100f) - pos;
        var nor = Vector3.up;
        var cross = Vector3.Cross(vec.normalized, nor);
        var vel = Mathf.SmoothStep(0, 1, 1 / 2f);
        var dis = Width/3f;
        
        switch (WidthLevel)
        {
            case 0:
                _currentObstaclePos = pos + cross * dis + Vector3.up * vel * Depth * 0.8f;
                _currentObstacleRot = Quaternion.LookRotation((_currentObstaclePos - pos).normalized);
                break;
            case 1:
                _currentObstaclePos = Path.path.GetPointAtTime(Percentage/100f);
                _currentObstacleRot = Quaternion.LookRotation(cross);
                break;
            case 2:
                _currentObstaclePos = pos - cross * dis + Vector3.up * vel * Depth * 0.8f;
                _currentObstacleRot = Quaternion.LookRotation((_currentObstaclePos - pos).normalized);
                break;
        }
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_currentObstaclePos,0.1f);
        
        if (!_isCreatingNewLevel)
        {
            if (_isObstacleExist)
            {
                for (int i = 0; i < _obstaclePosList.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_obstaclePosList[i], 0.1f);
                }
            }

            if (_isExtraBoatExist)
            {
                for (int i = 0; i < _extraBoatPosList.Count; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(_extraBoatPosList[i], 0.1f);
                }
            }

            if (_isPointBonusExist)
            {
                for (int i = 0; i < _pointBonusPosList.Count; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(_pointBonusPosList[i], 0.1f);
                }
            }

            if (_isSpeedBonusExist)
            {
                for (int i = 0; i < _speedBonusPosList.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(_speedBonusPosList[i], 0.1f);
                }
            }
            switch (ObjectEnumField)
            {
                case ObjectEnum.Obstacle:
                    if (_isObstacleExist)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(_obstaclePosList[CurrentObstacle], 0.1f);
                    }
                    break;
                case ObjectEnum.ExtraBoat:
                    if (_isExtraBoatExist)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(_extraBoatPosList[CurrentExtraBoat], 0.1f);
                    }
                    break;
                case ObjectEnum.PointBonus:
                    if (_isPointBonusExist)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(_pointBonusPosList[CurrentPointBonus], 0.1f);
                    }
                    break;
                case ObjectEnum.SpeedBonus:
                    if (_isSpeedBonusExist)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(_speedBonusPosList[CurrentSpeedBonus], 0.1f);
                    }
                    break;
            }
        }
    }
}