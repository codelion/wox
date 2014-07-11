using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Reflection;

/**
 * This is a simple object to XML serializer. The SimpleWriter class
 * extends ObjectWriter. It takes a C# object and a XmlTextWriter object.
 * It writes (converts) an object to XML, and writes it using the XmlTextWriter
 * to the specified storage media. The object is represented in standard XML
 * defined by WOX. For more information about the XML representation 
 * please visit: http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: SimpleWriter.cs - 1.0
 */
namespace wox.serial
{
    public class SimpleWriter : ObjectWriter
    {

        Hashtable map;

        int count;
        bool writePrimitiveTypes = true;
        bool doStatic = true;

        // not much point writing out final values - at least yet -
        // the reader is not able to set them (though there's probably
        // a hidden way of doing this
        bool doFinal = false;


        public SimpleWriter()
        {
            //Console.Out.WriteLine("inside SimpleWriter...");
            //System.out.println("inside SimpleWriter Constructor...");
            map = new Hashtable();
            count = 0;

        }

        /**
     * This method is the etry point to write an object in XML. It actually
     * returns a JDOM Element representing the object passed as parameter.
     * @param ob Object - The object to be serialized
     * @return org.jdom.Element - The serialized object
     */
        public override void write(Object ob, XmlTextWriter el)
        {
            if (ob == null)
            {
                // a null object is represented by an empty Object tag with no attributes
                //<object />
                //Console.Out.WriteLine("null object...");
                el.WriteElementString(OBJECT, null);
                //return el;
                //return new Element(OBJECT);
            }
            

            //if the object is already in the map, print its IDREF and not the whole object
            //<object idref="5" />
            //if (map.get(ob) != null)
            else if (map.ContainsKey(ob))
            {
                //Console.Out.WriteLine("object already in the map...");
                el.WriteStartElement(OBJECT);
                //el.WriteAttributeString(IDREF, (string)map[ob]);
                el.WriteAttributeString(IDREF, map[ob].ToString());
                el.WriteEndElement();
                //return el;
                //el = new Element(OBJECT);
                //el.setAttribute(IDREF, map.get(ob).toString());
                //return el;
            }
            // a previously unseen object...
            else
            {
                // a previously unseen object... put is in the map
                //map.put(ob, new Integer(count++));
                //Console.Out.WriteLine("adding the object to the map...");
                map.Add(ob, count++);
                //check if the object can go to string (wrapper objects - Integer, Boolean, etc.)
                if (Util.stringable(ob))
                {
                    //Console.Out.WriteLine("object is stringable...");
                    //Console.Out.WriteLine("typeof object: " + ob.GetType());
                    el.WriteStartElement(OBJECT);                    
                    //Console.Out.WriteLine("ob" + ob + ", " + ob.GetType().ToString());
                    String woxType = (String)mapCSharpToWOX[ob.GetType().ToString()];
                    //Console.Out.WriteLine("woxType: " + woxType);
                    //get the wrapper type, because this is a root object
                    //String woxWrapperType = (String)mapWrapper[woxType];
                    //el.WriteAttributeString(TYPE, woxWrapperType);
                    el.WriteAttributeString(TYPE, woxType);
                    el.WriteAttributeString(VALUE, stringify(ob));
                    el.WriteAttributeString(ID, map[ob].ToString());
                    el.WriteEndElement();
                    /*el = new Element(OBJECT);
                    String woxType = (String)mapJavaToWOX.get(ob.getClass());
                    el.setAttribute(TYPE, woxType);
                    el.setAttribute(VALUE, stringify(ob));*/

                }
                else if (ob is System.Array)
                {
                    //Console.Out.WriteLine("it is an array..." + ob.GetType().ToString());
                    writeArray(ob, el);

                }
                //this is to handle ArrayList objects (added 25 April)
                else if (ob is System.Collections.ArrayList)
                {
                    writeArrayList(ob, el);
                }
                //this is to handle ArrayList objects (added 30 April)
                else if (ob is System.Collections.Hashtable)
                {
                    writeHashtable(ob, el);
                }
                else
                {
                    //Console.Out.WriteLine("it is another type of object..." + ob.GetType().ToString());
                    el.WriteStartElement(OBJECT);
                    el.WriteAttributeString(TYPE, ob.GetType().ToString());
                    el.WriteAttributeString(ID, map[ob].ToString());
                    writeFields(ob, el);
                    el.WriteEndElement();

                    /*el = new Element(OBJECT);
                    el.setAttribute(TYPE, ob.getClass().getName());
                    writeFields(ob, el);*/
                }

                //el.setAttribute(ID, map.get(ob).toString());            
                //el.WriteAttributeString(ID, map[ob].ToString());

                //return el;
            }
        }


