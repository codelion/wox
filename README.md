wox
===

WOX is an XML serializer for Java and C# objects 
(forked from http://woxserializer.sourceforge.net/)

### Features

Some of the WOX main features are listed below.

- Easy to use. The Easy class provides serialization and de-serialization methods.
- Simple XML. The XML generated is simple, easy to understand, and language independent.
- Requires no class modifications. Classes do not require to have default constructors, getters or setters.
- Field visibility. Private fields are serialized just as any other field. WOX serializes fields regardless their visibility.
- Interoperability Java and C#. WOX can serialize a Java object to XML, and reconstruct the XML back to a C# object; and viceversa.
- Standard XML object representation. This could potentially allow to have WOX serializers in different object-oriented programming languages.
- WOX data types. The WOX mapping table specifies how primitive data types are mapped to WOX data types.
- Robust to class changes. Defaults will be used for newly added fields.
- Arrays. Handles arrays and multi-dimensional arrays of primitives and Objects.
- Base-64. Byte arrays are base-64 encoded for efficiency.
- Collection classes. Lists and Maps are provided as WOX data types. (ArrayList and HashMap in Java; ArrayList and Hashtable in C#).
- Object references. Handles duplicate and circular object references with id/idref.
- Class and Type. Objects of these classes are saved by their String name.
- Small footprint. The woxSerializer.jar file (which contains only .class files) is only 25k.
