using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VizagoExtension.FunctionLibrary;

namespace Vizago.Core.ProceduralDungeon
{
    public class Recursive : MazeCreator
    {
        protected override void GenerateMaze()
        {
            Generate(Random.Range(1, m_width), Random.Range(1, m_depth));
        }

        void Generate(int x, int z)
        {
            if (CountSquareNeighbours(x, z) > 1)
                return;

            map[x, z] = 0;

            crawlDirection.ShuffleArray();
            
            Generate(x + crawlDirection[0].x, z + crawlDirection[0].z);
            Generate(x + crawlDirection[1].x, z + crawlDirection[1].z);
            Generate(x + crawlDirection[2].x, z + crawlDirection[2].z);
            Generate(x + crawlDirection[3].x, z + crawlDirection[3].z);
            
        }
    }
}