        public void writeHashtable(Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAYLIST);
            //Element el = new Element(OBJECT);
            el.WriteStartElement(OBJECT);
            //the type for this object is "map"
            el.WriteAttributeString(TYPE, MAP);
            el.WriteAttributeString(ID, map[ob].ToString());
            //we already know it is an ArrayList, so, we get the underlying array
            //and pass it to the "writeObjectArrayGeneric" method to get it serialized
            Hashtable hashMap = (Hashtable)ob;
            //get the entry set
            IDictionaryEnumerator keys = hashMap.GetEnumerator();
            //el.WriteAttributeString(LENGTH, "" + keys.);
            while (keys.MoveNext())
            {
                //Console.Out.WriteLine("writing an entry...");
                //Console.Out.WriteLine("*KEY* " + keys.Key + ", *OBJECT* " + keys.Value);
                writeMapEntry(keys.Entry, el);
                
            }
            
            el.WriteEndElement();
                        
            /*//for each element in the entry set I have to create an entry object
            Iterator it = keys.iterator();
            while (it.hasNext())
            {
                Map.Entry entryMap = (Map.Entry)it.next();
                el.addContent(writeMapEntry(entryMap));
                //System.out.println("*KEY* " + entryMap.getKey() + ", *OBJECT* " + entryMap.getValue());
            }
            return el;*/
        }

