using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

/**
 * The Serial class defines the constants used in the
 * XML representation of objects. It also provides arrays of classes
 * used for mapping primitive types, and primitive arrays.
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version Serial.cs - 1.0
 */
namespace wox.serial
{
    public class Serial {
        // use string constants to enforce consistency
        // between readers and writers
        //public static final String OBJECT = "object";        
        public const String OBJECT = "object";
        public const String FIELD = "field";
        public const String NAME = "name";
        public const String TYPE = "type";
        public const String VALUE = "value";
        public const String ARRAY = "array";
        public const String ARRAYLIST = "list";
        public const String ELEMENT_TYPE = "elementType"; 
        public const String MAP = "map";
        public const String KEY_TYPE = "keyType";
        public const String VALUE_TYPE = "valueType";
        public const String ENTRY = "entry";
        public const String KEY = "key";    
        public const String LENGTH = "length";
        public const String ID = "id";
        public const String IDREF = "idref";

        // next is used to disambiguate shadowed fields
        public const String DECLARED = "declaredClass";

        public String[] primitiveArrays = { 
            "System.Int32[]",
            "System.Boolean[]",
            "System.Byte[]",
            "System.Int16[]",            
            "System.Int64[]",
            "System.Char[]",
            "System.Single[]",
            "System.Double[]",
            "System.Type[]"            
        };


        public String[] primitiveArraysWOX = {
                "int",
                "boolean",
                "byte",
                "short",
                "long",
                "char",
                "float",
                "double",               
                "class"
            };

        public static Type[] primitives =
            {
                typeof(int),
                typeof(bool),
                typeof(sbyte),                
                typeof(short),
                typeof(long),
                typeof(char),
                typeof(float),
                typeof(double),
                typeof(System.Type),
                typeof(System.String)
            };



        /*public const Class[] primitiveArrays =
            new Class[]{
                int[].class,
                boolean[].class,
                byte[].class,
                short[].class,
                long[].class,
                char[].class,
                float[].class,
                double[].class
            };*/

    // now declare the wrapper classes for each primitive object type
    // note that this order must correspond to the order in primitiveArrays

    // there may be a better way of doing this that does not involve
    // wrapper objects (e.g. Integer is the wrapper of int), but I've
    // yet to find it
    // note that the creation of wrapper objects is a significant
    // overhead
    // example: reading an array of 1 million int (all zeros) takes
    // about 900ms using reflection, versus 350ms hard-coded
    /*public static final Class[] primitiveWrappers =
            new Class[]{
                Integer.class,
                Boolean.class,
                Byte.class,
                Short.class,
                Long.class,
                Character.class,
                Float.class,
                Double.class
            };*/

    /*public static final Class[] primitives =
            new Class[]{
                int.class,
                boolean.class,
                byte.class,
                short.class,
                long.class,
                char.class,
                float.class,
                double.class
            };*/


