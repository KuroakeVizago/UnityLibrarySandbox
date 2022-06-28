using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vizago.Core.ProceduralDungeon
{
    public class PrimsCrawler : MazeCreator
    {
        protected override void GenerateMaze()
        {

            int x = 2;
            int z = 2;

            map[x, z] = 0;

            List<MapLocation> wallLocation = new List<MapLocation>();
            
            wallLocation.Add(new MapLocation(x + 1, z));
            wallLocation.Add(new MapLocation(x - 1, z));
            wallLocation.Add(new MapLocation(x, z + 1));
            wallLocation.Add(new MapLocation(x, z - 1));

            int loopsHandler = 0;
            while (wallLocation.Count-1 > 0 && loopsHandler < 5000)
            {
                int randomWall = Random.Range(0, wallLocation.Count);

                x = wallLocation[randomWall].x;
                z = wallLocation[randomWall].z;
                
                wallLocation.RemoveAt(randomWall);
                
                if (CountSquareNeighbours(x, z) == 1)
                {
                    map[x, z] = 0;
                    wallLocation.Add(new MapLocation(x + 1, z));
                    wallLocation.Add(new MapLocation(x - 1, z));
                    wallLocation.Add(new MapLocation(x, z + 1));
                    wallLocation.Add(new MapLocation(x, z - 1));
                }

                loopsHandler++;

            }

        }
    }
}
