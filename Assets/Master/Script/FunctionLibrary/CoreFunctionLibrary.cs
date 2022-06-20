using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VizagoExtension.FunctionLibrary
{
    public static class CoreFunctionLibrary
    {
        private static System.Random _rng = new System.Random();

        public static void ShuffleArray<T>(this T []array)
        {
            var n = array.Length;
            while (n > 1) 
            {
                var k = _rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
    }
}
