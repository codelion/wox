using System;
using wox.serial;

/**
 * This class provides an example of a class with some primitive
 * fields, and an array of Course objects. The main method uses the
 * Easy class to serialize a Student object to XML (save method);
 * and to de-serialize the XML to a Student object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: Student.cs - 1.0
 */
public class Student
{
    private String name;
    private Int32 registrationNumber;
    private Course[] courses;

    public Student()
    {
    }

    public Student(String name, Int32 registrationNumber, Course[] courses)
    {
        this.name = name;
        this.registrationNumber = registrationNumber;
        this.courses = courses;
    }

    public override string ToString()
    {
        return "name: " + name + ", registrationNumber, " + registrationNumber +
               ", courses: \n" + printArray(courses);
    }


    public String printArray(Object[] ob)
    {
        String coursesStr = "";
        if (courses == null)
        {
            return coursesStr;
        }
        else
        {
            for (int i = 0; i < ob.Length; i++)
            {
                coursesStr = coursesStr + ob[i] + "\n";
            }
            return coursesStr;
        }
    }

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX represntation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void main(String[] args)
    {        
        Course[] courses = {new Course(6756, "XML and Related Technologies", 2),
                            new Course(9865, "Object Oriented Programming", 2),
                            new Course(1134, "E-Commerce Programming", 3)};
        Student student = new Student("Carlos Jaimez", 76453, courses);

        String filename = "C:\\TestStudentCSharp.xml";
        //print the Student object
        Console.Out.WriteLine(student);
        //object to standard XML
        Easy.save(student, filename);
        //get the object back from the XML file
        Student newStudent = (Student)Easy.load(filename);
        //print the new object - it is the same as before
        Console.Out.WriteLine(newStudent);
    }

}

