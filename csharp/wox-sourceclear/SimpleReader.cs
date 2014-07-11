using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

/**
 * This is a simple XML to object de-serializer. The SimpleReader class
 * extends ObjectReader. It reads an object from a XmlReader object
 * and puts it back to a live C# object. The XML representation of the
 * object is a standard WOX representation. For more information about
 * the XML representation please visit: http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: SimpleReader.cs - 1.0
 */
namespace wox.serial
{
    public class SimpleReader : ObjectReader
    {

        Hashtable map;

        public SimpleReader()
        {
            //Console.Out.WriteLine("inside SimpleReader...");
            map = new Hashtable();
        }


        //public Object read(Element xob)
        public override Object read(XmlReader xob)
        {

            // there are several possibilities - see how we handle them
            if (empty(xob))
            {
                return null;
            }
            else if (reference(xob))
            {
                //System.out.println("it is a reference: " + xob.getAttributeValue(IDREF) );
                //printMap();
                //return map.get(xob.getAttributeValue(IDREF));
                return map[xob.GetAttribute(IDREF)];
            }

            // at this point we must be reading an actual Object
            // so we need to store it in
            // there are two ways we can handle objects referred to
            // by idrefs
            // the  simplest is to put all objects in an ArrayList or
            // HashMap, and then get retrieve the objects from the collection
            Object ob = null;
            String id = xob.GetAttribute(ID);
            //Console.Out.WriteLine("id: " + id);
            if (isPrimitiveArray(xob))
            {
                //Console.Out.WriteLine("readPrimitiveArray: " + xob.GetAttribute(TYPE));
                ob = readPrimitiveArray(xob, id);
                return ob;
            }

            else if (isObjectArray(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                //Console.Out.WriteLine("readObjectArray: " + xob.GetAttribute(TYPE));
                ob = readObjectArray(xob, id);
                //Console.Out.WriteLine("ready to return array");
                return ob;
            }
            else if (isArrayList(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                ob = readArrayList(xob, id);
            }
            else if (isMap(xob))
            {
                //System.out.println("readObjectArray: " + xob.getAttributeValue(TYPE));
                ob = readMap(xob, id);
            }
            else if (Util.stringable(xob.GetAttribute(TYPE)))
            {
                //Console.Out.WriteLine("it is an stringable object!!!...");
                //System.out.println("readStringObject: " + xob.getAttributeValue(TYPE));
                ob = readStringObject(xob, id);
                return ob;
            }
            else
            { // assume we have a normal object with some fields to set
                //System.out.println("readObject: " + xob.getAttributeValue(TYPE));
                //Console.Out.WriteLine("it is a NORMAL object!!!...");
                ob = readObject(xob, id);
            }
            // now place the object in a collection for later reference
            //System.out.println("ob: " + ob + ", id: " + id);
            return ob;
        }

        //public boolean empty(Element xob)
        public bool empty(XmlReader xob)
        {
            // empty only if it has no attributes and no content
            // System.out.println("Empty test on: " + xob);
            //return !xob.getAttributes().iterator().hasNext() &&
            //!xob.getContent().iterator().hasNext();
            return !xob.HasAttributes && xob.IsEmptyElement;
        }

        //public bool reference(Element xob)
        public bool reference(XmlReader xob)
        {
            //            bool ret = xob.getAttribute(IDREF) != null;
            bool ret = xob.GetAttribute(IDREF) != null;
            // System.out.println("Reference? : " + ret);
            return ret;
        }

        //public boolean primitiveArray(Element xob)
        public bool isPrimitiveArray(XmlReader xob)
        {
            //if (!xob.Name.Equals(ARRAY))
            if (!xob.GetAttribute(TYPE).Equals(ARRAY))
            {
                //Console.Out.WriteLine("Name of the element is not array");
                return false;
            }
            // at this point we must have an array - but is it
            // primitive?  - iterate through all the primitive array types to see
            //String arrayType = xob.GetAttribute(TYPE);
            String arrayType = xob.GetAttribute(ELEMENT_TYPE);
            for (int i = 0; i < primitiveArraysWOX.Length; i++)
            {
                if (primitiveArraysWOX[i].Equals(arrayType))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isObjectArray(XmlReader xob)
        {
            // this actually returns true for any array
            //return xob.Name.Equals(ARRAY);            
            return xob.GetAttribute(TYPE).Equals(ARRAY);
        }


        public bool isArrayList(XmlReader xob)
        {
            // this actually returns true for any arrayList
            return xob.GetAttribute(TYPE).Equals(ARRAYLIST);
        }


        public bool isMap(XmlReader xob)
        {
            // this actually returns true for any map (hashtable in C#)
            return xob.GetAttribute(TYPE).Equals(MAP);
        }        


        //----------

        // now on to the reading methods
        public Object readPrimitiveArray(XmlReader xob, Object id)
        {
            //Console.Out.WriteLine("inside readPrimitiveArray...");
            try
            {
                //Class type = getPrimitiveType(xob.getAttributeValue(TYPE));
                //get the Java type that corresponds to the WOX type
                Type arrayType = (Type)mapWOXToCSharp[xob.GetAttribute(ELEMENT_TYPE)];
                //Console.Out.WriteLine("arrayType: " + arrayType.ToString());
                //get the wrapper type to be able to construct the elements of the array
                //Class wrapperType = getWrapperType(type);
                //System.out.println("type: " + type + ", wrapperType: " + wrapperType);

                int len = Int32.Parse(xob.GetAttribute(LENGTH));
                //Console.Out.WriteLine("length: " + len);
                Array array = Array.CreateInstance(arrayType, len);
                //Console.Out.WriteLine("array created");

                //get array as string. e.g. "67 87 98 87"            
                String st = xob.ReadString();
                //Console.Out.WriteLine("st: " + st);
                char[] seps = { ' ' };
                //get the array elements in as an array of Strings
                String[] values = st.Split(seps);


                //code added by Carlos Jaimez (29th April 2005)
                /*if ((type.equals(byte.class)) || (type.equals(Byte.class))) {
                    Object byteArray = readByteArray(xob);
                    //if it is a Byte array, we have to copy the byte array into it
                    if (type.equals(Byte.class)) {
                        byte[] arrayPrimitiveByte = (byte[])byteArray;
                        Byte[] arrayWrapperByte = new Byte[arrayPrimitiveByte.length];
                        for(int k=0; k<arrayPrimitiveByte.length; k++){
                            arrayWrapperByte[k] = new Byte(arrayPrimitiveByte[k]);
                        }
                        map.put(id, arrayWrapperByte);
                        return arrayWrapperByte;
                    }
                    //if it is a byte array
                    else{
                        map.put(id, byteArray);
                        return byteArray;
                    }
                }*/

                //determine the type primitive type of the array
                if (arrayType.Equals(typeof(System.SByte)))
                {

                }
                else if (arrayType.Equals(typeof(System.Int16)))
                {
                    //Console.Out.WriteLine("array of Int16");
                    Object shortArray = readShortArray((short[])array, values);
                    map.Add(id, shortArray);
                    return shortArray;
                }
                else if (arrayType.Equals(typeof(System.Int32)))
                {
                    //Console.Out.WriteLine("array of Int32");
                    Object intArray = readIntArray((int[])array, values);
                    map.Add(id, intArray);
                    return intArray;
                }
                else if (arrayType.Equals(typeof(System.Int64)))
                {
                    //Console.Out.WriteLine("array of Int64");
                    Object longArray = readLongArray((long[])array, values);
                    map.Add(id, longArray);
                    return longArray;
                }
                else if (arrayType.Equals(typeof(System.Single)))
                {
                    //Console.Out.WriteLine("array of Float");
                    Object floatArray = readFloatArray((float[])array, values);
                    map.Add(id, floatArray);
                    return floatArray;
                }
                else if (arrayType.Equals(typeof(System.Double)))
                {
                    //Console.Out.WriteLine("array of Double");
                    Object doubleArray = readDoubleArray((double[])array, values);
                    map.Add(id, doubleArray);
                    return doubleArray;
                }
                else if (arrayType.Equals(typeof(System.Char)))
                {
                    //Console.Out.WriteLine("array of Char");
                    Object charArray = readCharArray((char[])array, values);
                    map.Add(id, charArray);
                    return charArray;
                }
                else if (arrayType.Equals(typeof(System.Boolean)))
                {
                    //Console.Out.WriteLine("array of Boolean");
                    Object boolArray = readBooleanArray((bool[])array, values);
                    map.Add(id, boolArray);
                    return boolArray;
                }
                else if (arrayType.Equals(typeof(System.Type)))
                {
                    Object classArray = readClassArray((Type[])array, values);
                    map.Add(id, classArray);
                    return classArray;
                }
                else
                {
                    return null;
                }

                map.Add(id, array);
                return array;

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("The exception is: " + e.Message);
                //e.printStackTrace();
                //throw new RuntimeException(e);
            }
            return "";
        }


        public Object readShortArray(short[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int16.Parse(s[i]);
            }
            return a;
        }

        public Object readIntArray(int[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int32.Parse(s[i]);
            }
            return a;
        }

        public Object readLongArray(long[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Int64.Parse(s[i]);
            }
            return a;
        }

        public Object readFloatArray(float[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Single.Parse(s[i]);
            }
            return a;
        }

        public Object readDoubleArray(double[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Double.Parse(s[i]);
            }
            return a;
        }

        public Object readCharArray(char[] a, String[] s)
        {
            //Console.Out.WriteLine("reading array of char");
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                //the token represents the unicode value in the form "\\u0004"
                //Console.Out.WriteLine("s[" + i + "]:" + s[i]);                
                int decimalValue = getDecimalValue(s[i]);
                a[index++] = (char)decimalValue;
                //Console.Out.WriteLine(a[i]);
                //a[index++] = Double.Parse(s[i]);
            }
            return a;
        }

        public Object readBooleanArray(bool[] a, String[] s)
        {
            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                a[index++] = Boolean.Parse(s[i]);
            }
            return a;
        }

        private static int getDecimalValue(String unicodeValue)
        {
            //Console.Out.WriteLine("unicodeValue: " + unicodeValue);
            //first remove the "\\u" part of the unicode value
            //String unicodeModified = unicodeValue.substring(2, unicodeValue.length());
            String unicodeModified = unicodeValue.Substring(2, 4); //unicodeValue.Length);
            //Console.Out.WriteLine("unicodeModified: " + unicodeModified);
            //System.out.println("unicodeModified: " + unicodeModified);
            //int decimalValue = Int32.Parse(unicodeModified);
            int decimalValue = HexToInt(unicodeModified);
            //Console.Out.WriteLine("decimalValue:" + decimalValue);            
            return decimalValue;
        }

        //to convert an hexadecimal value to int
        private static int HexToInt(string hexString)
        {
            return int.Parse(hexString,
                System.Globalization.NumberStyles.HexNumber, null);
        }



        //-----------------------------------------------------------------------------
        /**
         * Purpose: To constuct the byte array based on the a and xob
         * Befor constructs back the byte array, it has to be decoded
         * Carlos Jaimez (29 april 2005)
         * @param a
         * @param xob
         * @return : int Array
         */
        /*public Object readByteArray(Element xob) {
            //get the encoded base64 text from the XML
            String strByte = xob.getText();
            //get the bytes from the string
            byte[] encodedArray = strByte.getBytes();
            System.out.println("encoded.length: " + encodedArray.length);
            //decode the source byte[] array
            byte[] decodedArray = EncodeBase64.decode(encodedArray);
            System.out.println("decoded.length: " + decodedArray.length);
            //return the real decoded array of byte
            return decodedArray;
        }*/




        public Object readClassArray(Type[] a, String[] s)
        {

            int index = 0;
            //for every element in the array
            for (int i = 0; i < a.Length; i++)
            {
                if (s[i].Equals("null"))
                {
                    a[index++] = null;
                }
                else
                {
                    Type cSharpClass = (Type)mapWOXToCSharp[s[i]];
                    //if the data type was NOT found in the map
                    if (cSharpClass == null)
                    {
                        cSharpClass = (Type)mapArrayWOXToCSharp[s[i]];
                        //if the data type was NOT found in the array map
                        if (cSharpClass == null)
                        {
                            try
                            {
                                //Console.Out.WriteLine("class NOT found in any of the maps: " + s[i]);
                                //a[index++] = Class.forName(s);
                                Type typeObject = Type.GetType(s[i]);
                                //Console.Out.WriteLine("typeObject: " + typeObject);
                                //checking if the type is not null
                                if (typeObject == null)
                                {
                                    //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                                    //try getting the type from the assembly that call this one...
                                    Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                    typeObject = MyAssembly.GetType(s[i]);
                                    //Console.Out.WriteLine("NOW typeObject: " + typeObject);
                                }
                                a[index++] = typeObject;
                            }
                            //catch(java.lang.ClassNotFoundException e){
                            catch (Exception e)
                            {
                                //e.printStackTrace();
                                Console.Out.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            //Console.Out.WriteLine("WOX type: " + s[i] + ", CSharp type (array): " + cSharpClass);
                            a[index++] = cSharpClass;
                        }
                    }
                    else
                    {
                        //Console.Out.WriteLine("WOX type: " + s[i] + ", CSharp type: " + cSharpClass);
                        a[index++] = cSharpClass;
                    }

                }

                //a[index++] = Boolean.Parse(s[i]);
            }
            return a;
        }


        public Object readMap(XmlReader xob, Object id)
        {
            //Console.Out.WriteLine("Reading a Map...");
            Hashtable myHashtable = new Hashtable();

            //get the subtree of the MAP
            XmlReader xobSubTree = xob.ReadSubtree();
            //position the cursor at the beginning of the MAP <object type="map">
            xobSubTree.Read();
            //iterate all its entry objects <object type="entry">
            while (xobSubTree.Read())
            {
                //Console.Out.WriteLine("type subtree: " + xobSubTree.GetAttribute(TYPE));
                //we will only process Elements, and no EndElements
                if (xobSubTree.NodeType == XmlNodeType.Element)
                {
                    //Console.Out.WriteLine("about to read the node..." + xobSubTree.NodeType);
                    //Console.Out.WriteLine("this node contains: " + xob.ReadString());
                    //I will read the key
                    //XmlReader xobKey = xobSubTree.ReadSubtree();

                    //position the cursor in the KEY object
                    xobSubTree.Read();
                    //Console.Out.WriteLine("type key: " + xobSubTree.GetAttribute(TYPE));
                    //Read the KEY object
                    Object myKey = read(xobSubTree);
                    //position the cursor in the VALUE object
                    xobSubTree.Read();
                    //Console.Out.WriteLine("type value: " + xobSubTree.GetAttribute(TYPE));
                    //Read the VALUE object
                    Object myValue = read(xobSubTree);
                    //Add the key and the value to the Hashtable
                    myHashtable.Add(myKey, myValue);                    
                }
            }
            map.Add(id, myHashtable);
            return myHashtable;           
        }


        public Object readArrayList(XmlReader xob, Object id)
        {
            //Console.Out.WriteLine("Reading an ArrayList...");
            Array array = (Array)readObjectArrayGeneric(xob, id);
            ArrayList list = new ArrayList();
            //populate the ArrayList with the array elements
            for (int i = 0; i < array.GetLength(0); i++)
            {
                list.Add(array.GetValue(i));
            }
            map.Add(id, list);
            return list;
        }

        public Object readObjectArray(XmlReader xob, Object id)
        {
            Object array = readObjectArrayGeneric(xob, id);
            map.Add(id, array);
            return array;
        }

        //-----------------------------------------------------------------------------
        public Object readObjectArrayGeneric(XmlReader xob, Object id)
        {
            // to read an object array we first determine the
            // class of the array - leave this to a separate method
            // since there seems to be no automatic way to get the
            // type of the array

            //System.out.println("--------------------READ OBJECT ARRAY");
            try
            {
                //String arrayTypeName = xob.GetAttribute(TYPE);
                String arrayTypeName = xob.GetAttribute(ELEMENT_TYPE);
                int len = Int32.Parse(xob.GetAttribute(LENGTH));
                //Console.Out.WriteLine("type: " + arrayTypeName + ", len: " + len);
                Type componentType = getObjectArrayComponentType(arrayTypeName);
                //Console.Out.WriteLine("componentType: " + componentType);
                Array array = Array.CreateInstance(componentType, len);
                //Console.Out.WriteLine("array created...");
                //map.Add(id, array);
                // now fill in the array

                //get the subtree of the <array>
                XmlReader xobSubTree = xob.ReadSubtree();
                //position the cursor in the <array> element
                xobSubTree.Read();

                int index = 0;
                //for every node (element) in the array            
                while (xobSubTree.Read())
                {
                    //we will only process Elements, and no EndElements
                    if (xobSubTree.NodeType == XmlNodeType.Element)
                    {
                        //Console.Out.WriteLine("about to read the node..." + xobSubTree.NodeType);
                        //Console.Out.WriteLine("this node contains: " + xob.ReadString());
                        Object childArray = read(xobSubTree);
                        //Console.Out.WriteLine(index + " child: " + childArray);
                        array.SetValue(childArray, index++);
                    }
                }
                //Console.Out.WriteLine("returning array from readObjectArray");

                /*List children = xob.getChildren();
                int index = 0;
                for (Iterator i = children.iterator(); i.hasNext();) {
                    //System.out.println("before reading...");
                    Object childArray = read((Element) i.next());
                    //System.out.println(index + " child: " + childArray);
                    Array.set(array, index++, childArray);
                }*/

                return array;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception is: " + e.Message);
                //e.printStackTrace();
                //throw new RuntimeException(e);
                return null;
            }
        }

        public Type getObjectArrayComponentType(String arrayTypeName)
        {
            //throws Exception {
            // System.out.println("Getting class for: " + arrayTypeName);

            //we first look for the Java type in the map
            //Class javaClass = (Class)mapWOXToJava.get(arrayTypeName);
            Type cSharpClass = (Type)mapWOXToCSharp[arrayTypeName];
            //if the type was not found, we now look for it in the array map
            if (cSharpClass == null)
            {
                //Console.Out.WriteLine("cSharpClass was NULL... 1st time...");
                cSharpClass = (Type)mapArrayWOXToCSharp[arrayTypeName];
                //if the type is not in the array map
                if (cSharpClass == null)
                {
                    //Console.Out.WriteLine("WOX type not found in any of the maps...");
                    if (arrayTypeName.Equals("Object"))
                    {
                        //Console.Out.WriteLine("It is an array of Object...");
                        arrayTypeName = "System.Object";
                    }
                    Type typeObject = Type.GetType(arrayTypeName);
                    //Console.Out.WriteLine("typeObject: " + typeObject);
                    //checking if the type is not null
                    if (typeObject == null)
                    {
                        //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                        //try getting the type from the assembly that call this one...
                        Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                        typeObject = MyAssembly.GetType(arrayTypeName);
                        //Console.Out.WriteLine("NOW typeObject: " + typeObject);
                    }
                    return typeObject;
                }
                else
                {
                    //Console.Out.WriteLine("returning cSharpClass from mapArrayWOXToCSharp");
                    return cSharpClass;
                }
            }
            else
            {
                //Console.Out.WriteLine("returning cSharpClass from mapWOXToCSharp");
                return cSharpClass;
            }

            //        String componentTypeName = arrayTypeName.substring(1);
            //        System.out.println("Component type name: " + componentTypeName);
            //        Class componentType = Class.forName(componentTypeName);
            //        System.out.println("Component type: " + componentType);
            //        return componentType;
        }





        ///---------------
        ///


        public Object readStringObject(XmlReader xob, Object id)
        {
            try
            {
                //get the CSharp type that corresponds to the WOX type in the XML
                Type cSharpType = (Type)mapWOXToCSharp[xob.GetAttribute(TYPE)];
                //Class type = Class.forName(xob.getAttributeValue(TYPE));
                // System.out.println("Declared: ");
                // print(type.getDeclaredConstructors());
                // System.out.println("All?: ");
                // print(type.getConstructors());
                // System.out.println("Type: " + type);
                // System.out.println("Text: " + xob.getText());
                // AccessController.doPrivileged(null);
                // PrivilegedAction action

                // handle class objects differently
                if (cSharpType.Equals(typeof(System.Type)))
                {
                    //look for the Java class that corresponds to the WOX type
                    Type cSharpClass = (Type)mapWOXToCSharp[xob.GetAttribute(VALUE)];
                    //if not found, look for it in the array map
                    if (cSharpClass == null)
                    {
                        cSharpClass = (Type)mapArrayWOXToCSharp[xob.GetAttribute(VALUE)];
                        //if not found, load it
                        if (cSharpClass == null)
                        {
                            //Console.Out.WriteLine("NOT found in any of the arrays: " + xob.GetAttribute(VALUE));
                            //Object obClass = Type.GetType(xob.GetAttribute(VALUE));

                            //Console.Out.WriteLine("type: " + xob.GetAttribute(VALUE));
                            Object obClass = Type.GetType(xob.GetAttribute(VALUE));
                            //Console.Out.WriteLine("obClass: " + obClass);
                            //checking if the type is not null
                            if (obClass == null)
                            {
                                //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                                //try getting the type from the assembly that call this one...
                                Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                obClass = MyAssembly.GetType(xob.GetAttribute(VALUE));
                                //Console.Out.WriteLine("NOW obClass: " + obClass);
                            }

                            map.Add(id, obClass);  //added Oct 2006
                            return obClass;
                        }
                        //if found in the array map
                        else
                        {
                            //Console.Out.WriteLine("Found in the Array Map: " + cSharpClass);
                            map.Add(id, cSharpClass);  //added Oct 2006
                            return cSharpClass;
                        }
                    }
                    //if found in the first map
                    else
                    {
                        //Console.Out.WriteLine("Found in the First Map: " + cSharpClass);
                        map.Add(id, cSharpClass);  //added Oct 2006
                        return cSharpClass;
                    }



                    //System.out.println("type: " + type + ", text: " + xob.getText());
                    //if it was a primitive class (i.e. double, boolean, etc.), then get it from the map
                    /*System.out.println("xob.getText()" + xob.getAttributeValue(VALUE));
                    Object primitiveClass = primitivesMap.get(xob.getAttributeValue(VALUE));
                    if (primitiveClass != null){
                         map.put(id, primitiveClass);  //added Oct 2006
                        return ((Class)primitiveClass);
                    }
                    //otherwise load the appropriate class and return it
                    //Object obClass = Class.forName(xob.getText());
                    Object obClass = Class.forName(xob.getAttributeValue(VALUE));
                    map.put(id, obClass);  //added Oct 2006
                    return obClass;*/
                }
                /******************************************/
                /*else if (type.equals(java.util.concurrent.atomic.AtomicLong.class)){
                    //System.out.println("it is atomic long...");
                    Class[] st = {long.class};
                    Constructor cons = type.getDeclaredConstructor(st);
                    // System.out.println("String Constructor: " + cons);
                    Object ob = makeObject(cons, new Object[]{new Long(xob.getText())}, id);
                    return ob;

                } */
                /********************************************/
                else
                {
                    //if it is a Character object - special case because Character has no constructor
                    //that takes a String. It only has a constructor that takes a char value
                    if (cSharpType.Equals(typeof(System.Char)))
                    {
                        //int decimalValue = getDecimalValue(xob.getText());
                        int decimalValue = getDecimalValue(xob.GetAttribute(VALUE));
                        Char charObject = (char)decimalValue;
                        //Console.Out.WriteLine("decimalvalue: " + decimalValue + ", charObject: " + charObject);
                        return charObject;
                        /*System.out.println("it is CHAR!!!");
                        st = new Class[]{char.class};
                        System.out.println("charText: " + charText + ", decimalValue: " + );*/
                    }
                    //for the rest of the Wrapper objects - they have constructors that take "String"
                    else if (cSharpType.Equals(typeof(System.SByte)))
                    {
                        return SByte.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int16)))
                    {
                        return Int16.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int32)))
                    {
                        return Int32.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Int64)))
                    {
                        return Int64.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Single)))
                    {
                        return Single.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Double)))
                    {
                        return Double.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.Boolean)))
                    {
                        return Boolean.Parse(xob.GetAttribute(VALUE));
                    }
                    else if (cSharpType.Equals(typeof(System.String)))
                    {
                        return xob.GetAttribute(VALUE);
                    }
                    else
                    {


                        ///CHECK!!!!!!!!!!!!!!  April 2008
                        /*Class[] st = {String.class};
                         Constructor cons = type.getDeclaredConstructor(st);
                         //Object ob = makeObject(cons, new String[]{xob.getText()}, id);
                         Object ob = makeObject(cons, new String[]{xob.getAttributeValue(VALUE)}, id);
                         return ob;*/
                        return null;

                    }

                }
            }
            catch (Exception e)
            {

                //e.printStackTrace();
                Console.Out.WriteLine(e.Message);
                // System.out.println("While trying: " type );
                return null;
                // throw new RuntimeException(e);
            }

        }




        //////from here
        public Object readObject(XmlReader xob, Object id)
        {
            //Console.Out.WriteLine("inside readObject...");
            // to read in an object we iterate over all the field elements
            // setting the corresponding field in the Object
            // first we construct an object of the correct class
            // this class may not have a public default constructor,
            // but will have a private default constructor - so we get
            // this back
            try
            {
                //System.out.println("Type: " + xob.getAttributeValue(TYPE));
                //System.out.println("Element: " + xob.getName());

                //Class type = Class.forName(xob.getAttributeValue(TYPE));
                //Console.Out.WriteLine("type: " + xob.GetAttribute(TYPE));
                Type typeObject = Type.GetType(xob.GetAttribute(TYPE));
                //Console.Out.WriteLine("typeObject: " + typeObject);
                //checking if the type is not null
                if (typeObject == null)
                {
                    //Console.Out.WriteLine("typeObject is not in this assembly. Getting it from another.");
                    //try getting the type from the assembly that call this one...
                    Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    typeObject = MyAssembly.GetType(xob.GetAttribute(TYPE));
                    //Console.Out.WriteLine("NOW typeObject: " + typeObject);
                }
                
                //Console.Out.WriteLine("typeObject: " + typeObject);
                //Util.testReflection(typeObject);


                //System.out.println("type: " + type + ", TYPE: " + xob.getAttributeValue(TYPE));
                // System.out.println("Declared: ");
                // print(type.getDeclaredConstructors());
                // System.out.println("All?: ");
                // print(type.getConstructors());
                // AccessController.doPrivileged(null);
                // PrivilegedAction action

                // put the forced call in here!!!
                // Constructor cons = type.getDeclaredConstructor(new Class[0]);
                ConstructorInfo cons = Util.forceDefaultConstructor(typeObject);
                //Console.Out.WriteLine("cons: " + cons);
                //cons.setAccessible(true);


                //System.out.println("Default Constructor: " + cons + ", id is: " + id);
                //this.printMap();
                Object ob = makeObject(cons, new Object[0], id);
                //Console.Out.WriteLine("ob: " + ob);
                //System.out.println("ob before instanceof: " + ob);
                //boolean bbb = ob instanceof Method;
                //System.out.println("ob instanceof Method: " +  bbb + ", ob is: " + ob);
                // now go through setting all the fields
                setFields(ob, xob);
                //System.out.println("after setFields(). ob is: " +  ob);

                /////to here
                /************************************************************/
                //if the TYPE is "java.lang.reflect.Method", this is a special case. We will construct
                //the method by invoking the getMethod method
                //System.out.println("/*********************************************************************/");
                /*           if (xob.getAttributeValue(TYPE).equals("java.lang.reflect.Method")){
                               //we get the info about the method
                               String methodName = "";
                               String className = "";
                               Class[] methodParameters = null;
                               for (Iterator i = xob.getChildren().iterator(); i.hasNext();) {
                                   Element fe = (Element) i.next();
                                   String name = fe.getAttributeValue(NAME);
                                   if (name.equals("clazz"))
                                       className = fe.getChild(OBJECT).getText();
                                   else if (name.equals("name"))
                                       methodName = fe.getChild(OBJECT).getText();
                                   else if (name.equals("parameterTypes")){
                                       System.out.println("getting the parameter types...");
                                       Element child = (Element) fe.getChildren().iterator().next();
                                       methodParameters = (Class[]) read(child);
                                   }
                                       //System.out.println("parameterTypes");
                                   //System.out.println("fe: " + fe);
                               }
                               System.out.println("CLASS: " + className + ", METHOD: " + methodName);
                               System.out.println("methodParameters.length: " + methodParameters.length);
                               for(int g=0; g<methodParameters.length; g++){
                                   System.out.println("m[" + g + "]: " + methodParameters[g]);
                               }

                               Class myClass = Class.forName(className);
                               Method method = myClass.getMethod(methodName, methodParameters);
                               System.out.println("method is: "  + method);

                               //Element className = xob.getChild("");
                               //System.out.println("xxx: " + xob.getParentElement());
                           }                                                                   */
                //System.out.println("/*********************************************************************/");


                /********************************************************************/


                //////from here

                return ob;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                //e.printStackTrace();
                return null;
                // throw new RuntimeException(e);
            }
        }

        //////// to here



        // this method not only makes the object, but also places
        // it in the HashMap of object references
        public Object makeObject(ConstructorInfo cons, Object[] args, Object key)
        {
            //System.out.println("STEP 1");
            //cons.setAccessible(true);
            //System.out.println("STEP 2");
            //Console.Out.WriteLine("s1 cons: " + cons);
            Object value = cons.Invoke(args);
            //Console.Out.WriteLine("s2");
            //System.out.println("value is: " + value);
            map.Add(key, value);
            //Console.Out.WriteLine("s3");
            return value;
        }


        public FieldInfo getField(Type typeObject, String name)
        {
            //Console.Out.WriteLine("typeObject: " + typeObject + ", name: " + name);
            // System.out.println(type + " :::::: " + name);
            if (typeObject == null)
            {
                return null;
            }
            try
            {
                // throws an exception if there's no such field
                //Console.Out.WriteLine("getting the field...");
                FieldInfo[] fields = typeObject.GetFields(BindingFlags.Instance
                                                       | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo field = null;
                for (int i = 0; i < fields.Length; i++)
                {
                    //Console.Out.WriteLine("fields[" + i + "]: " + fields[i]);
                    if (fields[i].Name.Equals(name))
                    {
                        //Console.Out.WriteLine("This is the field I WANT: " + name);
                        field = fields[i];
                        break;
                    }
                }
                //Console.Out.WriteLine("field retrieved...");
                //Console.Out.WriteLine("getField. field: " + field.Name);
                return field;
            }
            catch (Exception e)
            {
                // try the superclass instead
                //return getField(typeObject.getSuperclass(), name);
                Console.Out.WriteLine(e.Message);
                return null;
            }
        }




        public void setFields(Object ob, XmlReader xob)
        {
            // iterate over the set of fields
            //Class type = ob.getClass();
            Type typeObject = ob.GetType();
            //Console.Out.WriteLine("typeObject: " + typeObject);

            //XmlSubtreeReader xobSubtree = (XmlSubtreeReader)xob.ReadSubtree();
            //Console.Out.WriteLine("subtree");
            XmlReader xobSubtree = xob.ReadSubtree();
            xobSubtree.Read();
            while (xobSubtree.Read())
            {
                //for (Iterator i = xob.getChildren().iterator(); i.hasNext();) {
                //Element fe = (Element) i.next();
                //we will only process Elements, and no EndElements
                if (xobSubtree.NodeType == XmlNodeType.Element)
                {
                    String name = xobSubtree.GetAttribute(NAME);
                    //Console.Out.WriteLine("name: " + name);
                    //System.out.println("name: " + name);
                    // ignore shadowing for now...
                    //String declaredType = fe.getAttributeValue(DECLARED);
                    try
                    {
                        //Class declaringType;
                        //if (declaredType != null) {
                        //    declaringType = Class.forName(declaredType);
                        //} else {
                        //    declaringType = type;
                        //}
                        Type declaringType = typeObject;
                        //Console.Out.WriteLine("declaringType: " + declaringType);
                        //System.out.println("Field name: " + name + " belonging to: " + declaringType);
                        FieldInfo field = getField(declaringType, name);
                        //Console.Out.WriteLine("field: " + field);
                        //field.setAccessible(true);
                        Object value = null;
                        if (Util.primitive(field.FieldType))
                        {
                            // System.out.println("Primitive");
                            //System.out.println("setfield... primitive...type: " + fe.getAttributeValue(TYPE));
                            string attributeType = xobSubtree.GetAttribute(TYPE);
                            if (attributeType.Equals("char"))
                            {
                                //Console.Out.WriteLine("this field is char");
                                int decimalValue = getDecimalValue(xobSubtree.GetAttribute(VALUE));
                                Char charObject = (char)decimalValue;
                                //System.out.println("decimalvalue: " + decimalValue + ", charObject: " + charObject);
                                value = charObject;
                            }
                            else if (attributeType.Equals("byte"))
                            {
                                //Console.Out.WriteLine("this field is byte");
                                value = SByte.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("short"))
                            {
                                //Console.Out.WriteLine("this field is short");
                                value = Int16.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("int"))
                            {
                                //Console.Out.WriteLine("this field is int");
                                value = Int32.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("long"))
                            {
                                //Console.Out.WriteLine("this field is long");
                                value = Int64.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("float"))
                            {
                                //Console.Out.WriteLine("this field is float");
                                value = Single.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("double"))
                            {
                                //Console.Out.WriteLine("this field is double");
                                value = Double.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("boolean"))
                            {
                                //Console.Out.WriteLine("this field is boolean");
                                value = Boolean.Parse(xobSubtree.GetAttribute(VALUE));
                            }
                            else if (attributeType.Equals("string"))
                            {
                                //Console.Out.WriteLine("this field is string");
                                value = xobSubtree.GetAttribute(VALUE);
                            }
                            else if (attributeType.Equals("class"))
                            {
                                //Console.Out.WriteLine("this field is class");
                                //value = Type.GetType(xobSubtree.GetAttribute(VALUE));

                                //Console.Out.WriteLine("value: " + xobSubtree.GetAttribute(VALUE));
                                value = Type.GetType(xobSubtree.GetAttribute(VALUE));
                                //Console.Out.WriteLine("value: " + value);
                                //checking if the type is not null
                                if (value == null)
                                {
                                    //Console.Out.WriteLine("value is not in this assembly. Getting it from another.");
                                    //try getting the type from the assembly that call this one...
                                    Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
                                    value = MyAssembly.GetType(xobSubtree.GetAttribute(VALUE));
                                    //Console.Out.WriteLine("NOW value: " + value);
                                }
                            }
                            //else if (cSharpType.Equals(typeof(System.String)))
                            //{
                            //    return xob.GetAttribute(VALUE);
                            //}
                            else
                            {
                                value = null;
                            }

                            //else{
                            //    value = makeWrapper(field.getType(), fe.getAttributeValue(VALUE));
                            //}
                        }
                        //it means that the datatype is a Wrapper or a String
                        /*else if (mapWOXToJava.get(fe.getAttributeValue(TYPE))!= null){
                            Class typeWrapper = (Class)TypeMapping.mapWOXToJava.get(fe.getAttributeValue(TYPE));
                            //if it is a Character object - special case because Character has no constructor
                            //that takes a String. It only has a constructor that takes a char value
                            if (typeWrapper.equals(Character.class)){
                                //int decimalValue = getDecimalValue(xob.getText());
                                int decimalValue = getDecimalValue(fe.getAttributeValue(VALUE));
                                Character charObject = new Character((char)decimalValue);
                                System.out.println("decimalvalue: " + decimalValue + ", charObject: " + charObject);
                                value = charObject;
                                //return charObject;
                                //System.out.println("it is CHAR!!!");
                                //st = new Class[]{char.class};
                                //System.out.println("charText: " + charText + ", decimalValue: " + );
                            }
                            //for the rest of the Wrapper objects - they have constructors that take "String"
                            else{
                                Class[] st = {String.class};
                                Constructor cons = typeWrapper.getDeclaredConstructor(st);
                                cons.setAccessible(true);
                                //System.out.println("STEP 2");
                                value = cons.newInstance(new String[]{fe.getAttributeValue(VALUE)});
                                //Object ob = makeObject(cons, new String[]{xob.getText()}, id);
                                //Object ob2 = makeObject(cons, new String[]{fe.getAttributeValue(VALUE)});
                                ///value = ob2;
                                //return ob;
                            }

                        }*/
                        else
                        {
                            //Console.Out.WriteLine("this field is an object!!!");
                            // must be an object with only one child
                            //System.out.println("Object");
                            //Element child = (Element) fe.getChildren().iterator().next();                    
                            //this is to jump to the next element, which is actually the <object> element 
                            xobSubtree.Read();
                            //Console.Out.WriteLine("length: " + xobSubtree.GetAttribute(LENGTH));
                            //get the subtree for the <object>
                            XmlReader xobSubSubTree = xobSubtree.ReadSubtree();
                            //and position the cursor in the <object> element
                            xobSubSubTree.Read();
                            //Console.Out.WriteLine("length subSubtree: " + xobSubSubTree.GetAttribute(LENGTH));
                            value = read(xobSubSubTree);
                        }
                        //System.out.println("  Setting: " + field);
                        // System.out.println("  of: " + ob);
                        //System.out.println("  to: " + value );
                        //field.set(ob, value);
                        field.SetValue(ob, value);
                        // still need to retrieve the value of this object!!!
                        // how to do that?
                        // well - either the Object is stringable (e.g. String or
                        // so at this stagw we either determine the value of the
                        // field directly, or otherwise
                    }
                    catch (Exception e)
                    {
                        Console.Out.WriteLine("Exception is: " + e.Message);
                        // e.printStackTrace();
                        // throw new RuntimeException(e);
                        //System.out.println(name + " : " + e);

                    }
                }

            }

        }








    }









}



