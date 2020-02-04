using System;
using KeyCommon;
using KeyGenerate;
using KeyVerify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyTest
{
    [TestClass]
    public class KeyTests
    {
        [TestMethod]
        public void Test_pkv_licence_key_generation_and_verification()
        {
            var pkvLicenceKey = new KeyGenerator();

            var pkvKeyCheck = new KeyCheck();

            var key = string.Empty;

            KeyByteSet[] keyByteSets =
            {
                new KeyByteSet(1, 58, 6, 97),
                new KeyByteSet(2, 96, 254, 23),
                new KeyByteSet(3, 11, 185, 69),
                new KeyByteSet(4, 2, 93, 41),
                new KeyByteSet(5, 62, 4, 234),
                new KeyByteSet(6, 200, 56, 49),
                new KeyByteSet(7, 89, 45, 142),
                new KeyByteSet(8, 6, 88, 32)
            };

            // Change these to a random key byte set from the above array to test key verification with

            var kbs1 = keyByteSets[3];
            var kbs2 = keyByteSets[7];
            var kbs3 = keyByteSets[4];

            // The check project also uses a class called KeyByteSet, but with
            // separate name spacing to achieve single self contained dll

            var keyByteSet1 =
                new KeyByteSet(kbs1.KeyByteNo, kbs1.KeyByteA, kbs1.KeyByteB, kbs1.KeyByteC); // Change no to test others
            var keyByteSet2 = new KeyByteSet(kbs2.KeyByteNo, kbs2.KeyByteA, kbs2.KeyByteB, kbs2.KeyByteC);
            var keyByteSet3 = new KeyByteSet(kbs3.KeyByteNo, kbs3.KeyByteA, kbs3.KeyByteB, kbs3.KeyByteC);

            for (var i = 0; i < 10000; i++)
            {
                var seed = new Random().Next(0, int.MaxValue);

                key = pkvLicenceKey.MakeKey(seed, keyByteSets);

                // Check that check sum validation passes
                Assert.IsTrue(pkvKeyCheck.CheckKeyChecksum(key, keyByteSets.Length));

                // Check using full check method
                Assert.IsTrue(pkvKeyCheck.CheckKey(
                                  key,
                                  new[] {keyByteSet1, keyByteSet2, keyByteSet3},
                                  keyByteSets.Length,
                                  null
                              ) == LicenceKeyResult.KeyGood, "Failed on iteration " + i
                );

                // Check that erroneous check sum validation fails
                Assert.IsFalse(pkvKeyCheck.CheckKeyChecksum(key.Remove(23, 1) + "A",
                    keyByteSets.Length)); // Change key by replacing 17th char
            }

            // Check a few random inputs
            Assert.IsFalse(pkvKeyCheck.CheckKey("adcsadrewf",
                               new[] {keyByteSet1, keyByteSet2},
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
            );
            Assert.IsFalse(pkvKeyCheck.CheckKey("",
                               new[] {keyByteSet1, keyByteSet2},
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
            );
            Assert.IsFalse(pkvKeyCheck.CheckKey("123",
                               new[] {keyByteSet1, keyByteSet2},
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
            );
            Assert.IsFalse(pkvKeyCheck.CheckKey("*()",
                               new[] {keyByteSet1, keyByteSet2},
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
            );
            Assert.IsFalse(pkvKeyCheck.CheckKey("dasdasdasgdjwqidqiwd21887127eqwdaishxckjsabcxjkabskdcbq2e81y12e8712",
                               new[] {keyByteSet1, keyByteSet2},
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
            );
        }

        [TestMethod]
        public void
            Test_PKV_licence_key_generation_and_verification_with_random_key_bytes_key_byte_qty_and_verification_key_byte_selection()
        {
            var pkvLicenceKey = new KeyGenerator();

            var pkvKeyCheck = new KeyCheck();

            for (var i = 0; i < 10000; i++)
            {
                var randomKeyByteSetsLength = new Random().Next(2, 400);

                var keyByteSets = new KeyByteSet[randomKeyByteSetsLength];

                for (var j = 0; j < randomKeyByteSetsLength; j++)
                {
                    var random = new Random();

                    var kbs = new KeyByteSet
                    (
                        j + 1,
                        (byte) random.Next(0, 256),
                        (byte) random.Next(0, 256),
                        (byte) random.Next(0, 256)
                    );

                    keyByteSets[j] = kbs;
                }

                // Select a random key byte set to test key verification with

                var kbs1 = keyByteSets[new Random().Next(0, randomKeyByteSetsLength)];
                var kbs2 = keyByteSets[new Random().Next(0, randomKeyByteSetsLength)];

                // The check project also uses a class called KeyByteSet, but with
                // separate name spacing to achieve single self contained dll

                var keyByteSet1 =
                    new KeyByteSet(kbs1.KeyByteNo, kbs1.KeyByteA, kbs1.KeyByteB,
                        kbs1.KeyByteC); // Change no to test others
                var keyByteSet2 = new KeyByteSet(kbs2.KeyByteNo, kbs2.KeyByteA, kbs2.KeyByteB, kbs2.KeyByteC);

                var seed = new Random().Next(0, int.MaxValue);

                var key = pkvLicenceKey.MakeKey(seed, keyByteSets);

                // Check that check sum validation passes

                Assert.IsTrue(pkvKeyCheck.CheckKeyChecksum(key, keyByteSets.Length));

                // Check using full check method

                Assert.IsTrue(pkvKeyCheck.CheckKey(
                                  key,
                                  new[] {keyByteSet1, keyByteSet2},
                                  keyByteSets.Length,
                                  null
                              ) == LicenceKeyResult.KeyGood, "Failed on iteration " + i
                );
            }
        }
    }
}