        public void writeMapEntry(Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAYLIST);
            //Element el = new Element(OBJECT);
            el.WriteStartElement(OBJECT);
            //the type for this object is "map"
            //el.setAttribute(TYPE, ENTRY);
            el.WriteAttributeString(TYPE, ENTRY);
            //lets cast the object to a Map.Entry
            DictionaryEntry entry = (DictionaryEntry)ob;
            writeMapEntryKey(entry.Key, el);
            writeMapEntryKey(entry.Value, el);
            el.WriteEndElement();
            //return el;
        }

        public void writeMapEntryKey(Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAYLIST);
            /*Element el = new Element(FIELD);
            //the type for this object is "map"
            el.setAttribute(TYPE, KEY);
            el.addContent(write(ob));
            return el;*/
            //return write(ob);
            write(ob, el);
        }

        public void writeMapEntryValue(Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAYLIST);
            /*Element el = new Element(FIELD);
            //the type for this object is "map"
            el.setAttribute(TYPE, VALUE);
            el.addContent(write(ob));
            return el;*/
            write(ob, el);            
        }






        public void writeArrayList(Object ob, XmlTextWriter el)
        {
            //Console.Out.WriteLine("it is an arraylist...");
            el.WriteStartElement(OBJECT);
            el.WriteAttributeString(TYPE, ARRAYLIST);

            //we already know it is an ArrayList, so, we get the underlying array
            //and pass it to the "writeObjectArrayGeneric" method to get it serialized
            ArrayList list = (ArrayList)ob;
            Object obArray = list.ToArray();

            writeObjectArrayGeneric(ob, obArray, el);
        }



        public void writeArray(Object ob, XmlTextWriter el)
        {
            //a primitive array is an array of any of the following:
            //byte, short, int, long, float, double, char, boolean,
            //Byte, Short, Integer, Long, Float, Double, Character, Boolean, and Class
            //These arrays can go easily to a string with spaces separating their elements.
            if (isPrimitiveArray(ob.GetType().ToString()))
            {
                //Console.Out.WriteLine("-----------------PRIMITIVE ARRAY------------------");
                writePrimitiveArray(ob, el);
            }
            else
            {
                //Console.Out.WriteLine("-----------------NOT A PRIMITIVE ARRAY------------------");
                writeObjectArray(ob, el);
            }
        }


        public void writeObjectArray(Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAY);
            //el.WriteStartElement(ARRAY);
            el.WriteStartElement(OBJECT);
            el.WriteAttributeString(TYPE, ARRAY);

            writeObjectArrayGeneric(ob, ob, el);

        }


        public void writeObjectArrayGeneric(Object obStored, Object ob, XmlTextWriter el)
        {
            //Element el = new Element(ARRAY);
            //el.WriteStartElement(ARRAY);

            //it gets the correct WOX type from the map, in case there is one
            //for example for int[][].class it will get int[][]
            //String woxType = (String)mapJavaToWOX.get(ob.getClass().getComponentType());
            String woxType = (String)mapCSharpToWOX[ob.GetType().GetElementType().ToString()];
            if (woxType == null)
            {
                //woxType = (String)mapArrayJavaToWOX.get(ob.getClass().getComponentType());
                woxType = (String)mapArrayCSharpToWOX[ob.GetType().GetElementType().ToString()];
                if (woxType != null)
                {
                    //el.setAttribute(TYPE, woxType);
                    //el.WriteAttributeString(TYPE, woxType);
                    el.WriteAttributeString(ELEMENT_TYPE, woxType);
                }
                else
                {
                    //this is for arrays of Object
                    if (ob.GetType().GetElementType().ToString().Equals("System.Object"))
                    {
                        //el.WriteAttributeString(TYPE, "Object");
                        el.WriteAttributeString(ELEMENT_TYPE, "Object");
                    }
                    //for the rest of the arrays, we use the appropriate data type
                    else
                    {
                        //el.setAttribute(TYPE, ob.getClass().getComponentType().getName());
                        //el.WriteAttributeString(TYPE, ob.GetType().GetElementType().ToString());
                        el.WriteAttributeString(ELEMENT_TYPE, ob.GetType().GetElementType().ToString());
                    }
                }
            }
            else
            {
                //el.setAttribute(TYPE, woxType);
                //el.WriteAttributeString(TYPE, woxType);
                el.WriteAttributeString(ELEMENT_TYPE, woxType);

            }
            //int len = Array.getLength(ob);
            //el.setAttribute(LENGTH, "" + len);
            Array obArray = (Array)ob;
            int len = obArray.GetLength(0);
            el.WriteAttributeString(LENGTH, "" + len);
            el.WriteAttributeString(ID, map[obStored].ToString());
            //Console.Out.WriteLine("array length: " + len);

            for (int i = 0; i < len; i++)
            {
                //el.addContent(write(Array.get(ob, i)));
                write(obArray.GetValue(i), el);
            }
            el.WriteEndElement();

            //return el;
        }



        public void writePrimitiveArray(Object ob, XmlTextWriter el)
        {
            //       Element el = new Element(ARRAY);
            /*el.WriteStartElement(OBJECT);
            el.WriteAttributeString(TYPE, ob.GetType().ToString());
            el.WriteAttributeString(ID, map[ob].ToString());*/
            //el.WriteStartElement(ARRAY);
            el.WriteStartElement(OBJECT);
            el.WriteAttributeString(TYPE, ARRAY);

            //el.setAttribute(TYPE, ob.getClass().getComponentType().getName());
            //it gets the correct WOX type from the map, in case there is one
            //for example for int[][].class it will get int[][]
            //String woxType = (String)mapJavaToWOX.get(ob.getClass().getComponentType());            
            String woxType = (String)mapCSharpToWOX[ob.GetType().GetElementType().ToString()];
            //el.setAttribute(TYPE, woxType);
            el.WriteAttributeString(ELEMENT_TYPE, woxType);
            //Console.Out.WriteLine("primitive array: " + woxType);
            //we need to get the lenght of the array, so, we cast it to an Array
            Array obArray = (Array)ob;
            int len = obArray.GetLength(0);
            //Console.Out.WriteLine("array length: " + len);
            //CJ this should not be here beacsue the lenght for the byte[] can be different
            //el.setAttribute(LENGTH, "" + len);

            if ((obArray is System.SByte[]))
            {
                //it is a Byte[] array
                //Console.Out.WriteLine("it is a SByte[] array");
                /*if (ob instanceof Byte[]) {
                    System.out.println("Byte[] array");
                    Byte[] arrayWrapperByte = (Byte[])ob;
                    byte[] arrayPrimitiveByte = new byte[arrayWrapperByte.length];
                    for(int k=0; k<arrayWrapperByte.length; k++){
                        arrayPrimitiveByte[k] = arrayWrapperByte[k].byteValue();
                    }
                    el.setText(byteArrayString(arrayPrimitiveByte, el));
                }
                //it is a byte[] array
                else{
                    System.out.println("byte[] array");
                    el.setText(byteArrayString((byte[]) ob, el));
                }*/
            }
            else
            {
                //el.setAttribute(LENGTH, "" + len);
                //el.setText(arrayString(ob, len));
                el.WriteAttributeString(LENGTH, "" + len);
                el.WriteAttributeString(ID, map[ob].ToString());
                el.WriteString(arrayString(obArray, len));
                el.WriteEndElement();
            }
            //return el;*/
        }



        //method modified to include base64 encoding
        /*public String byteArrayString(byte[] a, Element e)
        {
            byte[] target = EncodeBase64.encode(a);
            //set the lenght fro the new encoded array
            e.setAttribute(LENGTH, "" + target.length);
            String strTarget = new String(target);
            return strTarget;
        }*/

        public String arrayString(Array obArray, int len)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                //if it is an array of Class objects
                if (obArray is Type[])
                {
                    //we have to handle null values for arrays of Class objects
                    Type arrayElement = (Type)obArray.GetValue(i);
                    if (arrayElement != null)
                    {
                        //get the correct WOX type if it exists
                        String woxType = (String)mapCSharpToWOX[arrayElement.ToString()];
                        if (woxType != null)
                        {
                            sb.Append(woxType);
                        }
                        else
                        {
                            //get the correct WOX array type if it exists                        
                            woxType = (String)mapArrayCSharpToWOX[arrayElement.ToString()];
                            if (woxType != null)
                            {
                                sb.Append(woxType);
                            }
                            else
                            {
                                sb.Append(arrayElement.ToString());
                            }

                        }
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }
                //if it is an array of char, we must get its unicode representation (June 2007)
                else if (obArray is System.Char[])
                {
                    //we have to handle null values for arrays of Character
                    //this is not necessary for arrays of primitive char because null values do not exist!
                    Object arrayElement = obArray.GetValue(i);
                    if (arrayElement != null)
                    {
                        Char myChar = (Char)obArray.GetValue(i);
                        sb.Append(getUnicodeValue(myChar));
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }

                //if it is an array of boolean, we must convert "True" and "False" to lowercase
                else if (obArray is System.Boolean[])
                {
                    //we have to handle null values for arrays of Boolean
                    //this is not necessary for arrays of primitive boolean because null values do not exist!
                    Object arrayElement = obArray.GetValue(i);
                    String booleanValue = "";
                    if (arrayElement != null)
                    {
                        Boolean myBool = (Boolean)obArray.GetValue(i);
                        if (myBool.ToString().Equals("True"))
                        {
                            booleanValue = "true";
                        }
                        else if (myBool.ToString().Equals("False"))
                        {
                            booleanValue = "false";
                        }
                        sb.Append(booleanValue);
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }

                //for the rest of data types, we just append the values as string
                //it also includes wrapper data types: Integer, Short, Boolean, etc.
                else
                {
                    //we have to handle null values for arrays of wrappers and arrays of Class objects
                    //this is not necessary for arrays of primitives because null values do not exist!
                    //Object arrayElement = Array.get(ob, i);
                    Object arrayElement = obArray.GetValue(i);
                    if (arrayElement != null)
                    {
                        sb.Append(arrayElement.ToString());
                    }
                    else
                    {
                        sb.Append("null");
                    }

                }
            }
            return sb.ToString();
        }





        public void writeFields(Object o, XmlTextWriter parent)
        {
            // get the class of the object
            // get its fields
            // then get the value of each one
            // and call write to put the value in the Element

            Type cl = o.GetType();
            FieldInfo[] fields = getFields(cl);
            String name = null;
            for (int i = 0; i < fields.Length; i++)
            {
                /*if ((doStatic || !Modifier.isStatic(fields[i].getModifiers())) &&
                        (doFinal || !Modifier.isFinal(fields[i].getModifiers())))*/
                try
                {
                    //fields[i].setAccessible(true);
                    name = fields[i].Name;
                    //Console.Out.WriteLine("name: " + name);
                    //Console.Out.WriteLine("name: " + fields[i].Name + ", type: " + fields[i].FieldType + " ,value: " + fields[i].GetValue(person));
                    // need to handle shadowed fields in some way...
                    // one way is to add info about the declaring class
                    // but this will bloat the XML file if we di it for
                    // every field - might be better to just do it for
                    // the shadowed fields
                    // name += "." + fields[i].getDeclaringClass().getName();
                    // fields[i].
                    //Object value = fields[i].get(o);
                    //Console.Out.WriteLine("step 1");
                    Object value = fields[i].GetValue(o);
                    //Console.Out.WriteLine("step 2");
                    //Element field = new Element(FIELD);
                    parent.WriteStartElement(FIELD);
                    //Console.Out.WriteLine("step 3");
                    //field.setAttribute(NAME, name);
                    parent.WriteAttributeString(NAME, name);
                    /*if (shadowed(fields, name)) {
                        field.setAttribute(DECLARED, fields[i].getDeclaringClass().getName());
                    }*/
                    //if the field is a primitive data type (int, float, char, etc.)
                    if (mapCSharpToWOX.ContainsKey(fields[i].FieldType.ToString()))
                    {
                        //Console.Out.WriteLine("it is a primitive: " + fields[i].FieldType);
                        // this is not always necessary - so it's optional
                        if (writePrimitiveTypes)
                        {
                            //field.setAttribute(TYPE, fields[i].getType().getName());
                            parent.WriteAttributeString(TYPE, (string)mapCSharpToWOX[fields[i].FieldType.ToString()]);
                        }
                        //if it is a char primitive, then we must store its unicode value (June 2007)
                        if (fields[i].FieldType.ToString().Equals("System.Char"))
                        {
                            //System.out.println("IT IS A CHAR...");
                            Char myChar = (Char)value;
                            String unicodeValue = getUnicodeValue(myChar);
                            parent.WriteAttributeString(VALUE, unicodeValue);
                            parent.WriteEndElement();
                            //parent.WriteEndElement();
                            //field.setAttribute(VALUE, unicodeValue);
                        }
                        //if it is a boolean primitive, "False" and "True" must be in lowercase
                        else if (fields[i].FieldType.ToString().Equals("System.Boolean"))
                        {
                            String booleanValue = "";
                            if (value == null)
                            {
                                booleanValue = "";
                            }
                            if (value.ToString().Equals("True"))
                            {
                                booleanValue = "true";
                            }
                            else if (value.ToString().Equals("False"))
                            {
                                booleanValue = "false";
                            }
                            parent.WriteAttributeString(VALUE, booleanValue);
                            parent.WriteEndElement();

                        }
                        //for the rest of the primitives, we store their values as string
                        else
                        {
                            //field.setAttribute(VALUE, value.toString());
                            if (value == null)
                            {
                                value = "";
                            }
                            parent.WriteAttributeString(VALUE, value.ToString());
                            parent.WriteEndElement();

                        }

                    }
                    //if the field is NOT a primitive data type (e.g. it is an object)
                    else
                    {
                        //field.addContent(write(value));
                        write(value, parent);
                        parent.WriteEndElement();

                    }
                    //no -> parent.addContent(field);

                }
                catch (Exception e)
                {
                    //Console.Out.WriteLine("error: " + e.Message);
                    //e.printStackTrace();
                    //System.out.println(e);
                    // at least comment on what went wrong
                    //parent.addContent(new Comment(e.toString()));
                }
            }
        }


        private static String getUnicodeValue(char character)
        {
            int asciiValue = (int)character;
            //int asciiValue = Char. (int)character.charValue();
            String hexValue = IntToHex(asciiValue);
            //String hexValue = Integer.toHexString(asciiValue);
            String unicodeValue = "\\u" + fillWithZeros(hexValue);
            //System.out.println("ASCII: " + asciiValue + ", HEX: " + hexValue + ", UNICODE: " + unicodeValue);
            return unicodeValue;
        }

        private static String fillWithZeros(String hexValue)
        {
            int len = hexValue.Length;
            switch (len)
            {
                case 1:
                    return ("000" + hexValue);
                case 2:
                    return ("00" + hexValue);
                case 3:
                    return ("0" + hexValue);
                default:
                    return hexValue;
            }
        }

        //to convert an int value to hexadecimal
        private static string IntToHex(int number)
        {
            return String.Format("{0:x}", number);
        }

        //to convert an hexadecimal value to int
        private static int HexToInt(string hexString)
        {
            return int.Parse(hexString,
                System.Globalization.NumberStyles.HexNumber, null);
        }



        public static String stringify(Object ob)
        {
            //if it is a Class, we only get the class name
            if (ob is Type)
            {
                //we cast it, so we are able to get its fully qualified name
                Type obType = (Type)ob;
                //Console.Out.WriteLine("it is a Type...");
                //Console.Out.WriteLine("obType.ToString(): " + obType.ToString());
                //Console.Out.WriteLine("obType.Name: " + obType.Name);

                //get the correct WOX type if it exists
                //String woxType = (String)mapJavaToWOX.get((Class)ob);                
                //String woxType = (String)mapCSharpToWOX[ob.GetType().ToString()];
                String woxType = (String)mapCSharpToWOX[obType.ToString()];

                if (woxType != null)
                {
                    return woxType;
                }
                else
                {
                    //return ((Class) ob).getName();
                    //return ob.GetType().ToString();
                    return obType.ToString();                    
                }
            }
            // if it is a Character we must get the unicode representation
            else if (ob is Char)
            {
                //Console.Out.WriteLine("object is char...");
                return (getUnicodeValue((Char)ob));
            }
            else if (ob is Boolean)
            {
                //Console.Out.WriteLine("object is bool...");                                
                if (ob.ToString().Equals("True"))
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            //if is is any of the other wrapper classes
            else
            {
                return ob.ToString();
            }
        }


        public static FieldInfo[] getFields(Type c)
        {
            ArrayList v = new ArrayList();
            while (!(c == null))
            { // c.equals( Object.class ) ) {
                FieldInfo[] fields = c.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0; i < fields.Length; i++)
                {
                    //Console.Out.WriteLine("fields[" + i + "]: " + fields[i]);
                    v.Add(fields[i]);
                }
                c = null;
                //c = c.getSuperclass();
            }
            FieldInfo[] f = new FieldInfo[v.Count];
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (FieldInfo)v[i];
            }
            return f;


            /*Vector v = new Vector();
            while (!(c == null))
            { // c.equals( Object.class ) ) {
                Field[] fields = c.getDeclaredFields();
                for (int i = 0; i < fields.length; i++)
                {
                    // System.out.println(fields[i]);
                    v.addElement(fields[i]);
                }
                c = c.getSuperclass();
            }
            Field[] f = new Field[v.size()];
            for (int i = 0; i < f.length; i++)
            {
                f[i] = (Field)v.get(i);
            }
            return f;*/
        }


        public bool isPrimitiveArray(String c)
        {
            for (int i = 0; i < primitiveArrays.Length; i++)
            {
                if (c.Equals(primitiveArrays[i]))
                {
                    return true;
                }
            }

            return false;
        }







    }
}
