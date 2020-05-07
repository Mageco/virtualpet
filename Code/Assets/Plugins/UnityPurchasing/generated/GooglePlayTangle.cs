#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("J11PnVozDhMNRgIcAAO7pmj12uuLDIQ/+uQNvWbtvHQLSd+6yBq8hvT+tOSGxTR+1XYbE8MOO/hweY71R2/v3n0oUGvkFNpH7blj9ZHWnQKJCgQLO4kKAQmJCgoLnQX7DXAZ/sXRsTLeU8NIQtV/Ms+B+evckOd8lZiVs1J+g3w1PlC02JIvT/5cxqFmutP8WmT8u1Wo1T7fCcCZamLJth79blXvtaT/r1aRaZHPZZy/QOxo7okWW2wgJYQO1baIZtOvVpZkc3o7iQopOwYNAiGNQ438BgoKCg4LCCZdWjC6zYmjiitZ34q/lWUXCsZj9en3UGrcwB6hg6O+RLzzlff6hTh5QmxkT2AKNEIaHiqMtk92F3kkwxYrEZ4LcY0FhAkICgsK");
        private static int[] order = new int[] { 7,8,12,6,8,9,13,8,10,12,10,12,13,13,14 };
        private static int key = 11;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
