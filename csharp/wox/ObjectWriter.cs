using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/**
 * The ObjectWriter abstract class must be extended by object writers
 * (serializers). It defines the write method, which takes a C# object
 * and a XmlTextWriter object, and converts it to XML.
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: ObjectWriter.cs - 1.0
 */
namespace wox.serial
{
    public abstract class ObjectWriter : Serial
    {

        public abstract void write(Object o, XmlTextWriter writer);
        
    }
}