        //**********************************************************
        //09 October 2007
        public static Hashtable mapWOXToCSharp = new Hashtable();
        public static Hashtable mapCSharpToWOX = new Hashtable();
        public static Hashtable mapArrayCSharpToWOX = new Hashtable();
        public static Hashtable mapArrayWOXToCSharp = new Hashtable();
        public static Hashtable mapWrapper = new Hashtable();

        
        //static contsructor is executed only once, when the instance of the class
        //is created, or when a static memeber is referenced. - works for our purpose
        static Serial(){
            //initialise map of WOX data types to C# data types
            mapWOXToCSharp.Add("byte", typeof(System.SByte))   ;
            mapWOXToCSharp.Add("short", typeof(System.Int16));
            mapWOXToCSharp.Add("int", typeof(System.Int32));
            mapWOXToCSharp.Add("long", typeof(System.Int64));
            mapWOXToCSharp.Add("float", typeof(System.Single));
            mapWOXToCSharp.Add("double", typeof(System.Double));
            mapWOXToCSharp.Add("char", typeof(System.Char));
            mapWOXToCSharp.Add("boolean", typeof(System.Boolean));
            mapWOXToCSharp.Add("string", typeof(System.String));
            mapWOXToCSharp.Add("class", typeof(System.Type));
            //mapWOXToCSharp.Add("array", typeof(System.Array));
            //map wrpper types in WOX to primitives in C#
            mapWOXToCSharp.Add("byteWrapper", typeof(System.SByte));
            mapWOXToCSharp.Add("shortWrapper", typeof(System.Int16));
            mapWOXToCSharp.Add("intWrapper", typeof(System.Int32));
            mapWOXToCSharp.Add("longWrapper", typeof(System.Int64));
            mapWOXToCSharp.Add("floatWrapper", typeof(System.Single));
            mapWOXToCSharp.Add("doubleWrapper", typeof(System.Double));
            mapWOXToCSharp.Add("charWrapper", typeof(System.Char));
            mapWOXToCSharp.Add("booleanWrapper", typeof(System.Boolean));
            //initialise map of WOX primitive arrays to C# value type arrays
            mapArrayWOXToCSharp.Add("byte[]", typeof(System.SByte[]));
            mapArrayWOXToCSharp.Add("byte[][]", typeof(System.SByte[][]));
            mapArrayWOXToCSharp.Add("byte[][][]", typeof(System.SByte[][][]));
            mapArrayWOXToCSharp.Add("short[]", typeof(System.Int16[]));
            mapArrayWOXToCSharp.Add("short[][]", typeof(System.Int16[][]));
            mapArrayWOXToCSharp.Add("short[][][]", typeof(System.Int16[][][]));
            mapArrayWOXToCSharp.Add("int[]", typeof(System.Int32[]));
            mapArrayWOXToCSharp.Add("int[][]", typeof(System.Int32[][]));
            mapArrayWOXToCSharp.Add("int[][][]", typeof(System.Int32[][][]));
            mapArrayWOXToCSharp.Add("long[]", typeof(System.Int64[]));
            mapArrayWOXToCSharp.Add("long[][]", typeof(System.Int64[][]));
            mapArrayWOXToCSharp.Add("long[][][]", typeof(System.Int64[][][]));
            mapArrayWOXToCSharp.Add("float[]", typeof(System.Single[]));
            mapArrayWOXToCSharp.Add("float[][]", typeof(System.Single[][]));
            mapArrayWOXToCSharp.Add("float[][][]", typeof(System.Single[][][]));
            mapArrayWOXToCSharp.Add("double[]", typeof(System.Double[]));
            mapArrayWOXToCSharp.Add("double[][]", typeof(System.Double[][]));
            mapArrayWOXToCSharp.Add("double[][][]", typeof(System.Double[][][]));
            mapArrayWOXToCSharp.Add("char[]", typeof(System.Char[]));
            mapArrayWOXToCSharp.Add("char[][]", typeof(System.Char[][]));
            mapArrayWOXToCSharp.Add("char[][][]", typeof(System.Char[][][]));
            mapArrayWOXToCSharp.Add("boolean[]", typeof(System.Boolean[]));
            mapArrayWOXToCSharp.Add("boolean[][]", typeof(System.Boolean[][]));
            mapArrayWOXToCSharp.Add("boolean[][][]", typeof(System.Boolean[][][]));
            //initialise map of WOX arrays (Class and String) to C# arrays
            mapArrayWOXToCSharp.Add("class[]", typeof(System.Type[]));
            mapArrayWOXToCSharp.Add("class[][]", typeof(System.Type[][]));
            mapArrayWOXToCSharp.Add("class[][][]", typeof(System.Type[][][]));
            mapArrayWOXToCSharp.Add("string[]", typeof(System.String[]));
            mapArrayWOXToCSharp.Add("string[][]", typeof(System.String[][]));
            mapArrayWOXToCSharp.Add("string[][][]", typeof(System.String[][][]));


            
            //initialise map of C# data types (primitive) to WOX data types
            mapCSharpToWOX.Add("System.SByte", "byte");
            mapCSharpToWOX.Add("System.Int16", "short");
            mapCSharpToWOX.Add("System.Int32", "int");
            mapCSharpToWOX.Add("System.Int64", "long");
            mapCSharpToWOX.Add("System.Single", "float");
            mapCSharpToWOX.Add("System.Double", "double");
            mapCSharpToWOX.Add("System.Char", "char");
            mapCSharpToWOX.Add("System.Boolean", "boolean");
            //initialise map of C# data types (String, Class and Array) to WOX data types
            mapCSharpToWOX.Add("System.String", "string");
            mapCSharpToWOX.Add("System.Type", "class");
            mapCSharpToWOX.Add("System.RuntimeType", "class");
            mapCSharpToWOX.Add("System.Array", "array");
            //initialise map of C# primitive arrays to WOX arrays
            mapArrayCSharpToWOX.Add("System.SByte[]", "byte[]");
            mapArrayCSharpToWOX.Add("System.SByte[][]", "byte[][]");
            mapArrayCSharpToWOX.Add("System.SByte[][][]", "byte[][][]");
            mapArrayCSharpToWOX.Add("System.Int16[]", "short[]");
            mapArrayCSharpToWOX.Add("System.Int16[][]", "short[][]");
            mapArrayCSharpToWOX.Add("System.Int16[][][]", "short[][][]");
            mapArrayCSharpToWOX.Add("System.Int32[]", "int[]");
            mapArrayCSharpToWOX.Add("System.Int32[][]", "int[][]");
            mapArrayCSharpToWOX.Add("System.Int32[][][]", "int[][][]");
            mapArrayCSharpToWOX.Add("System.Int64[]", "long[]");
            mapArrayCSharpToWOX.Add("System.Int64[][]", "long[][]");
            mapArrayCSharpToWOX.Add("System.Int64[][][]", "long[][][]");
            mapArrayCSharpToWOX.Add("System.Single[]", "float[]");
            mapArrayCSharpToWOX.Add("System.Single[][]", "float[][]");
            mapArrayCSharpToWOX.Add("System.Single[][][]", "float[][][]");
            mapArrayCSharpToWOX.Add("System.Double[]", "double[]");
            mapArrayCSharpToWOX.Add("System.Double[][]", "double[][]");
            mapArrayCSharpToWOX.Add("System.Double[][][]", "double[][][]");
            mapArrayCSharpToWOX.Add("System.Char[]", "char[]");
            mapArrayCSharpToWOX.Add("System.Char[][]", "char[][]");
            mapArrayCSharpToWOX.Add("System.Char[][][]", "char[][][]");
            mapArrayCSharpToWOX.Add("System.Boolean[]", "boolean[]");
            mapArrayCSharpToWOX.Add("System.Boolean[][]", "boolean[][]");
            mapArrayCSharpToWOX.Add("System.Boolean[][][]", "boolean[][][]");
            //initialise map of Java arrays of Class and String to WOX arrays
            mapArrayCSharpToWOX.Add("System.Type[]", "class[]");
            mapArrayCSharpToWOX.Add("System.Type[][]", "class[][]");
            mapArrayCSharpToWOX.Add("System.Type[][][]", "class[][][]");
            mapArrayCSharpToWOX.Add("System.String[]", "string[]");
            mapArrayCSharpToWOX.Add("System.String[][]", "string[][]");
            mapArrayCSharpToWOX.Add("System.String[][][]", "string[][][]");






            //initialise map to use for "wrappers" - root primitive objects
            mapWrapper.Add("byte", "byteWrapper");
            mapWrapper.Add("short", "shortWrapper");
            mapWrapper.Add("int", "intWrapper");
            mapWrapper.Add("long", "longWrapper");
            mapWrapper.Add("float", "floatWrapper");
            mapWrapper.Add("double", "doubleWrapper");
            mapWrapper.Add("char", "charWrapper");
            mapWrapper.Add("boolean", "booleanWrapper");            


        }

    }
}
