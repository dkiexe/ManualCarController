using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [Serializable]
    class SerializedDict<T1, T2>
    {
        public SerializedDictRecord<T1, T2>[] ItemRecords;

        public Dictionary<T1, T2> toDictionary()
        {
            Dictionary<T1, T2> newDict = new Dictionary<T1, T2>();

            for(int i = 0; i <= ItemRecords.Length; i++)
            {
                SerializedDictRecord<T1, T2> record = ItemRecords[i];

                newDict[record.key] = record.value;
            }

            return newDict;
        }
    }

    [Serializable]
    class SerializedDictRecord<T1, T2>
    {
        [SerializeField]
        public T1 key;

        [SerializeField]
        public T2 value;
    }
}
