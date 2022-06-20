using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Vizago.Core.ProceduralDungeon
{

    public struct MapLocation
    {
        public int x,z;

        public MapLocation(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        
    }
    
    public class MazeCreator : MonoBehaviour
    {

        [Header("Config:")] 
        [SerializeField] protected int m_width = 30;
        [SerializeField] protected int m_depth = 30;
        [Space] 
        [SerializeField] protected int m_scale = 6;
        [SerializeField] protected bool m_createOnStart;

        protected byte[,] map;

        protected readonly MapLocation[] crawlDirection =
        {
            new MapLocation(1,0), 
            new MapLocation(0,1),
            new MapLocation(-1,0),
            new MapLocation(0,-1)
        };


        [Button("Draw Maze", enabledMode: EButtonEnableMode.Playmode)]
        private void ReDrawMaze()
        {
            foreach (var childObj in GetComponentsInChildren<Transform>())
                if (childObj != transform) 
                    Destroy(childObj.gameObject);
            
            GenerateMaze();
            DrawMaze();
            
        }        
        
        private void Start()
        {

            if (m_createOnStart)
                InitMaze();

        }

        protected void InitMaze()
        {
            CreateMazeArea();
            GenerateMaze();
            DrawMaze();
        }
        
        protected virtual void CreateMazeArea()
        {
            
            map = new byte[m_width, m_depth];

            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    map[x, z] = 1;
                }
            }
        }
        
        protected virtual void GenerateMaze()
        {
            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    map[x, z] = 0;
                }
            }
        }
        
        protected virtual void DrawMaze()
        {
            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Debug.Log(map[x, z]);
                    if (map[x,z] == 1)
                    {
                        GameObject cacheWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cacheWall.transform.localScale *= m_scale;
                        cacheWall.transform.position = new Vector3(x * m_scale, 0, z * m_scale);
                        cacheWall.transform.SetParent(this.transform);
                    }
                }
            }
        }

        protected int CountSquareNeighbours(int x, int z)
        {
            var count = 0;
            
            //Check if the x or z is at the map border
            if (x <= 0 || x >= m_width - 1 || z <= 0 || z >= m_depth - 1)
                return 5;
            
            if (map[x - 1, z] == 0) count++;
            if (map[x + 1, z] == 0) count++;
            if (map[x, z - 1] == 0) count++;
            if (map[x, z + 1] == 0) count++;

            return count;

        }

        protected int CountDiagonalNeighbours(int x, int z)
        {
            int count = 0;
            
            //Check if the x or z is at the map border
            if (x <= 0 || x >= m_width - 1 || z <= 0 || z >= m_depth - 1)
                return 5;
            
            if (map[x - 1, z - 1] == 0) count++;
            if (map[x - 1, z + 1] == 0) count++;
            if (map[x + 1, z - 1] == 0) count++;
            if (map[x + 1, z + 1] == 0) count++;
            
            return count;
        }

        protected int CountAllNeighbour(int x, int z)
        {
            return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
        }
        
    }
}
