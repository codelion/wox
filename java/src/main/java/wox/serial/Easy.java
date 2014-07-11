package wox.serial;

import org.jdom2.Element;
import org.jdom2.Document;
import org.jdom2.input.SAXBuilder;
import org.jdom2.output.XMLOutputter;
import java.io.FileWriter;
import java.io.InputStream;
import java.io.FileInputStream;

/**
 * The <code>Easy</code> class is used to serialize/de-serialize objects to/from XML.
 * It has two static methods. The <code>save</code> method serializes an object to XML
 * and stores it in an XML file; and the <code>load</code> method de-serializes an object
 * from an XML file.
 *
 * @author Simon M. Lucas <br />
 *         Carlos R. Jaimez Gonzalez
 * @version Easy.java - 1.0
 */
public class Easy {

    /**
     * This method saves an object to the specified XML file. Example: <br /><br />
     * <code>
     * ArrayList list = new ArrayList();  <br />
     * list.add(new Product("Beans", 500)); <br />
     * list.add(new Product("Bread", 200)); <br />
     * Easy.save(list, "list.xml");
     * </code>
     * @param ob Any object.
     * @param filename This is the XML file where the object will be stored.
     */
    public static void save(Object ob, String filename) {
        try {
            ObjectWriter writer = new SimpleWriter();
            Element el = writer.write(ob);
            XMLOutputter out = new XMLOutputter(); // ("  ", true);
            FileWriter file = new FileWriter(filename);
            out.output(el, file);
            file.close();
            System.out.println("Saved object to " + filename);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * This method loads an object from the specified XML file. Example: <br /><br />
     * <code>
     * ArrayList list = (ArrayList)Easy.load("list.xml");
     * </code>
     * @param filename The XML file where the object is stored.
     * @return Object The live object.
     */
    public static Object load(String filename) {
        try {
            SAXBuilder builder = new SAXBuilder();
            InputStream is = new FileInputStream(filename);
            Document doc = builder.build(is);
            Element el = doc.getRootElement();
            ObjectReader reader = new SimpleReader();
            return reader.read(el);
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }
}
