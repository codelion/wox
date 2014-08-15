using System;
using System.Collections.Generic;
using System.Text;
using wox.serial;

/**
 * This class provides an example of a class with a primitive bidimensional array as field.
 * The main method uses the Easy class to serialize TestMultiArray object to XML (save method);
 * and to de-serialize the XML to a TestMultiArray object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: TestMultiArray.cs - 1.0
 */
public class TestMultiArray {

    private int[][] matrix;

    public TestMultiArray()
    {
    }

    public TestMultiArray(int[][] matrix) {
        this.matrix = matrix;
    }

    public static void printTestArray(TestMultiArray testArray){
        String display = "";
        for (int i=0; i<testArray.matrix.Length; i++){
            for (int j=0; j<testArray.matrix[i].Length; j++){
                display += testArray.matrix[i][j] + " ";
            }
            display += "\n";
        }
        Console.Out.WriteLine(display);
    }

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX represntation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void main(String[] args) {
        TestMultiArray test = new TestMultiArray(new int[][]{ new int[]{23, 56, 89, 36, 68},
                                                              new int[]{87, 64, 88, 32},
                                                              new int[]{78, 80, 21, 29, 34, 67} } );

        String filename = "C:\\TestMultiArraysCSharp.xml";
        //print the object
        printTestArray(test);
        //object to standard XML
        Easy.save(test, filename);
        //load the object from the XML file
        TestMultiArray newTest = (TestMultiArray)Easy.load(filename);
        //print the new object - it is the same as before               
        printTestArray(newTest);
    }

}