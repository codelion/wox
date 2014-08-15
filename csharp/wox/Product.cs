using System;
using wox.serial;

/**
 * This class provides an example of a class with some primitive fields. The main method
 * uses the Easy class to serialize an array of Product objects to XML (save method);
 * and to de-serialize the XML to an array of Product objects back again (load method).
 * http://woxserializer.sourceforge.net/
 *
 * Authors: Carlos R. Jaimez Gonzalez
 *          Simon M. Lucas
 * Version: Product.cs - 1.0
 */
public class Product
{
    private String name;
    private double price;
    private int grams;
    private bool registered;
    private char category;

    public Product()
    {
    }

    public Product(String name, double price, int grams, bool registered, char category)
    {
        this.name = name;
        this.price = price;
        this.grams = grams;
        this.registered = registered;
        this.category = category;
    }

    public override string ToString()
    {
        return "name: " + name + ", price: " + price + ", grams: " + grams +
               ", registered: " + registered + ", category: " + category;
    }

    public static void printProducts(Product[] products)
    {
        for (int i = 0; i < products.Length; i++)
        {
            Console.Out.WriteLine("" + products[i]);
        }
    }

    /**
     * This method shows how easy is to serialize and de-serialize C# objects
     * to/from XML. The XML representation of the objects is a standard WOX represntation.
     * For more information about the XML representation please visit:
     * http://woxserializer.sourceforge.net/
     */
    public static void main(String[] args)
    {
        Product[] products = new Product[]{new Product("Baked beans", 1.75, 250, true, 'B'),
                                           new Product("Basmati Rice", 3.89, 750, true, 'R'),
                                           new Product("White bread", 1.06, 300, false, 'H')};

        String filename = "C:\\TestProductsCSharp.xml";
        //print the array of objects
        printProducts(products);
        //object to standard XML
        Easy.save(products, filename);
        //load the object (array) from the XML file
        Product[] newProducts = (Product[])Easy.load(filename);
        //print the new object - it is the same as before
        printProducts(newProducts);
    }

}

