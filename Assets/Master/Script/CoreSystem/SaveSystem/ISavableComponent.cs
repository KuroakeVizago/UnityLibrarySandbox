using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Vizago.SaveSystem
{
    public interface ISavableComponent
    {

        /// <summary>
        /// Taking saved data from the component
        /// </summary>
        /// <returns> Giving the object value of the data that has been passed </returns>
        public abstract object Save();
        
        /// <summary>
        /// Passing a saved Data in Json Form
        /// Use JsonUtility.FromJson() To parse the data
        /// </summary>
        /// <param name="savedJsonData"> The saved json data that contains component data </param>
        public abstract void Load(string savedJsonData);
        
    }
}
