using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Vizago.Core.ProceduralDungeon
{
    public class WilsonsCrawler : MazeCreator
    {
        private List<MapLocation> _availableMaze = new List<MapLocation>();

        protected override void GenerateMaze()
        {
            //Generate starting cell
            int x = Random.Range(2, m_width - 1);
            int z = Random.Range(2, m_depth - 1);
            map[x, z] = 2;

            int loopLimit = 0;
            //|| loopLimit < 50000
            while (GetAvailableCells() > 1 && loopLimit < 50000)
            {
                RandomCrawler();
                loopLimit++;
                if (loopLimit > 50000)
                {
                    break;
                    throw new Exception("To many loops");
                }
            }
            
        }

        int CountStartPointSquareNeighbours(int x, int z)
        {
            var count = 0;
            
            for (int i = 0; i < crawlDirection.Length; i++)
            {
                int nextX = x + crawlDirection[i].x;
                int nextZ = z + crawlDirection[i].z;

                if (map[nextX, nextZ] == 2) 
                    count++;
            }
            
            return count;
        }

        int GetAvailableCells()
        {
            _availableMaze.Clear();

            for (int z = 1; z < m_depth - 1; z++)
                for (int x = 1; x < m_width - 1; x++)
                {
                    if (CountStartPointSquareNeighbours(x, z) == 0)
                    {
                        _availableMaze.Add(new MapLocation(x, z));
                    }
                    
                }

            return _availableMaze.Count;
        }
        
        private void RandomCrawler()
        {
            List<MapLocation> crawlHistory = new List<MapLocation>();

            int randomStartIndex = Random.Range(0, _availableMaze.Count);
            int currentX = _availableMaze[randomStartIndex].x;
            int currentZ = _availableMaze[randomStartIndex].z;
            
            crawlHistory.Add(new MapLocation(currentX, currentZ));
            
            int loopsLimit = 0;
            bool validPath = false;
            
            while (!(currentX <= 0 || currentX >= m_width - 1 || currentZ <= 0 || currentZ >= m_depth - 1) 
                   && loopsLimit < 5000 && !validPath)
            {
                map[currentX, currentZ] = 0;

                if (CountStartPointSquareNeighbours(currentX, currentZ) > 1)
                    break;
                    
                int randomDirection = Random.Range(0, crawlDirection.Length);

                int nextX = currentX + crawlDirection[randomDirection].x;
                int nextZ = currentZ + crawlDirection[randomDirection].z;
                
                if (CountSquareNeighbours(nextX, nextZ) == 1)
                {

                    currentX = nextX;
                    currentZ = nextZ;
                    crawlHistory.Add(new MapLocation(currentX, currentZ));
                }

                validPath = CountStartPointSquareNeighbours(currentX, currentZ) == 1;
               
                loopsLimit++;
            }

            if (validPath)
            {
                map[currentX, currentZ] = 0;

                foreach (var crawlLocation in crawlHistory)
                    map[crawlLocation.x, crawlLocation.z] = 0;
                crawlHistory.Clear();
                
            }
            else
            {
                foreach (var crawlLocation in crawlHistory)
                    map[crawlLocation.x, crawlLocation.z] = 1;
                crawlHistory.Clear();
        
            }
        }
    }
}
