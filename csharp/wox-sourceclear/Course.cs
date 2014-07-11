using System;
using wox.serial;

/**
 * This class provides an example of a class with some primitive
 * fields. The main method uses the Easy class to serialize a Course
 * object to XML (save method); and to de-serialize the XML to a
 * Course object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: Course.cs - 1.0
 */
public class Course
{
    private Int32 code;
    private String name;
    private Int32 term;

    public Course()
    {
    }

    public Course(Int32 code, String name, Int32 term)
    {
        this.code = code;
        this.name = name;
        this.term = term;
    }

    public override string ToString()
    {
        return "code: " + code + ", name: " + name + ", term: " + term;
    }

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX represntation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void Main(String[] args) {
        String filename = "C:\\TestCourseCSharp.xml";
        Course course1 = new Course(6756, "XML and Related Technologies", 3);
        Course course2 = new Course(9865, "Object Oriented Programming", 2);
        Course course3 = new Course(1134, "E-Commerce Programming", 2);

        //print the object
        Console.Out.WriteLine(course1);
        //object to standard XML
        Easy.save(course1, filename);
        //load the object from the XML file
        Course newCourse = (Course)Easy.load(filename);
        //print the new object - it is the same as before
        Console.Out.WriteLine(newCourse);
    }

}
