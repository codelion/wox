using System;
using System.Collections.Generic;
using System.Text;
using wox.serial;
using System.Collections;

/**
 * This class provides an example of a Hashtable of Course objects.
 * The main method uses the Easy class to serialize the Hashtable to XML (save method);
 * and to de-serialize the XML to a Hashtable object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: TestMapCourses.cs - 1.0
 */
public class TestMapCourses
{

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX represntation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void main(String[] args)
    {
        Hashtable map = new Hashtable();
        map.Add(6756, new Course(6756, "XML and Related Technologies", 3));
        map.Add(9865, new Course(9865, "Object Oriented Programming", 2));
        map.Add(1134, new Course(1134, "E-Commerce Programming", 2));
        map.Add(4598, new Course(4598, "Enterprise Component Architecture", 3));

        String filename = "C:\\TestMapCoursesCSharp.xml";
        //print the object
        printMap(map);
        //object to XML
        Easy.save(map, filename);
        //load the object from the XML file
        Hashtable newMap = (Hashtable)Easy.load(filename);
        //print the object just loaded
        printMap(newMap);

    }

    public static void printMap(Hashtable map)
    {
        IDictionaryEnumerator keys = map.GetEnumerator();
        while (keys.MoveNext())
        {
            Console.Out.WriteLine("*KEY* " + keys.Key + ", *OBJECT* " + keys.Value);
        }

    }



}