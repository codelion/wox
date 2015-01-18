/*
 * © Copyright 2015 
 */

package wox.serial.tests;

import com.pholser.junit.quickcheck.generator.GenerationStatus;
import com.pholser.junit.quickcheck.generator.Generator;
import com.pholser.junit.quickcheck.random.SourceOfRandomness;
import org.apache.commons.lang3.RandomStringUtils;
import org.jdom2.Element;

/**
 *
 */
public class ElementGenerator extends Generator<Element> {

  ///////////////////////////// Class Attributes \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

  ////////////////////////////// Class Methods \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

  //////////////////////////////// Attributes \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

  /////////////////////////////// Constructors \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
  
  public ElementGenerator() {
    super(Element.class);
  }
  
  ////////////////////////////////// Methods \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

  //------------------------ Implements:

  //------------------------ Overrides: generate
  
  @Override
  public Element generate(SourceOfRandomness rand, GenerationStatus gs) {
    Element e = new Element(RandomStringUtils.randomAlphabetic(16));
    int numofAttr = rand.nextInt(8);
    for(int i=0; i<numofAttr;i++){
      e.setAttribute(RandomStringUtils.randomAlphabetic(8),RandomStringUtils.randomAlphabetic(8));
    }
    e.addContent(RandomStringUtils.randomAlphabetic(rand.nextInt(16)));
    return e;
  }
  
  //---------------------------- Abstract Methods -----------------------------

  //---------------------------- Utility Methods ------------------------------

  //---------------------------- Property Methods -----------------------------
}
