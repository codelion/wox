/*
 * This file was automatically generated by EvoSuite
 * Fri Dec 26 00:34:15 SGT 2014
 */

package wox.serial;

import static org.junit.Assert.*;
import org.junit.Test;
import org.evosuite.runtime.EvoRunner;
import org.evosuite.runtime.EvoRunnerParameters;
import org.evosuite.runtime.EvoSuiteFile;
import org.junit.runner.RunWith;
import wox.serial.TypeMapping;

@RunWith(EvoRunner.class) @EvoRunnerParameters(mockJVMNonDeterminism = true, useVFS = true, resetStaticState = true) 
public class TypeMapping_ESTest extends TypeMapping_ESTest_scaffolding {

  //Test case number: 0
  /*
   * 1 covered goal:
   * 1 wox.serial.TypeMapping.<init>()V: root-Branch
   */

  @Test
  public void test0()  throws Throwable  {
      TypeMapping typeMapping0 = new TypeMapping();
  }
}
