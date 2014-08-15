using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/**
 * The Util class provides static methods that are used by
 * SimpleReader and SimpleWriter. The methods of
 * this class are used by the serialization and de-serialization processes.
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: Util.cs - 1.0
 */
namespace wox.serial
{
    class Util
    {

        public static void main(String[] args)
        {
            //TestDataType testObject = new TestDataType(15);
            //testReflection(testObject);

            Type typeObject = Type.GetType("serializer.Student");
            //Console.Out.WriteLine("typeObject: " + typeObject);
            ConstructorInfo defaultConstructor = typeObject.GetConstructor(System.Type.EmptyTypes);
            //Console.Out.WriteLine("defaultConstructor: " + defaultConstructor);
            Object value = defaultConstructor.Invoke(null);
            //Console.Out.WriteLine("value: " + value);


        }

        //This version of the method is used by the WOX writer (serializer)
        public static bool stringable(Object o)
        {
            // assume the following types go easily to strings...
            bool val = (o is SByte) ||
                       (o is Double) ||
                       (o is Single) ||
                       (o is Int32) ||
                       (o is Int64) ||
                       (o is Int16) ||
                       (o is Boolean) ||
                       (o is Char) ||
                       (o is Type) ||
                       (o is String);
            // System.out.println("Stringable: " + o + " : " + val + " : " + o.getClass());
            return val;
        }


        //This version of the method is used by the WOX reader (de-serializer)
        /*public static boolean stringable(Class type) {
            // assume the following types go easily to strings...
            boolean val = (Byte.class.isAssignableFrom(type) ) ||
                          (Double.class.isAssignableFrom(type) ) ||
                          (Float.class.isAssignableFrom(type) ) ||
                          (Integer.class.isAssignableFrom(type) ) ||
                          (Long.class.isAssignableFrom(type) ) ||
                          (Short.class.isAssignableFrom(type) ) ||
                          (Boolean.class.isAssignableFrom(type)) ||
                          (Character.class.isAssignableFrom(type) ) ||
                          (Class.class.equals(type)) ||
                          (String.class.equals(type));
            // System.out.println("Stringable: " + type + " : " + val);
            return val;
        }*/

        //This version of the method is used by the WOX reader (de-serializer)
        public static bool stringable(String name)
        {
            //Console.Out.WriteLine("inside method stringable(String name)....");
            // assume the following types go easily to strings...
            // System.out.println("Called (String) version");
            try
            {
                //Class type = Class.forName(name);
                //Class type = Class.forName(name);
                //return stringable(type);
                //first we need to get the real data type
                Type realDataType = (Type)Serial.mapWOXToCSharp[name];
                if (realDataType != null)
                {
                    //Console.Out.WriteLine("real data type is: " + realDataType);
                    return true;
                }
                else
                {
                    //Console.Out.WriteLine("it is not stringable!!!...");
                    return false;
                }
                //return stringable(realDataType);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        /**
    * Returns a no-arg constructor that
    * despite appearences can be used to construct objects
    * of the specified type!!!of first non-serializable
    */
        /*public static Constructor forceDefaultConstructor(Class cl) throws Exception {
            Constructor cons = Object.class.getDeclaredConstructor(new Class[0]);
            cons = reflFactory.newConstructorForSerialization(cl, cons);
            cons.setAccessible(true);
            // System.out.println("Cons: " + cons);
            return cons;
        }*/


        public static ConstructorInfo forceDefaultConstructor(Type cl)
        {
            ConstructorInfo defaultConstructor = cl.GetConstructor(System.Type.EmptyTypes);
            return defaultConstructor;
        }


        public static void testReflection(Object testObject)
        {
            Type objectType = testObject.GetType();
            testReflection(objectType);

        }

        public static void testReflection(Type objectType)
        {
            
            ConstructorInfo defaultConstructor = objectType.GetConstructor(System.Type.EmptyTypes);
            ConstructorInfo[] info = objectType.GetConstructors();
            MethodInfo[] methods = objectType.GetMethods();

            Object ob = defaultConstructor.Invoke(new Object[0]);
            //Console.Out.WriteLine("ob: " + ob);

            //defaultConstructor.Invoke(
            //print the defaul constructor
            //Console.WriteLine("deafult ctor: " + defaultConstructor);
            // get all the constructors
            //Console.WriteLine("Constructors:");
            foreach (ConstructorInfo cf in info)
            {
                //Console.WriteLine(cf);
            }

            //Console.WriteLine();
            // get all the methods
            //Console.WriteLine("Methods:");
            foreach (MethodInfo mf in methods)
            {
                //Console.WriteLine(mf);
            }
        }




        public static bool primitive(Type typeOb)
        {
            for (int i = 0; i < Serial.primitives.Length; i++)
            {
                if (Serial.primitives[i].Equals(typeOb))
                {
                    return true;
                }
            }
            return false;
        }




    }
}
