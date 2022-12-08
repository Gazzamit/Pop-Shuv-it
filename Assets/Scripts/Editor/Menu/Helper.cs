using System.IO;
using System.Xml.Serialization;

//Helper is for redudant operations
public static class Helper
{
    //Serialize 
    public static string Serialize<T>(this T toSerealize) //Template operator T (any type)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T)); 
        StringWriter writer = new StringWriter();
        xml.Serialize(writer, toSerealize); //string is serialized
        return writer.ToString(); //return to single string
    }

    //Deserialize
    public static T Deserialize<T>(this string toDeserialize) //returning a type
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(toDeserialize);
        return (T)xml.Deserialize(reader);
    }
}