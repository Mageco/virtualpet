#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("cwgPZe+Y3PbffgyK3+rAMEJfkzYSOrqLKH0FPrFBjxK47DagxIPIVywXOTEaNV9hF09Lf9njGiNCLHGWu9xDDjl1cNFbgOPdM4b6A8MxJi+QhORniwaWHReAKmea1Ky+icWyKaC8ogU/iZVL9Nb26xHppsCir9BtcggayA9mW0ZYE1dJVVbu8z2gj75LqDsAuuDxqvoDxDzEmjDJ6hW5PW7cX3xuU1hXdNgW2KlTX19fW15dM++GqQ8xqe4A/YBrilyVzD83nOPcX1FebtxfVFzcX19eyFCuWCVMq8DNwOYHK9YpYGsF4Y3HehqrCZP03lnRaq+xWOgzuOkhXhyK751P6dOhq+Gx05BhK4AjTkaWW26tJSzboEN+RMteJNhQ0VxdX15f");
        private static int[] order = new int[] { 5,6,11,11,9,11,7,10,11,9,10,13,13,13,14 };
        private static int key = 94;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
