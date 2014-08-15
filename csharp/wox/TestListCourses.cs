using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using wox.serial;

/**
 * This class provides an example of an ArrayList of Course objects.
 * The main method uses the Easy class to serialize the ArrayList to XML (save method);
 * and to de-serialize the XML to an ArrayList object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: TestListCourses.cs - 1.0
 */
public class TestListCourses
{

    public static void printArray(IEnumerable list)
    {
        foreach (Object obj in list)
            Console.Out.WriteLine("" + obj);
    }

   /**
    * This method shows how easy is to serialize and de-serialize C# objects
    * to/from XML. The XML representation of the objects is a standard WOX represntation.
    * For more information about the XML representation please visit:
    * http://woxserializer.sourceforge.net/
    */
    public static void main(String[] args)
    {       
        ArrayList list = new ArrayList();
        list.Add(new Course(6756, "XML and Related Technologies", 3));
        list.Add(new Course(9865, "Object Oriented Programming", 2));
        list.Add(new Course(1134, "E-Commerce Programming", 2));
        list.Add(new Course(4598, "Enterprise Component Architecture", 3));

        String filename = "C:\\TestListCoursesCSharp.xml";
        //print the object
        printArray(list);
        //object to XML
        Easy.save(list, filename);
        //load the object from the XML file
        ArrayList newList = (ArrayList)Easy.load(filename);
        //print the object just loaded
        printArray(newList);

    }


}