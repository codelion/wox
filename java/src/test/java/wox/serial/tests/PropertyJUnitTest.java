
package wox.serial.tests;

import org.junit.contrib.theories.*;
import org.junit.runner.RunWith;
import org.junit.Assert;
import com.pholser.junit.quickcheck.ForAll;
import com.pholser.junit.quickcheck.From;
import wox.serial.EncodeBase64;
import static org.hamcrest.Matchers.*;
import org.jdom2.Element;
import static org.junit.Assume.assumeThat;
import static wox.serial.XMLUtil.element2String;

/**
 *
 * @author asankhaya
 */
@RunWith(Theories.class)
public class PropertyJUnitTest {
  @Theory public void testEncodeBase64(@ForAll byte [] src){
    byte [] ec = EncodeBase64.encode(src);
    byte [] dec = EncodeBase64.decode(ec);
    Assert.assertArrayEquals(src,dec);
  }
  
  @Theory public void testEncodeBase64withLength(@ForAll byte [] src){
    assumeThat(src.length, greaterThan(32)); 
    byte [] ec = EncodeBase64.encode(src);
    byte [] dec = EncodeBase64.decode(ec);
    Assert.assertArrayEquals(src,dec);
  }
  
  @Theory public void testElement2String(@ForAll @From(ElementGenerator.class) Element e) throws Exception {
    String s = element2String(e);
    Assert.assertNotNull(s);
  }
}
