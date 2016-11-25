using System;

namespace EC.Version
{
    /// <summary>
    /// These extension methods help pack and unpack versions and packed integers
    /// </summary>
    public static partial class ExtensionMethods
    {
        // These constants support version packing all the way up to 15.15.15.32767
        // This also means unpacking supports up to SVN revision 32767
        private const int DefaultMajorMask = 0xF;  // 4 bits
        private const int DefaultMinorMask = 0xF;  // 4 bits
        private const int DefaultBuildMask = 0xFF; // 8 bits

        // Hopefully this never changes, I certainly never expect it to!
        private const int BitsPerByte = 8;

        // This is the bit offset where all packing begins.
        // We subtract by one to exclude packing into the sign bit.
        // This gives our packed integers the property of always being positive.
        private const int StartingBitOffset = sizeof(int) * BitsPerByte - 1;

        // This function creates the bit mask geometry for the 4 int components being packed
        // A geometry is simply the stacked masks within the full 31 bit integer space
        // Our default geometry is 4, 4, 8, 15 (number of bits for each component)
        // If we sum the geometry values together we should always get 31.
        // We sum to 31 because we must not pack into the integer sign bit (most significant bit),
        // otherwise our packed value could become negative which has comparator implications.
        // This function only requests masks for the first 3 components, the final component's mask geometry
        // is determined by taking the remaining bit space within the desired geometry.
        // This function assumees you are passing in sane masks, it may break down with invalid masks
        // This means you must use values like 0x1, 0x3, 0x7, 0xF, 0x1F, 0x3F, 0x7F, 0xFF, ...
        private static int[] GetMaskGeometry(int majorMask, int minorMask, int buildMask)
        {
            // We use Log base 2 to determine mask size because a mask is essentially a
            // power of 2 (minus 1 to get the full bit mask). Because we need to subtract
            // 1 to get the full mask we must call Ceiling on the result.
            // i.e., 0xF = 15 (dec) = 0b1111 = 2^4 - 1
            // Log_2(16) = 4, Log_2(15) ~= 3.9
            // Ceiling(3.9) = 4 = the mask size

            var majorMaskSize = (int)Math.Ceiling(Math.Log(majorMask, 2));
            var minorMaskSize = (int)Math.Ceiling(Math.Log(minorMask, 2));
            var buildMaskSize = (int)Math.Ceiling(Math.Log(buildMask, 2));
            
            // calculate the remaining mask space
            var revisionMaskSize = StartingBitOffset - majorMaskSize - minorMaskSize - buildMaskSize;

            // ensure there is enough geometry space for at least a single fourth bit in the geometry
            if (revisionMaskSize <= 0)
            {
                throw new InvalidOperationException("Not Enough Bits Left for Revision, Need {revisionMaskSize * -1} Additional Bits");
            }

            return new[] { majorMaskSize, minorMaskSize, buildMaskSize, revisionMaskSize };
        }

        // This function simply checks to make sure that the value being packed can be safely represented by the provided mask
        // The type parameter is simply for error reporting
        private static void ValidateMaskSize(int value, int maskSize, string type)
        {
            // We must calculate the required mask size for the provided value.
            // We do this by getting the Log base 2 of the value, adding one, and then flooring the result
            // Log base 2 of any value will return the required mask size with one bit lacking, this is why we add one and floor
            // Log_2(1) = 0 -> 0 + 1 = 1 bit to mask the value 1
            // Log_2(2) = 1 -> 1 + 1 = 2 bits to mask the value 2
            // Log_2(15) ~= 3.9 -> 3.9 + 1 = 4.9 -> 4 bits to mask the value 15
            var valueSize = (int)Math.Floor(Math.Log(value, 2) + 1);

            // If the mask required for the provided value is larger than the mask we intend to use, then throw an exception
            if (valueSize > maskSize)
            {
                throw new InvalidOperationException("Not Enough Bits to Represent {type}: Have {maskSize}, Need {valueSize}");
            }
        }

        // This function validates all masks for each component in the provided version
        private static void ValidateMaskGeometry(System.Version version, int[] maskGeometry)
        {
            ValidateMaskSize(version.Major, maskGeometry[0], "Major");
            ValidateMaskSize(version.Minor, maskGeometry[1], "Minor");
            ValidateMaskSize(version.Build, maskGeometry[2], "Build");
            ValidateMaskSize(version.Revision, maskGeometry[3], "Revision");
        }

