using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Item{
    public string name;
    public int value;
    
    public Item()
    {
        name = "New Item";
        value = 0;
    }

    public Item(string _name, int _value){
        name = _name;
        value = _value;
    }
    
    //public static Item DeserializeFromNetwork(byte[] networkSerializedItem)
    //{
    //    MemoryStream stream = new MemoryStream(networkSerializedItem);
    //    BinaryFormatter binaryFormatter = new BinaryFormatter();
    //    return (Item)binaryFormatter.Deserialize(stream);
    //}

    //public byte[] SerializeForNetwork()
    //{
    //    MemoryStream stream = new MemoryStream();
    //    BinaryFormatter binaryFormatter = new BinaryFormatter();
    //    binaryFormatter.Serialize(stream, this);
    //    return stream.GetBuffer();
    //}
}
