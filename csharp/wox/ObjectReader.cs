using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/**
 * The ObjectReader abstract class must be extended by object readers
 * (de-serializers). It defines the read method, which takes a
 * XmlReader object, and returns the live object.
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: ObjectReader.cs - 1.0
 */
namespace wox.serial
{
    public abstract class ObjectReader : Serial
    {

        public abstract Object read(XmlReader reader);

    }
}