        /// <summary>
        /// Packs the provided <paramref name="version"/> into an int using the provided masks.
        /// Default sane masks are used if none are supplied.
        /// </summary>
        /// <param name="version">Version to pack</param>
        /// <param name="majorMask">Major component mask</param>
        /// <param name="minorMask">Minor component mask</param>
        /// <param name="buildMask">Build component mask</param>
        /// <returns>A positive integer with all version components packed into it</returns>
        public static int Pack(this System.Version version, int majorMask = DefaultMajorMask, int minorMask = DefaultMinorMask, int buildMask = DefaultBuildMask)
        {
            // First produce the mask geometry
            var maskGeometry = GetMaskGeometry(majorMask, minorMask, buildMask);

            // Now validate that we can fit our version components within the produced geometry
            ValidateMaskGeometry(version, maskGeometry);

            // Assign a current bit offset to the starting location
            var bitOffset = StartingBitOffset;

            // Pack the components into an integer
            // Since we already know that our version components fit within the mask geometry (the validation step above)
            // we don't need to re-mask them here, we can just bit shift their values into place
            // We are taking advantage of how a -= expression returns the result of the expression, but also updates the 
            // state of our bitOffset pointer. For each component we incrementally adjust our pointer by the corresponding
            // geometry for that component.
            // A visualization of this process is as follows (for a 0xF, 0xF, 0xFF, 0x7FFF geometry, the default):
            // These are the bit masks
            // Major    = 0b00000000000000000000000000001111
            // Minor    = 0b00000000000000000000000000001111
            // Build    = 0b00000000000000000000000011111111
            // Revision = 0b00000000000001111111111111111111
            // Now we shift these over using our geometry
            // The first shift for Major is from our bit offset minus the first geometry, 31 - 4 = 27
            // 0b00000000000000000000000000001111
            //    xxxx----<<----27----<<----xxxx
            // 0b01111000000000000000000000000000
            // Our bit offset is in place adjusted to 27 for the next expression
            // We simply perform the same steps again for each geometry
            // 27 - 4 = shift Minor by 23, 23 - 8 = shift Build by 15
            // The last component does not need to be shifted at all since it just sits at the least significant bits anyways
            // Now that each component is shifted to its packed location, we logical OR these parts together
            // We have these components to merge together
            // 0b0aaaa000000000000000000000000000
            // 0b00000bbbb00000000000000000000000
            // 0b000000000cccccccc000000000000000
            // 0b00000000000000000ddddddddddddddd
            // Logical OR will simply merge zeroes and the non-zeroes. Since no non-zeroes overlap, we end up with a nice packed integer like this:
            // 0b0aaaabbbbccccccccddddddddddddddd
            // This number is our POSITIVE packed integer
            return
                version.Major << (bitOffset -= maskGeometry[0]) | 
                version.Minor << (bitOffset -= maskGeometry[1]) | 
                version.Build << (bitOffset -= maskGeometry[2]) |
                version.Revision;
        }

        /// <summary>
        /// Unpacks a (positive) integer into its four components, then construct a version from these components.
        /// Default sane masks are used if none are supplied.
        /// </summary>
        /// <param name="value">Packed integer to unpack</param>
        /// <param name="majorMask">Major component mask</param>
        /// <param name="minorMask">Minor component mask</param>
        /// <param name="buildMask">Build component mask</param>
        /// <returns>A version that was previously represented by the packed integer</returns>
        public static System.Version Unpack(this int value, int majorMask = DefaultMajorMask, int minorMask = DefaultMinorMask, int buildMask = DefaultBuildMask)
        {
            // First produce the mask geometry
            var maskGeometry = GetMaskGeometry(majorMask, minorMask, buildMask);

            // No need to validate the geometry this time because we don't know what the version values are ahead of time

            // Assign a current bit offset to the starting location
            var bitOffset = StartingBitOffset;

            // Unpack the integer into the version components and create a new version instance
            // The strategy here is to (for each component) bit shift the packed value so that it
            // sits at the least significant bits, then apply the supplied mask to trim off any bits
            // that are more significant than the mask. The final component is already sitting at the 
            // least significant bits, so we just want to produce a mask that trims off everything else.
            // To create this mask, we use our current bit offset and first shift a full 32 bit mask 
            // right by our current offset amount. This yields our inverse mask, so we negate the mask to 
            // achieve our desired mask.
            // Our bit offset math is very similar to packing, 31 - 4 = 27, 27 - 4 = 23, 23 - 8 = 15
            // For the first three components, our operations look like this:
            // 0b01111000000000000000000000000000
            //    xxxx---->>----27---->>----xxxx
            // 0b00000000000000000000000000001111
            // The final component is a bit more complicated. Given our bit offset is sitting at 15 we 
            // start with the full 32 bit mask (all ones) then we shift this mask LEFT by 15 bits, from our current bit offset.
            // this produces zeroes at the least significant portion of the bit mask that correspond to the packed fourth component.
            // 0b11111111111111111111111111111111
            //   ---------<<----15----<<---------
            // 0b11111111111111111000000000000000
            // given that our mask is now just the inverse of what we want, we can negate the mask to get what we need
            // 0b00000000000000000111111111111111
            // When applied to our packed value, we trim off all the other components and are left with just the fourth component
            return new System.Version(
                (value >> (bitOffset -= maskGeometry[0])) & majorMask,
                (value >> (bitOffset -= maskGeometry[1])) & minorMask,
                (value >> (bitOffset -= maskGeometry[2])) & buildMask,
                value & ~(int.MaxValue << bitOffset)
            );
        }

        // this is a simple helper method to visualize a bit mask, 
        // we don't actually need it for anything other than testing
        //public static string ToBitString(this int value)
        //{
        //    return Convert.ToString(value, 2).PadLeft(sizeof(int) * BitsPerByte, '0');
        //}
    }
}
