using System;
using wox.serial;

/**
 * This class provides an example of a class with some primitive arrays as fields.
 * The main method uses the Easy class to serialize TestArray object to XML (save method);
 * and to de-serialize the XML to a TestArray object back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: TestArray.cs - 1.0
 */
public class TestArray {
    private char[] codes;
    private int[] values;
    private bool[] answers;

    public TestArray()
    {
    }

    public TestArray(char[] codes, int[] values, bool[] answers) {
        this.codes = codes;
        this.values = values;
        this.answers = answers;
    }

    public override string ToString()
    {
        String display = "";
        if (codes != null)
        {
            for (int i = 0; i < codes.Length; i++)
            {
                display += codes[i] + " ";
            }
            display += "\n";
        }
        if (values != null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                display += values[i] + " ";
            }
            display += "\n";
        }
        if (answers != null)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                display += answers[i] + " ";
            }
            display += "\n";
        }
        return display;
        
    }

    public static void printTestArray(TestArray testArray){
        String display = "";
        for (int i=0; i<testArray.codes.Length; i++){
            display += testArray.codes[i] + " ";
        }
        display += "\n";
        for (int i=0; i<testArray.values.Length; i++){
            display += testArray.values[i] + " ";
        }
        display += "\n";
        for (int i=0; i<testArray.codes.Length; i++){
            display += testArray.answers[i] + " ";
        }
        display += "\n";
        Console.Out.WriteLine(display);
    }

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX representation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void main(String[] args) {
        TestArray testArray = new TestArray(new char[]{'e', 't', 'r', 'g', 'w'},
                                            new int[]{23, 56, 78, 33, 69},
                                            new bool[]{true, false, true, false, false});

        String filename = "C:\\TestPrimitiveArraysCSharp.xml";
        //print the object
        printTestArray(testArray);
        //object to standard XML
        Easy.save(testArray, filename);
        //load the object from the XML file
        TestArray newTestArray = (TestArray)Easy.load(filename);       
        //print the new object - it is the same as before
        Console.Out.WriteLine(newTestArray);
        //printTestArray(newTestArray);
    }

}

