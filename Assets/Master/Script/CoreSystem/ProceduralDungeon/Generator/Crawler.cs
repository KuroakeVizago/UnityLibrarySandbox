using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Vizago.Core.ProceduralDungeon
{
    public class Crawler : MazeCreator
    {

        [Header("Amount: ")]
        [SerializeField][InspectorName("Horizontal Crawler")] private int m_amountHorizontal = 3;
        [SerializeField][InspectorName("Vertical Crawler")] private int m_amountVertical = 2;


        protected override void GenerateMaze()
        {
            for (int i = 0; i < m_amountHorizontal; i++)
                CrawlHorizontal();
            
            for (int i = 0; i < m_amountVertical; i++)
                CrawlVertical();

        }

        private void CrawlHorizontal()
        {
            bool finish = false;
            int x = 1;
            int z = Random.Range(1, m_depth-1);

            while (!finish)
            {
                map[x, z] = 0;

                if (Random.Range(0, 2) == 1)
                    x += Random.Range(1, 2);
                else
                    z += Random.Range(-1, 2);
                
                finish = x < 1 || x >= m_width-1 || z < 1 || z >= m_depth-1;
            }
        }
        
        private void CrawlVertical()
        {
            bool finish = false;
            int x = Random.Range(1, m_depth-1);
            int z = 1;

            while (!finish)
            {
                map[x, z] = 0;

                if (Random.Range(0, 2) == 1)
                    x += Random.Range(-1, 2);
                else
                    z += Random.Range(1, 2);
                finish = x < 1 || x >= m_width-1 || z < 1 || z >= m_depth-1;
            }
        }
        
    }
}
