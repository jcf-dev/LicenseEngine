﻿#define Key01
#undef Key02
#undef Key03
#undef Key04
#undef Key05
#undef Key06
#define Key07
#undef Key08

using System;
using System.Globalization;
using KeyCommon;

namespace KeyVerify
{
    /// <summary>
    ///     Provides methods for verifying a licence key.
    /// </summary>
    public class KeyCheck
    {
        /// <summary>
        ///     Check a given key for validity
        /// </summary>
        /// <param name="key">The full key</param>
        /// <param name="keyByteSetsToCheck">The KeyBytes that are to be tested in this check</param>
        /// <param name="totalKeyByteSets">The total number of KeyBytes used to make the key</param>
        /// <param name="blackListedSeeds">Any seed values (hex string representation) that should be banned</param>
        /// <returns></returns>
        public LicenceKeyResult CheckKey(
            string key,
            KeyByteSet[] keyByteSetsToCheck,
            int totalKeyByteSets,
            string[] blackListedSeeds
        )
        {
            key = FormatKeyForCompare(key);

            var result = LicenceKeyResult.KeyInvalid;

            var checksumPass = CheckKeyChecksum(key, totalKeyByteSets);

            if (checksumPass)
            {
                if (blackListedSeeds != null && blackListedSeeds.Length > 0)
                    // Test key against our black list

                    // Example black listed seed: 111111 (Hex val). Producing keys with the same 
                    // seed and key bytes will produce the same key, so using a seed such as a user id
                    // can provide a mechanism for tracking the source of any keys that are found to
                    // be used out of licence terms.

                    for (var i = 0; i < blackListedSeeds.Length; i++)
                        if (key.StartsWith(blackListedSeeds[i]))
                            result = LicenceKeyResult.KeyBlackListed;

                if (result != LicenceKeyResult.KeyBlackListed)
                {
                    // At this point, the key is either valid or forged,
                    // because a forged key can have a valid checksum.
                    // We now test the "bytes" of the key to determine if it is
                    // actually valid.

                    // When building your release application, use conditional defines
                    // or comment out most of the byte checks!  This is the heart
                    // of the partial key verification system. By not compiling in
                    // each check, there is no way for someone to build a keygen that
                    // will produce valid keys.  If an invalid keygen is released, you
                    // simply change which byte checks are compiled in, and any serial
                    // number built with the fake keygen no longer works.

                    // Note that the parameters used for PKV_GetKeyByte calls MUST
                    // MATCH the values that PKV_MakeKey uses to make the key in the
                    // first place!

                    result = LicenceKeyResult.KeyPhoney;

                    int seed;

                    var seedParsed = int.TryParse(key.Substring(0, 8), NumberStyles.HexNumber, null, out seed);

                    if (seedParsed)
                    {
                        string keyBytes;
                        byte b;
                        int keySubstringStart;

                        // The using of conditional compilation for the key byte checks
                        // means that we define the portions of the key that are checked
                        // at runtime. The advantage of this is that if someone creates
                        // a keygen using this source code, we can change the key bytes that
                        // are checked, so subsequent releases will not work with keys generated by 
                        // the key generator.

                        foreach (var keyByteSet in keyByteSetsToCheck)
                        {
                            keySubstringStart = GetKeySubstringStart(keyByteSet.KeyByteNo);

                            if (keySubstringStart - 1 > key.Length)
                                throw new InvalidOperationException(
                                    "The KeyByte check position is out of range. You may have specified a check KeyByteNo that did not exist in the original key generation.");

                            keyBytes = key.Substring(keySubstringStart, 2);
                            b = GetKeyByte(seed, keyByteSet.KeyByteA, keyByteSet.KeyByteB, keyByteSet.KeyByteC);

                            if (keyBytes != b.ToString("X2"))
                                // If true, then it means the key is either good, or was made
                                // with a keygen derived from "this" release.

                                return result; // Return result in failed state 
                        }

                        result = LicenceKeyResult.KeyGood;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Short hand way of creating pattern 8, 10, 12, 14
        /// </summary>
        /// <param name="keyByteNo"></param>
        /// <returns></returns>
        private int GetKeySubstringStart(int keyByteNo)
        {
            return keyByteNo * 2 + 6;
        }

        /// <summary>
        ///     Indicate if the check sum portion of the key is valid
        /// </summary>
        /// <param name="key"></param>
        /// <param name="totalKeyByteSets"> </param>
        /// <returns></returns>
        public bool CheckKeyChecksum(string key, int totalKeyByteSets)
        {
            var result = false;

            var formattedKey = FormatKeyForCompare(key);

            if (formattedKey.Length == 8 + 4 + 2 * totalKeyByteSets
            ) // First 8 are seed, 4 for check sum, plus 2 for each KeyByte
            {
                var keyLessChecksumLength = formattedKey.Length - 4;

                var checkSum = formattedKey.Substring(keyLessChecksumLength, 4); // Last 4 chars are checksum

                var keyWithoutChecksum = formattedKey.Substring(0, keyLessChecksumLength);

                result = GetChecksum(keyWithoutChecksum) == checkSum;
            }

            return result;
        }

        ////////////////////////////////////////////////////
        // Code below from here is duplicated across both
        // private and public projects / dlls
        ////////////////////////////////////////////////////

        /// <summary>
        ///     Strip padding chars for comparison
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKeyForCompare(string key)
        {
            if (key == null) key = string.Empty;

            // Replace -, space etc, upper case

            return key.Trim().ToUpper().Replace("-", string.Empty).Replace(" ", string.Empty);
        }

        /// <summary>
        ///     Given a seed and some input bytes, generate a single byte to return. This should
        ///     be used with randomised data, that can be represented to retrieve the same key.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private byte GetKeyByte(long seed, byte a, byte b, byte c)
        {
            var aTemp = a % 25;
            var bTemp = b % 3;

            long result;

            if (a % 2 == 0)
                result = ((seed >> aTemp) & 0xFF) ^ ((seed >> bTemp) | c);
            else
                result = ((seed >> aTemp) & 0xFF) ^ ((seed >> bTemp) & c);

            return (byte) result;
        }

        /// <summary>
        ///     Generate a new checksum for a key
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetChecksum(string str)
        {
            ushort left = 0x56;
            ushort right = 0xAF;

            if (str.Length > 0)
                // 0xFF hex for 255

                for (var cnt = 0; cnt < str.Length; cnt++)
                {
                    right = (ushort) (right + Convert.ToByte(str[cnt]));

                    if (right > 0xFF) right -= 0xFF;

                    left += right;

                    if (left > 0xFF) left -= 0xFF;
                }

            var sum = (ushort) ((left << 8) + right);

            return sum.ToString("X4"); // 4 char hex
        }
    }
}