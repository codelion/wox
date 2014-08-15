using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using System.Text;
using wox.serial;
using System.Reflection;

/**
 * The Easy class is used to serialize/de-serialize objects to/from XML.
 * It has two static methods. The save method serializes an object to XML
 * and stores it in an XML file; and the load method de-serializes an object
 * from an XML file.
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: Easy.cs - 1.0
 */
namespace wox.serial
{
    public class Easy
    {

        public static void save(Object ob, String filename)
        {
            //this creates an XML writer, which will be used to serialize an object to XML
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            //this creates the WOX writer
            ObjectWriter woxWriter = new SimpleWriter();
            //writes the object to XML
            woxWriter.write(ob, writer);
            writer.Close();
            Console.Out.WriteLine("Saved object to " + filename);                      
        }


        public static Object load(String filename)
        {
            //this creates an XML reader, which will be used to de-serialize the object
            XmlTextReader xmlReader = new XmlTextReader(filename);
            //this creates the WOX reader
            ObjectReader woxReader = new SimpleReader();
            //Read the next node from the Stream. In this case it will be the Root Element
            xmlReader.Read();
            //reads the object from the XML file. We pass the xmlReader positioned in the first node!
            Object ob = woxReader.read(xmlReader);
            xmlReader.Close();
            Console.Out.WriteLine("Load object from " + filename);
            return ob;

        }

    }

    

}
