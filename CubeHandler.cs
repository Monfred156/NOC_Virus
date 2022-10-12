using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeHandler : MonoBehaviour
{
    struct Cube
    {
        public Vector3 Position;
        public GameObject GameObject;
        public bool Contaminated;
    }
    
    public int size = 3;
    
    [SerializeField] private GameObject prefabCube;
    [SerializeField] private Material healthyMaterial;
    [SerializeField] private Material contaminatedMaterial;
    
    [SerializeField] private TextMeshProUGUI iterationText;
    [SerializeField] private TextMeshProUGUI percentageText;

    
    private List<Cube> _subCube = new List<Cube>();
    private Vector3 _position = Vector3.zero;
    
    private float _nextActionTime = 0.0f;
    private float _period = 1.5f;

    private float _spaceBetweenSubCube = 1.5f;

    private int _iteration;
    private float _percentageOfContamination;
    private void Awake()
    {
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                for (var z = 0; z < size; z++)
                {
                    _subCube.Add(new Cube
                    {
                        Contaminated = false,
                        GameObject = Instantiate(prefabCube, _position, Quaternion.identity),
                        Position = _position
                    });
                    _position.z += _spaceBetweenSubCube;
                }
                _position.z = 0;
                _position.y += _spaceBetweenSubCube;
            }
            _position.y = 0;
            _position.x += _spaceBetweenSubCube;
        }

        var rand = Random.Range(0, _subCube.Count);
        _subCube[rand] = new Cube{
            Contaminated = true,
            GameObject = _subCube[rand].GameObject,
            Position = _subCube[rand].Position
        };
        _subCube[rand].GameObject.GetComponent<MeshRenderer>().material = contaminatedMaterial;
        _nextActionTime = Time.timeSinceLevelLoad + _period;
    }

    private void Update()
    {
        if (!(Time.time > _nextActionTime) || _percentageOfContamination == 100.0f) return;
        var contaminatedCubes = _subCube.Where(cube => cube.Contaminated).ToList();

        foreach (var contaminatedCube in contaminatedCubes)
        {
            var i = 0;
            foreach (var cube in _subCube.ToArray())
            {
                if (Vector3.Distance(cube.Position, contaminatedCube.Position) == _spaceBetweenSubCube)
                {
                    _subCube[i] = new Cube{
                        Contaminated = true,
                        GameObject = _subCube[i].GameObject,
                        Position = _subCube[i].Position
                    };
                    _subCube[i].GameObject.GetComponent<MeshRenderer>().material = contaminatedMaterial;
                }
                i++;
            }
        }
        _iteration++;
        _percentageOfContamination = _subCube.Where(cube => cube.Contaminated).ToList().Count * 100.0f / _subCube.Count;
        iterationText.text = "Iteration: " + _iteration;
        percentageText.text = "Percentage: " + _percentageOfContamination + "%";
        _nextActionTime += _period;
    }
}
