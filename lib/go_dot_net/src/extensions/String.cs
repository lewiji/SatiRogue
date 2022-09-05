namespace SatiRogue.lib.go_dot_net.src.extensions;

/// <summary>
/// String extensions.
/// </summary>
public static class StringX {
   /// <summary>
   /// Returns the string with the first letter capitalized.
   /// </summary>
   /// <param name="input">String receiver.</param>
   /// <returns>The string with the first letter capitalized.</returns>
   public static string ToNameCase(this string input) {
      return input switch {
         null => "",
         "" => "",
         _ => input[0].ToString().ToUpper() + input[1..]
      };
   }
}